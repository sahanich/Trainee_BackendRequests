using System;

public class GalleryEventsMediator
{
    public event Action<PicturePlace> PictureReplaceRequested;
    public event Action<PicturePlace, UserImageData> PictureForReplaceSelected;

    public void RaisePictureReplaceRequested(PicturePlace picturePlace)
    {
        PictureReplaceRequested?.Invoke(picturePlace);
    }

    public void RaisePictureForReplaceSelected(PicturePlace picturePlace, UserImageData userImage)
    {
        PictureForReplaceSelected?.Invoke(picturePlace, userImage);
    }
}
