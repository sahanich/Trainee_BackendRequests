using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GalleryInfrastructure : MonoBehaviour
{
    [SerializeField]
    private Transform PicturePlacesContent;

    private PicturePlace[] _picturePlaces;
    private GalleryEventsMediator _galleryEventsMediator = new();

    private void OnEnable()
    {
        _galleryEventsMediator.PictureReplaceRequested += OnPictureReplaceRequested;
        _galleryEventsMediator.PictureForReplaceSelected += OnPictureForReplaceSelected;
    }

    private void OnDisable()
    {
        _galleryEventsMediator.PictureReplaceRequested -= OnPictureReplaceRequested;
        _galleryEventsMediator.PictureForReplaceSelected -= OnPictureForReplaceSelected;
    }

    private void Start()
    {
        _picturePlaces = PicturePlacesContent.GetComponentsInChildren<PicturePlace>();
        
        foreach (var picturePlace in _picturePlaces)
        {
            picturePlace.Init(_galleryEventsMediator);
        }

        InitPicturesAsync().Forget();
    }

    private void OnPictureReplaceRequested(PicturePlace picturePlace)
    {
        ShowReplacePicturePanel(picturePlace).Forget();
    }

    private void OnPictureForReplaceSelected(PicturePlace picturePlace, UserImageData selectedImageData)
    {
        RequestReplacePicture(picturePlace, selectedImageData).Forget();
    }

    private async UniTask RequestReplacePicture(PicturePlace picturePlace, UserImageData newImageData)
    {
        UserSpaceData userSpaceWithImages = await GetUserSpaceWithImages();
        if (userSpaceWithImages == null)
        {
            return;
        }

        Dictionary<int, UserImageData> newImagesMap = new();

        foreach (var image in userSpaceWithImages.Images)
        {
            if (image.Key == picturePlace.PlaceNumber)
            {
                newImagesMap[image.Key] = newImageData;
            }
            else
            {
                newImagesMap[image.Key] = image.Value;
            }
        }

        await APIService.ReplaceAllMyPlacePictures(userSpaceWithImages.Id, newImagesMap);

        RefreshPlacesPictures(await GetUserSpaceWithImages());

        picturePlace.HideReplacePicturePanel();
    }

    private async UniTask ShowReplacePicturePanel(PicturePlace picturePlace)
    {
        picturePlace.ShowReplacePicturePanel(await APIService.GetUserImages());
    }

    private async UniTask InitPicturesAsync()
    {
        RefreshPlacesPictures(await GetUserSpaceWithImages());
    }

    private async UniTask<UserSpaceData> GetUserSpaceWithImages()
    {
        UserSpaceData[] userSpaceData = await APIService.GetUserSpaces();

        foreach (var item in userSpaceData)
        {
            if (item.Images != null && item.Images.Count > 0)
            {
                return item;
            }
        }

        return null;
    }

    private void RefreshPlacesPictures(UserSpaceData userSpaceData)
    {
        if (userSpaceData == null || userSpaceData.Images == null)
        {
            return;
        }

        foreach (var item in userSpaceData.Images)
        {
            PicturePlace place = _picturePlaces.FirstOrDefault(p => p.PlaceNumber == item.Key);
            if (place != null)
            {
                place.SetPicture(item.Value);
            }
        }
    }

}
