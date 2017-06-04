using ImageFanReloaded.CommonTypes.CommonEventArgs;
using ImageFanReloaded.CommonTypes.Info;
using ImageFanReloaded.Factories;
using ImageFanReloaded.Presenters;
using ImageFanReloaded.Views.Interface;
using ImageFanReloadedTest.Factories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace ImageFanReloadedTest.Tests.Views
{
    [TestClass]
    public class MainViewInteractionTest
    {
        [TestMethod, TestCategory("Unit"), TestCategory("Mock")]
        public void Test_MainViewInteraction_MainPresenterInitialized_CallsOnlyExpectedMethods()
        {
            _mainViewMock.Verify(mainView =>
                                 mainView.ClearThumbnailBoxes(),
                                 Times.Never());

            _mainViewMock.Verify(mainView =>
                                 mainView.PopulateSubFoldersTree(
                                                It.IsAny<IEnumerable<FileSystemEntryInfo>>(),
                                                true),
                                 Times.Exactly(2));
        }

        [TestMethod, TestCategory("Unit"), TestCategory("Mock")]
        public void Test_MainViewInteraction_MainViewFolderChanged_CallsOnlyExpectedMethods()
        {
            _mainViewMock.Raise(mainView => mainView.FolderChanged -= delegate { },
                                            new FileSystemEntryChangedEventArgs(@"C:\Temp"));

            _mainViewMock.Verify(mainView =>
                                 mainView.ClearThumbnailBoxes(),
                                 Times.Once());

            _mainViewMock.Verify(mainView =>
                                 mainView.PopulateSubFoldersTree(
                                                It.IsAny<IEnumerable<FileSystemEntryInfo>>(),
                                                false),
                                 Times.Once());
        }


        #region TestSetup

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TypesFactoryResolver.TypesFactoryInstance = new TestingTypesFactory();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _mainViewMock = new Mock<IMainView>(MockBehavior.Loose);
            _mainPresenter = new MainPresenter(_mainViewMock.Object);
        }

        #endregion


        #region Private

        private Mock<IMainView> _mainViewMock;
        private MainPresenter _mainPresenter;

        #endregion
    }
}
