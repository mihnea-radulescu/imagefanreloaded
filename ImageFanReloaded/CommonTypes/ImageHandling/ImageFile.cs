using ImageFanReloaded.CommonTypes.ImageHandling.Interface;
using ImageFanReloaded.Factories;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Media;

namespace ImageFanReloaded.CommonTypes.ImageHandling
{
    [DebuggerDisplay("{_fileNameWithPath}")]
    public class ImageFile
        : IImageFile
    {
        public ImageFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException("File path cannot be empty.", "filePath");

            GetFileNameAndPath(filePath);
            _lockObject = new object();
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
                        imageFromFile.Dispose();
                }
            }
        }

        public ImageSource GetResizedImage(Rectangle imageSize)
        {
            if (imageSize.Width <= 0)
                throw new ArgumentOutOfRangeException("imageSize.Width",
                                                      "The width cannot be non-positive.");
            if (imageSize.Height <= 0)
                throw new ArgumentOutOfRangeException("imageSize.Height",
                                                      "The height cannot be non-positive.");
            
            Image imageFromFile = null;
            Image resizedImage = null;

            try
            {
                imageFromFile = new Bitmap(_fileNameWithPath);
                resizedImage = TypesFactoryResolver.TypesFactoryInstance.ImageResizerInstance
                                        .CreateResizedImage(imageFromFile, imageSize);

                return resizedImage.ConvertToImageSource();
            }
            catch
            {
                resizedImage = TypesFactoryResolver.TypesFactoryInstance.ImageResizerInstance
                                        .CreateResizedImage(GlobalData.InvalidImageAsBitmap,
                                                            imageSize);
                
                return resizedImage.ConvertToImageSource();
            }
            finally
            {
                if (imageFromFile != null)
                    imageFromFile.Dispose();
                if (resizedImage != null)
                    resizedImage.Dispose();
            }
        }

        public void ReadThumbnailInputFromDisc()
        {
            lock (_lockObject)
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

        public ImageSource Thumbnail
        {
            get
            {
                lock (_lockObject)
                {
                    if (_thumbnailInput == null)
                        throw new InvalidOperationException(
                            "ReadThumbnailInput() must be executed prior to calling the Thumbnail property.");
                    
                    Image thumbnail = null;

                    try
                    {
                        thumbnail = TypesFactoryResolver.TypesFactoryInstance.ImageResizerInstance
                                        .CreateThumbnail(_thumbnailInput,
                                                         GlobalData.ThumbnailSize);

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
                            thumbnail.Dispose();
                    }
                }
            }
        }


        #region Private

        private string _fileNameWithPath;
        private Image _thumbnailInput;
        private object _lockObject;

        private void GetFileNameAndPath(string filePath)
        {
            FileName = Path.GetFileName(filePath);

            _fileNameWithPath = filePath;
        }

        private void DisposeThumbnailInput()
        {
            if ((_thumbnailInput != null) &&
                (_thumbnailInput != GlobalData.InvalidImageAsBitmap))
                _thumbnailInput.Dispose();

            _thumbnailInput = null;
        }

        #endregion
    }
}
