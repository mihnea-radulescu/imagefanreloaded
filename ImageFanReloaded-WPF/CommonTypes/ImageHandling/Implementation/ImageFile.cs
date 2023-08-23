using System;
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

			ImageSize = new ImageSize(int.MaxValue, int.MaxValue);

			_thumbnailGenerationLockObject = new object();
        }

        public string FileName { get; }

        public ImageSize ImageSize { get; private set; }

        public ImageSource GetImage()
        {
            Image image = null;
            var imageSource = GlobalData.InvalidImage;

            try
            {
                image = new Bitmap(_imageFilePath);

				SetImageSize(image);

				imageSource = image.ConvertToImageSource();
            }
            catch
            {
            }
            finally
            {
                image?.Dispose();
            }

            return imageSource;
        }

        public ImageSource GetResizedImage(ImageSize viewPortSize)
        {
            Image image = null;
            Image resizedImage = null;
            var resizedImageSource = GlobalData.InvalidImage;

            try
            {
                image = new Bitmap(_imageFilePath);

				SetImageSize(image);

				resizedImage = _imageResizer.CreateResizedImage(image, viewPortSize);
                resizedImageSource = resizedImage.ConvertToImageSource();
            }
            catch
            {
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

					SetImageSize(_imageData);
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
                if (_imageData == null)
                {
                    throw new InvalidOperationException(
                        $"The method {nameof(ReadImageDataFromDisc)} must be executed prior to calling the method {nameof(GetThumbnail)}.");
                }

                Image thumbnail = null;
                var thumbnailSource = GlobalData.InvalidImageThumbnail;

                try
                {
                    thumbnail = _imageResizer.CreateResizedImage(_imageData, GlobalData.ThumbnailSize);
                    thumbnailSource = thumbnail.ConvertToImageSource();
                }
                catch
                {
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

        private Image _imageData;

		private void SetImageSize(Image image)
		{
			ImageSize = new ImageSize(image.Width, image.Height);
		}

		#endregion
	}
}
