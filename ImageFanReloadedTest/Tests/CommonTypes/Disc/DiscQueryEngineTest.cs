using ImageFanReloaded.CommonTypes.Disc;
using ImageFanReloaded.Factories;
using ImageFanReloadedTest.Factories;
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
            var allDrives = DiscQueryEngine.Instance.GetAllDrives();

            Assert.IsTrue(allDrives.Any());
        }

        [TestMethod, TestCategory("Functional")]
        public void Test_GetSpecialFolders_ReturnsNonNullCollection()
        {
            var specialFolders = DiscQueryEngine.Instance.GetSpecialFolders();

            Assert.IsNotNull(specialFolders);
        }

        [TestMethod, TestCategory("Unit")]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_GetSubFolders_EmptyFolderPath_Throws()
        {
            var testFolderPath = string.Empty;

            var subFolders = DiscQueryEngine.Instance.GetSubFolders(testFolderPath);
        }

        [TestMethod, TestCategory("Functional")]
        public void Test_GetSubFolders_InvalidNonEmptyFolderPath_ReturnsEmptySubFolderCollection()
        {
            var testFolderPath = @"zzzzz";

            var subFolders = DiscQueryEngine.Instance.GetSubFolders(testFolderPath);

            Assert.IsFalse(subFolders.Any());
        }

        [TestMethod, TestCategory("Functional")]
        public void Test_GetSubFolders_ValidFolderPath_ReturnsExpectedSubFolderCollection()
        {
            var testFolderPath = @"TestData\TestFolder";

            var testSubFolders = DiscQueryEngine.Instance.GetSubFolders(testFolderPath);

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

            var imageFiles = DiscQueryEngine.Instance.GetImageFiles(testFolderPath);
        }

        [TestMethod, TestCategory("Functional")]
        public void Test_GetImageFiles_InvalidNonEmptyFolderPath_ReturnsEmptyImageFileCollection()
        {
            var testFolderPath = @"zzzzz";

            var imageFiles = DiscQueryEngine.Instance.GetImageFiles(testFolderPath);

            Assert.IsFalse(imageFiles.Any());
        }

        [TestMethod, TestCategory("Functional")]
        public void Test_GetImageFiles_ValidFolderPath_ReturnsExpectedImageFileCollection()
        {
            var testFolderPath = @"TestData";

            var testImages = DiscQueryEngine.Instance.GetImageFiles(testFolderPath);

            Assert.AreEqual(1, testImages.Count());
            Assert.IsTrue(testImages.Any(anImage =>
                                         anImage.FileName == "TestImage.jpg"));
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
