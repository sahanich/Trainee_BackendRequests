using System;
using UnityEngine;
using UnityEngine.UI;
using WebUtils;

public class UserPicture : MonoBehaviour
{
    [SerializeField]
    private Button PictureButton;
    [SerializeField]
    private Image Image;

    private WebDataDownloader _webDataDownloader = new();
    private UserImageData _imageData;

    public UserImageData ImageData => _imageData;

    public event Action<UserPicture> PictureClicked;

    private void OnEnable()
    {
        PictureButton.onClick.AddListener(OnPictureClicked);
    }

    private void OnDisable()
    {
        PictureButton.onClick.RemoveListener(OnPictureClicked);
    }

    public void SetPicture(UserImageData userImage, bool isPreview = false)
    {
        _imageData = userImage;

        _webDataDownloader.DownloadCompleted -= OnImageDownloadCompleted;
        _webDataDownloader.DownloadCompleted += OnImageDownloadCompleted;

        _webDataDownloader.DownloadFailed -= OnImageDownloadFailed;
        _webDataDownloader.DownloadFailed += OnImageDownloadFailed;

        Image.enabled = false;
        _webDataDownloader.DownloadData(isPreview ? userImage.PreviewUrl : userImage.FileUrl);
    }

    private void OnPictureClicked()
    {
        PictureClicked?.Invoke(this);
    }

    private void OnImageDownloadCompleted(byte[] rawData)
    {
        _webDataDownloader.DownloadCompleted -= OnImageDownloadCompleted;
        _webDataDownloader.DownloadFailed -= OnImageDownloadFailed;

        Texture2D texture2D = new Texture2D(1, 1);

        if (texture2D.LoadImage(rawData))
        {
            if (Image.sprite != null)
            {
                Destroy(Image.sprite);
            }

            Image.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.one / 2);
            Image.type = Image.Type.Simple;
            Image.preserveAspect = true;
        }

        Image.enabled = true;
    }

    private void OnImageDownloadFailed()
    {
        _webDataDownloader.DownloadCompleted -= OnImageDownloadCompleted;
        _webDataDownloader.DownloadFailed -= OnImageDownloadFailed;
    }
}
