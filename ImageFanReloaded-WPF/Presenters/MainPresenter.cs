using System;
using ImageFanReloaded.CommonTypes.CommonEventArgs;
using ImageFanReloaded.CommonTypes.Disc;
using ImageFanReloaded.Factories;
using ImageFanReloaded.Infrastructure;
using ImageFanReloaded.Views;

namespace ImageFanReloaded.Presenters
{
    public class MainPresenter
    {
        public MainPresenter(
            IDiscQueryEngine discQueryEngine,
            IMainView mainView,
            IVisualActionDispatcher dispatcher,
            IFolderVisualStateFactory folderVisualStateFactory)
        {
            _discQueryEngine = discQueryEngine ?? throw new ArgumentNullException(nameof(discQueryEngine));

            _mainView = mainView ?? throw new ArgumentNullException(nameof(mainView));
            _mainView.FolderChanged += OnFolderChanged;

            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

            _folderVisualStateFactory = folderVisualStateFactory;

            _generateThumbnailsLockObject = new object();

            PopulateDrivesAndSpecialFolders();
        }

        #region Private

        private readonly IDiscQueryEngine _discQueryEngine;
        private readonly IMainView _mainView;
        private readonly IVisualActionDispatcher _dispatcher;
        private readonly IFolderVisualStateFactory _folderVisualStateFactory;

        private IFolderVisualState _folderVisualState;
        private object _generateThumbnailsLockObject;

        private void PopulateDrivesAndSpecialFolders()
        {
            var specialFolders = _discQueryEngine.GetSpecialFolders();
            _mainView.PopulateSubFoldersTree(specialFolders, true);

            var drives = _discQueryEngine.GetAllDrives();
            _mainView.PopulateSubFoldersTree(drives, true);
        }

        private void OnFolderChanged(object sender, FolderChangedEventArgs e)
        {
            UpdateUserInterface(e.Path);
        }

        private void UpdateUserInterface(string folderPath)
        {
            if (_folderVisualState != null)
            {
                _folderVisualState.NotifyStopThumbnailGeneration();
            }

            _folderVisualState = _folderVisualStateFactory.GetFolderVisualState(
                _discQueryEngine,
                _mainView,
                _dispatcher,
                _generateThumbnailsLockObject,
                folderPath);

            _folderVisualState.UpdateVisualState();
        }

        #endregion
    }
}
