using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Media;

using ImageFanReloaded.CommonTypes.ImageHandling.Interface;

namespace ImageFanReloaded.CommonTypes.ImageHandling
{
    [DebuggerDisplay("{_fileNameWithPath}")]
    public class ImageFile
        : IImageFile
    {
        public ImageFile(IImageResizer imageResizer, string imageFilePath)
        {
            _imageResizer = imageResizer ?? throw new ArgumentNullException(nameof(imageResizer));

            if (string.IsNullOrEmpty(imageFilePath))
            {
                throw new ArgumentException("Image file path cannot be empty.", nameof(imageFilePath));
            }

            _imageFilePath = imageFilePath;
            FileName = Path.GetFileName(imageFilePath);

            _thumbnailGenerationLockObject = new object();
        }

        public string FileName { get; private set; }

        public ImageSource GetImage()
        {
            Image image = null;
            var imageSource = GlobalData.InvalidImage;

            try
            {
                image = new Bitmap(_imageFilePath);
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

        public ImageSource GetResizedImage(Rectangle imageSize)
        {
            if (imageSize.Width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(imageSize.Width),
                                                      "The width cannot be non-positive.");
            }

            if (imageSize.Height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(imageSize.Height),
                                                      "The height cannot be non-positive.");
            }

            Image image = null;
            Image resizedImage = null;
            var resizedImageSource = GlobalData.InvalidImage;

            try
            {
                image = new Bitmap(_imageFilePath);
                resizedImage = _imageResizer.CreateResizedImage(image, imageSize);
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

        public void ReadThumbnailInputFromDisc()
        {
            lock (_thumbnailGenerationLockObject)
            {
                try
                {
                    _thumbnailInput = new Bitmap(_imageFilePath);
                }
                catch
                {
                    _thumbnailInput = GlobalData.InvalidImageAsBitmap;
                }
            }
        }

        public ImageSource GetThumbnail()
        {
            lock (_thumbnailGenerationLockObject)
            {
                if (_thumbnailInput == null)
                {
                    throw new InvalidOperationException(
                        "The method ReadThumbnailInputFromDisc() must be executed prior to calling the method GetThumbnail().");
                }

                Image thumbnail = null;
                var thumbnailSource = GlobalData.InvalidImageThumbnail;

                try
                {
                    thumbnail = _imageResizer.CreateThumbnail(_thumbnailInput, GlobalData.ThumbnailSize);
                    thumbnailSource = thumbnail.ConvertToImageSource();
                }
                catch
                {
                }
                finally
                {
                    thumbnail?.Dispose();

                    DisposeThumbnailInput();
                }

                return thumbnailSource;
            }
        }

        public void DisposeThumbnailInput()
        {
            if (_thumbnailInput != null &&
                _thumbnailInput != GlobalData.InvalidImageAsBitmap)
            {
                _thumbnailInput.Dispose();
            }
        }

        #region Private

        private readonly IImageResizer _imageResizer;
        private readonly string _imageFilePath;
        private readonly object _thumbnailGenerationLockObject;

        private Image _thumbnailInput;

        #endregion
    }
}
