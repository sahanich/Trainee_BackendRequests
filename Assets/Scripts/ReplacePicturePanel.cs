using System;
using System.Collections.Generic;
using UnityEngine;

public class ReplacePicturePanel : MonoBehaviour
{
    [SerializeField]
    private UserPicture PicturePrefab;
    [SerializeField]
    private Transform PicturesContent;

    private List<UserPicture> _pictureInstances = new();

    public event Action<UserPicture> PictureForReplaceSelected;

    public void ShowPanel(UserImageData[] userImages)
    {
        Clear();
        foreach (var item in userImages)
        {
            UserPicture picture = Instantiate(PicturePrefab, PicturesContent);
            picture.SetPicture(item, isPreview: true);
            picture.PictureClicked += OnPictureClicked;
            _pictureInstances.Add(picture);
        }
        PicturesContent.gameObject.SetActive(true);
    }

    public void HidePanel()
    {
        Clear();
        PicturesContent.gameObject.SetActive(false);
    }

    private void Clear()
    {
        foreach (var item in _pictureInstances)
        {
            Destroy(item.gameObject);
        }
        _pictureInstances.Clear();
    }

    private void OnPictureClicked(UserPicture picture)
    {
        PictureForReplaceSelected?.Invoke(picture);
    }
}
