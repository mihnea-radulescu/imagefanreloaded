using ImageFanReloaded.CommonTypes.ImageHandling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;

namespace ImageFanReloadedTest.Tests.CommonTypes.ImageHandling
{
    [TestClass]
    public class BitmapExtensionsTest
    {
        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(NullReferenceException))]
        public void Test_ConvertToImageSource_NullImage_Throws()
        {
            Image image = null;
            
            BitmapExtensions.ConvertToImageSource(image);
        }

        [TestMethod, TestCategory("Functional")]
        public void Test_ConvertToImageSource_ValidImage_ReturnsNonNullImageSource()
        {
            var testImagePath = @"TestData\TestImage.jpg";
            using (var testImage = new Bitmap(testImagePath))
            {
                var testImageSource = BitmapExtensions.ConvertToImageSource(testImage);

                Assert.IsNotNull(testImageSource);
            }
        }
    }
}
