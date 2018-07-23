using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ImageFanReloaded.CommonTypes.Disc.Interface;
using ImageFanReloaded.CommonTypes.Info;
using ImageFanReloaded.Infrastructure.Interface;
using ImageFanReloaded.Views.Interface;

namespace ImageFanReloaded.Infrastructure
{
    public class FolderVisualState
    {
        public FolderVisualState(
            IDiscQueryEngine discQueryEngine,
            IMainView mainView,
            IVisualActionDispatcher dispatcher,
            object generateThumbnailsLockObject,
            string folderPath)
        {
            _discQueryEngine = discQueryEngine ?? throw new ArgumentNullException(nameof(discQueryEngine));
            _mainView = mainView ?? throw new ArgumentNullException(nameof(mainView));
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _generateThumbnailsLockObject = generateThumbnailsLockObject
                ?? throw new ArgumentNullException(nameof(generateThumbnailsLockObject));

            _folderPath = folderPath;

            _generateThumbnails = true;
        }

        public void NotifyStopThumbnailGeneration()
            => _generateThumbnails = false;

        public bool ContinueThumbnailGeneration
            => _generateThumbnails;

        public void UpdateVisualState()
        {
            Task.Factory.StartNew(() => UpdateVisualStateHelper());
        }

        #region Private

        private readonly IDiscQueryEngine _discQueryEngine;
        private readonly IMainView _mainView;
        private readonly IVisualActionDispatcher _dispatcher;
        private readonly object _generateThumbnailsLockObject;

        private readonly string _folderPath;

        private volatile bool _generateThumbnails;

        private void UpdateVisualStateHelper()
        {
            lock (_generateThumbnailsLockObject)
            {
                _dispatcher.Invoke(() => _mainView.ClearThumbnailBoxes());

                var subFolders = _discQueryEngine.GetSubFolders(_folderPath);
                _dispatcher.Invoke(() => _mainView.PopulateSubFoldersTree(subFolders, false));

                var thumbnails = GetImageFiles(_folderPath);

                for (var thumbnailCollection = (IEnumerable<ThumbnailInfo>)thumbnails;
                     ContinueThumbnailGeneration && thumbnailCollection.Any();
                     thumbnailCollection = thumbnailCollection.Skip(GlobalData.ProcessorCount))
                {
                    var currentThumbnails = thumbnailCollection
                        .Take(GlobalData.ProcessorCount)
                        .ToArray();

                    ReadThumbnailInput(currentThumbnails);
                    _dispatcher.Invoke(() => _mainView.PopulateThumbnailBoxes(currentThumbnails));

                    GetThumbnails(currentThumbnails);
                    _dispatcher.Invoke(() => _mainView.RefreshThumbnailBoxes(currentThumbnails));
                }
            }
        }

        private IList<ThumbnailInfo> GetImageFiles(string folderPath)
        {
            var imageFiles = _discQueryEngine.GetImageFiles(folderPath);

            var thumbnailInfoList = imageFiles
                .Select(anImageFile => new ThumbnailInfo(anImageFile))
                .ToList();

            return thumbnailInfoList;
        }

        private void ReadThumbnailInput(IList<ThumbnailInfo> currentThumbnails)
        {
            for (var i = 0; ContinueThumbnailGeneration && i < currentThumbnails.Count; i++)
            {
                currentThumbnails[i].ReadThumbnailInputFromDisc();
            }
        }

        private void GetThumbnails(ICollection<ThumbnailInfo> currentThumbnails)
        {
            Parallel.ForEach(currentThumbnails, aThumbnailInfo =>
            {
                if (ContinueThumbnailGeneration)
                {
                    aThumbnailInfo.SaveThumbnail();
                }
            });
        }

        #endregion
    }
}
