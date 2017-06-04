using ImageFanReloaded;
using ImageFanReloaded.CommonTypes.ImageHandling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;

namespace ImageFanReloadedTest.Tests.CommonTypes.ImageHandling
{
    [TestClass]
    public class ImageResizerTest
    {
        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_CreateThumbnail_NullImage_Throws()
        {
            Image image = null;
            var thumbnailSize = 25;

            var imageResizer = ImageResizer.Instance.CreateThumbnail(image, thumbnailSize);
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_CreateThumbnail_InvalidThumbnailSize_Throws()
        {
            Image image = GlobalData.InvalidImageAsBitmap;
            var thumbnailSize = 0;

            var imageResizer = ImageResizer.Instance.CreateThumbnail(image, thumbnailSize);
        }

        [TestMethod, TestCategory("Functional")]
        public void Test_CreateThumbnail_ValidInput_ReturnsNonNullThumbnailImage()
        {
            using (var image = new Bitmap(@"TestData\TestImage.jpg"))
            {
                var thumbnailSize = 25;

                using (var thumbnail =
                            ImageResizer.Instance.CreateThumbnail(image, thumbnailSize))
                {
                    Assert.IsNotNull(thumbnail);
                }
            }
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_CreateResizedImage_NullImage_Throws()
        {
            Image image = null;
            var imageSize = new Rectangle(0, 0, 25, 25);

            var imageResizer = ImageResizer.Instance.CreateResizedImage(image, imageSize);
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_CreateResizedImage_InvalidThumbnailSize_Throws()
        {
            Image image = GlobalData.InvalidImageAsBitmap;
            var imageSize = new Rectangle(0, 0, 5, -3);

            var imageResizer = ImageResizer.Instance.CreateResizedImage(image, imageSize);
        }

        [TestMethod, TestCategory("Functional")]
        public void Test_CreateResizedImage_ValidInput_ReturnsNonNullResizedImage()
        {
            using (var image = new Bitmap(@"TestData\TestImage.jpg"))
            {
                var imageSize = new Rectangle(0, 0, 25, 15);

                using (var resizedImage =
                            ImageResizer.Instance.CreateResizedImage(image, imageSize))
                {
                    Assert.IsNotNull(resizedImage);
                }
            }
        }
    }
}
