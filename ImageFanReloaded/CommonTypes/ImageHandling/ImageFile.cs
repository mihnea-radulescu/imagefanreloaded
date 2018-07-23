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
        public ImageFile(IImageResizer imageResizer, string filePath)
        {
            _imageResizer = imageResizer ?? throw new ArgumentNullException(nameof(imageResizer));

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("File path cannot be empty.", nameof(filePath));
            }

            GetFileNameAndPath(filePath);

            _thumbnailGenerationLockObject = new object();
        }

        public string FileName { get; private set; }

        public ImageSource Image
        {
            get
            {
                Image imageFromFile = null;

                try
                {
                    imageFromFile = new Bitmap(_fileNameWithPath);

                    return imageFromFile.ConvertToImageSource();
                }
                catch
                {
                    return GlobalData.InvalidImage;
                }
                finally
                {
                    if (imageFromFile != null)
                    {
                        imageFromFile.Dispose();
                    }
                }
            }
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

            Image imageFromFile = null;
            Image resizedImage = null;

            try
            {
                imageFromFile = new Bitmap(_fileNameWithPath);
                resizedImage = _imageResizer.CreateResizedImage(imageFromFile, imageSize);

                return resizedImage.ConvertToImageSource();
            }
            catch
            {
                resizedImage = _imageResizer.CreateResizedImage(GlobalData.InvalidImageAsBitmap, imageSize);
                
                return resizedImage.ConvertToImageSource();
            }
            finally
            {
                if (imageFromFile != null)
                {
                    imageFromFile.Dispose();
                }

                if (resizedImage != null)
                {
                    resizedImage.Dispose();
                }
            }
        }

        public void ReadThumbnailInputFromDisc()
        {
            lock (_thumbnailGenerationLockObject)
            {
                DisposeThumbnailInput();
                
                try
                {
                    _thumbnailInput = new Bitmap(_fileNameWithPath);
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

                try
                {
                    thumbnail = _imageResizer.CreateThumbnail(_thumbnailInput, GlobalData.ThumbnailSize);

                    return thumbnail.ConvertToImageSource();
                }
                catch
                {
                    return GlobalData.InvalidImageThumbnail;
                }
                finally
                {
                    DisposeThumbnailInput();

                    if (thumbnail != null)
                    {
                        thumbnail.Dispose();
                    }
                }
            }
        }

        #region Private

        private readonly IImageResizer _imageResizer;

        private string _fileNameWithPath;
        private Image _thumbnailInput;
        private object _thumbnailGenerationLockObject;

        private void GetFileNameAndPath(string filePath)
        {
            FileName = Path.GetFileName(filePath);

            _fileNameWithPath = filePath;
        }

        private void DisposeThumbnailInput()
        {
            if ((_thumbnailInput != null) &&
                (_thumbnailInput != GlobalData.InvalidImageAsBitmap))
            {
                _thumbnailInput.Dispose();
            }

            _thumbnailInput = null;
        }

        #endregion
    }
}
