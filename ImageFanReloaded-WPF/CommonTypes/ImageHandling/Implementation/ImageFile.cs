using System.Drawing;
using System.IO;
using System.Windows.Media;

namespace ImageFanReloaded.CommonTypes.ImageHandling.Implementation
{
    public class ImageFile
        : IImageFile
    {
        public ImageFile(IImageResizer imageResizer, string imageFilePath)
        {
            _imageResizer = imageResizer;
            _imageFilePath = imageFilePath;

            FileName = Path.GetFileName(imageFilePath);

            ImageSize = GlobalData.InvalidImageSize;

			_thumbnailGenerationLockObject = new object();
        }

        public string FileName { get; }

        public ImageSize ImageSize { get; private set; }

        public ImageSource GetImage()
        {
            Bitmap image = null;
            ImageSource imageSource;

            try
            {
                image = new Bitmap(_imageFilePath);
				ImageSize = new ImageSize(image.Width, image.Height);

				imageSource = image.ConvertToImageSource();
            }
            catch
            {
				imageSource = GlobalData.InvalidImage;
				ImageSize = GlobalData.InvalidImageSize;
            }
            finally
            {
                image?.Dispose();
            }

            return imageSource;
        }

        public ImageSource GetResizedImage(ImageSize viewPortSize)
        {
            Bitmap image = null;
            Bitmap resizedImage = null;
            ImageSource resizedImageSource;

            try
            {
                image = new Bitmap(_imageFilePath);
				ImageSize = new ImageSize(image.Width, image.Height);

				resizedImage = _imageResizer.CreateResizedImage(image, viewPortSize);
                resizedImageSource = resizedImage.ConvertToImageSource();
            }
            catch
            {
				resizedImageSource = GlobalData.InvalidImage;
				ImageSize = GlobalData.InvalidImageSize;
			}
            finally
            {
                image?.Dispose();
                resizedImage?.Dispose();
            }

            return resizedImageSource;
        }

        public void ReadImageDataFromDisc()
        {
            lock (_thumbnailGenerationLockObject)
            {
                try
                {
                    _imageData = new Bitmap(_imageFilePath);
					ImageSize = new ImageSize(_imageData.Width, _imageData.Height);
				}
                catch
                {
                    _imageData = GlobalData.InvalidImageAsBitmap;
                }
            }
        }

        public ImageSource GetThumbnail()
        {
            lock (_thumbnailGenerationLockObject)
            {
                Bitmap thumbnail = null;
                ImageSource thumbnailSource;

                try
                {
                    thumbnail = _imageResizer.CreateResizedImage(_imageData, GlobalData.ThumbnailSize);
                    thumbnailSource = thumbnail.ConvertToImageSource();
                }
                catch
                {
                    thumbnailSource = GlobalData.InvalidImageThumbnail;
				}
                finally
                {
                    thumbnail?.Dispose();

                    DisposeImageData();
                }

                return thumbnailSource;
            }
        }

        public void DisposeImageData()
        {
            if (_imageData != null &&
                _imageData != GlobalData.InvalidImageAsBitmap)
            {
                _imageData.Dispose();
            }

			_imageData = null;
		}

        #region Private

        private readonly IImageResizer _imageResizer;
        private readonly string _imageFilePath;
        private readonly object _thumbnailGenerationLockObject;

        private Bitmap _imageData;

		#endregion
	}
}
