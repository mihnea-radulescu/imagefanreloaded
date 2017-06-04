using ImageFanReloaded;
using ImageFanReloaded.CommonTypes.ImageHandling;
using ImageFanReloaded.Factories;
using ImageFanReloadedTest.Factories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;

namespace ImageFanReloadedTest.Tests.CommonTypes.ImageHandling
{
    [TestClass]
    public class ImageFileTest
    {
        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_Constructor_EmptyImageFilePath_Throws()
        {
            var filePath = string.Empty;
            
            new ImageFile(filePath);
        }

        [TestMethod, TestCategory("Unit")]
        public void Test_FileName_ValidImageFilePath_ReturnsExpectedFileName()
        {
            var filePath = @"TestData\TestImage.jpg";

            var imageFile = new ImageFile(filePath);
            var imageFileName = imageFile.FileName;

            Assert.AreEqual("TestImage.jpg", imageFileName);
        }

        [TestMethod, TestCategory("Functional")]
        public void Test_Image_InvalidImageFile_ReturnsPredefinedInvalidImage()
        {
            var filePath = @"TestData\TestDataFile.txt";

            var imageFile = new ImageFile(filePath);
            var image = imageFile.Image;

            Assert.AreEqual(GlobalData.InvalidImage, image);
        }

        [TestMethod, TestCategory("Functional")]
        public void Test_Image_ValidImageFile_ReturnsNonNullImage()
        {
            var filePath = @"TestData\TestImage.jpg";

            var imageFile = new ImageFile(filePath);
            var image = imageFile.Image;

            Assert.IsNotNull(image);
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_GetResizedImage_ValidImageFile_InvalidRectangle_Throws()
        {
            var filePath = @"TestData\TestImage.jpg";

            var imageFile = new ImageFile(filePath);
            var imageSize = new Rectangle(0, 0, -5, 0);
            var image = imageFile.GetResizedImage(imageSize);
        }

        [TestMethod, TestCategory("Functional")]
        public void Test_GetResizedImage_InvalidImageFile_ValidRectangle_ReturnsNonNullInvalidImage()
        {
            var filePath = @"TestData\TestDataFile.txt";

            var imageFile = new ImageFile(filePath);
            var imageSize = new Rectangle(0, 0, 25, 15);
            var image = imageFile.GetResizedImage(imageSize);

            Assert.IsNotNull(image);
        }

        [TestMethod, TestCategory("Functional")]
        public void Test_GetResizedImage_ValidImageFile_ValidRectangle_ReturnsNonNullImage()
        {
            var filePath = @"TestData\TestImage.jpg";

            var imageFile = new ImageFile(filePath);
            var imageSize = new Rectangle(0, 0, 25, 25);
            var image = imageFile.GetResizedImage(imageSize);

            Assert.IsNotNull(image);
        }

        [TestMethod, TestCategory("Functional")]
        public void Test_ReadThumbnailInputFromDisc_InvalidImageFile_DoesNotThrow()
        {
            var filePath = @"TestData\TestDataFile.txt";

            var imageFile = new ImageFile(filePath);
            imageFile.ReadThumbnailInputFromDisc();
        }

        [TestMethod, TestCategory("Functional")]
        public void Test_ReadThumbnailInputFromDisc_ValidImageFile_DoesNotThrow()
        {
            var filePath = @"TestData\TestImage.jpg";

            var imageFile = new ImageFile(filePath);
            imageFile.ReadThumbnailInputFromDisc();
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_Thumbnail_OutOfOrderMethodCall_Throws()
        {
            var filePath = @"TestData\TestImage.jpg";

            var imageFile = new ImageFile(filePath);
            var thumbnail = imageFile.Thumbnail;
        }

        [TestMethod, TestCategory("Functional")]
        public void Test_Thumbnail_InvalidImageFile_ReturnsNonNullInvalidImageThumbnail()
        {
            var filePath = @"TestData\TestDataFile.txt";

            var imageFile = new ImageFile(filePath);
            imageFile.ReadThumbnailInputFromDisc();
            var thumbnail = imageFile.Thumbnail;

            Assert.IsNotNull(thumbnail);
        }

        [TestMethod, TestCategory("Functional")]
        public void Test_Thumbnail_ValidImageFile_ReturnsNonNullImageThumbnail()
        {
            var filePath = @"TestData\TestImage.jpg";

            var imageFile = new ImageFile(filePath);
            imageFile.ReadThumbnailInputFromDisc();
            var thumbnail = imageFile.Thumbnail;

            Assert.IsNotNull(thumbnail);
        }


        #region TestSetup

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TypesFactoryResolver.TypesFactoryInstance = new TestingTypesFactory();
        }

        #endregion
    }
}
