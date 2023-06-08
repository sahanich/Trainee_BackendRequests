using API;
using UnityEngine;

public class PicturePlace : MonoBehaviour
{
    [SerializeField]
    private int Number;
    [SerializeField]
    private UserPicture Picture;
    [SerializeField]
    private ReplacePicturePanel ReplacePicturePanel;

    public int PlaceNumber => Number;

    private GalleryEventsMediator _galleryEventsMediator;

    private void OnEnable()
    {
        Picture.PictureClicked += OnReplacePictureButtonClick;
        ReplacePicturePanel.PictureForReplaceSelected += OnPictureForReplaceSelected;
    }

    private void OnDisable()
    {
        Picture.PictureClicked -= OnReplacePictureButtonClick;
        ReplacePicturePanel.PictureForReplaceSelected -= OnPictureForReplaceSelected;
    }

    public void Init(GalleryEventsMediator galleryEventsMediator)
    {
        _galleryEventsMediator = galleryEventsMediator;
    }

    public void SetPicture(UserImageData userImageData)
    {
        Picture.SetPicture(userImageData);
    }

    public void ShowReplacePicturePanel(UserImageData[] userImages)
    {
        ReplacePicturePanel.ShowPanel(userImages);
    }

    public void HideReplacePicturePanel()
    {
        ReplacePicturePanel.HidePanel();
    }

    private void OnReplacePictureButtonClick(UserPicture picture)
    {
        if (_galleryEventsMediator == null)
        {
            return;
        }
        _galleryEventsMediator.RaisePictureReplaceRequested(this);
    }

    private void OnPictureForReplaceSelected(UserPicture picture)
    {
        _galleryEventsMediator.RaisePictureForReplaceSelected(this, picture.ImageData);
    }
}
