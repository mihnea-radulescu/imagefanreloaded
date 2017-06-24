using ImageFanReloaded.CommonTypes.Disc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace ImageFanReloadedTest.Tests.CommonTypes.Disc
{
    [TestClass]
    public class DiscQueryEngineTest
    {
        [TestMethod, TestCategory("Functional")]
        public void Test_GetAllDrives_ReturnsNonEmptyDrivesCollection()
        {
            var allDrives = new DiscQueryEngine().GetAllDrives();

            Assert.IsTrue(allDrives.Any());
        }

        [TestMethod, TestCategory("Functional")]
        public void Test_GetSpecialFolders_ReturnsNonNullCollection()
        {
            var specialFolders = new DiscQueryEngine().GetSpecialFolders();

            Assert.IsNotNull(specialFolders);
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_GetSubFolders_EmptyFolderPath_Throws()
        {
            var testFolderPath = string.Empty;

            var subFolders = new DiscQueryEngine().GetSubFolders(testFolderPath);
        }

        [TestMethod, TestCategory("Functional")]
        public void Test_GetSubFolders_InvalidNonEmptyFolderPath_ReturnsEmptySubFolderCollection()
        {
            var testFolderPath = @"zzzzz";

            var subFolders = new DiscQueryEngine().GetSubFolders(testFolderPath);

            Assert.IsFalse(subFolders.Any());
        }

        [TestMethod, TestCategory("Functional")]
        public void Test_GetSubFolders_ValidFolderPath_ReturnsExpectedSubFolderCollection()
        {
            var testFolderPath = @"TestData\TestFolder";

            var testSubFolders = new DiscQueryEngine().GetSubFolders(testFolderPath);

            Assert.AreEqual(2, testSubFolders.Count());
            Assert.IsTrue(testSubFolders.Any(aSubFolder =>
                                             aSubFolder.Name == "TestSubFolder1"));
            Assert.IsTrue(testSubFolders.Any(aSubFolder =>
                                             aSubFolder.Name == "TestSubFolder2"));
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_GetImageFiles_EmptyFolderPath_Throws()
        {
            var testFolderPath = string.Empty;

            var imageFiles = new DiscQueryEngine().GetImageFiles(testFolderPath);
        }

        [TestMethod, TestCategory("Functional")]
        public void Test_GetImageFiles_InvalidNonEmptyFolderPath_ReturnsEmptyImageFileCollection()
        {
            var testFolderPath = @"zzzzz";

            var imageFiles = new DiscQueryEngine().GetImageFiles(testFolderPath);

            Assert.IsFalse(imageFiles.Any());
        }

        [TestMethod, TestCategory("Functional")]
        public void Test_GetImageFiles_ValidFolderPath_ReturnsExpectedImageFileCollection()
        {
            var testFolderPath = @"TestData";

            var testImages = new DiscQueryEngine().GetImageFiles(testFolderPath);

            Assert.AreEqual(1, testImages.Count());
            Assert.IsTrue(testImages.Any(anImage =>
                                         anImage.FileName == "TestImage.jpg"));
        }
    }
}
