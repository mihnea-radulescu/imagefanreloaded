using ImageFanReloaded.CommonTypes.ImageHandling.Interface;
using System;
using System.Diagnostics;
using System.Windows.Media;

namespace ImageFanReloaded.CommonTypes.Info
{
    [DebuggerDisplay("{ThumbnailText}")]
    public class ThumbnailInfo
    {
        public ThumbnailInfo(IImageFile imageFile)
        {
            if (imageFile == null)
                throw new ArgumentNullException("imageFile", "Image file cannot be null.");

            ImageFile = imageFile;
            _thumbnailImage = GlobalData.LoadingImageThumbnail;
        }

        public IImageFile ImageFile { get; private set; }

        public ImageSource ThumbnailImage
        { 
            get
            {
                return _thumbnailImage;
            }
            set
            {
                _thumbnailImage = value;
                OnThumbnailImageChanged(this, EventArgs.Empty);
            }
        }

        public string ThumbnailText
        { 
            get
            {
                return ImageFile.FileName;
            }
        }

        public event EventHandler ThumbnailImageChanged;


        #region Private

        private ImageSource _thumbnailImage;

        private void OnThumbnailImageChanged(object sender, EventArgs e)
        {
            var thumbnailImageChangedHandler = ThumbnailImageChanged;
            if (thumbnailImageChangedHandler != null)
                thumbnailImageChangedHandler(sender, e);
        }

        #endregion
    }
}
