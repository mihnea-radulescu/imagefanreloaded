using ImageFanReloaded.CommonTypes.CommonEventArgs;
using ImageFanReloaded.CommonTypes.Info;
using ImageFanReloaded.Factories;
using ImageFanReloaded.Views.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ImageFanReloaded.Presenters
{
    public class MainPresenter
    {
        public MainPresenter(IMainView mainView)
        {
            _mainView = mainView ?? throw new ArgumentNullException(nameof(mainView));
            _mainView.FolderChanged += OnFolderChanged;

            _populateThumbnailsLockObject = new object();

            PopulateDrivesAndSpecialFolders();
        }


        #region Private

        private IMainView _mainView;
        private object _populateThumbnailsLockObject;
        private CancellationTokenSource _cancellationTokenSource;

        private void OnFolderChanged(object sender, FileSystemEntryChangedEventArgs e)
        {
            StopPopulateThumbnails();

            PopulateSubFolders(e.Path);

            PopulateThumbnails(e.Path);
        }

        private void PopulateDrivesAndSpecialFolders()
        {
            var specialFolders = TypesFactoryResolver.TypesFactoryInstance
                                            .DiscQueryEngineInstance.GetSpecialFolders();
            _mainView.PopulateSubFoldersTree(specialFolders, true);
            
            var drives = TypesFactoryResolver.TypesFactoryInstance
                                            .DiscQueryEngineInstance.GetAllDrives();
            _mainView.PopulateSubFoldersTree(drives, true);
        }

        private void PopulateSubFolders(string folderPath)
        {
            var subFolders = TypesFactoryResolver.TypesFactoryInstance
                                            .DiscQueryEngineInstance.GetSubFolders(folderPath);
            _mainView.PopulateSubFoldersTree(subFolders, false);
        }

        private void StopPopulateThumbnails()
        {
            try
            {
                _cancellationTokenSource.Cancel();
            }
            catch
            {
            }
        }

        private async void PopulateThumbnails(string folderPath)
        {
            try
            {
                Monitor.Enter(_populateThumbnailsLockObject);

                _mainView.ClearThumbnailBoxes();

                using (_cancellationTokenSource = new CancellationTokenSource())
                {
                    var cancellationToken = _cancellationTokenSource.Token;

                    var getImageFilesTask = Task.Run(() => GetImageFiles(folderPath));
                    var thumbnails = await getImageFilesTask;

                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    for (var thumbnailCollection = thumbnails;
                             thumbnailCollection.Any();
                             thumbnailCollection =
                                        thumbnailCollection.Skip(GlobalData.ProcessorCount))
                    {
                        var currentThumbnails = thumbnailCollection
                            .Take(GlobalData.ProcessorCount)
                            .ToArray();

                        var readThumbnailInputTask = Task.Run(
                            () => ReadThumbnailInput(currentThumbnails, cancellationToken));
                        await readThumbnailInputTask;

                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }

                        _mainView.PopulateThumbnailBoxes(currentThumbnails);

                        var getThumbnailsTask = Task.Run(
                            () => GetThumbnails(currentThumbnails, cancellationToken));
                        await getThumbnailsTask;
                    }
                }
            }
            finally
            {
                Monitor.Exit(_populateThumbnailsLockObject);
            }
        }

        private IEnumerable<ThumbnailInfo> GetImageFiles(string folderPath)
        {
            var imageFiles = TypesFactoryResolver.TypesFactoryInstance
                                                        .DiscQueryEngineInstance
                                                        .GetImageFiles(folderPath);

            var thumbnailInfoCollection =
                imageFiles
                    .Select(anImageFile => new ThumbnailInfo(anImageFile))
                    .ToArray();

            return thumbnailInfoCollection;
        }

        private void ReadThumbnailInput(
            ICollection<ThumbnailInfo> currentThumbnails,
            CancellationToken cancellationToken)
        {
            foreach (var aThumbnail in currentThumbnails)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                aThumbnail.ImageFile.ReadThumbnailInputFromDisc();
            }
        }

        private void GetThumbnails(
            ICollection<ThumbnailInfo> currentThumbnails,
            CancellationToken cancellationToken)
        {
            currentThumbnails
                .AsParallel()
                .AsOrdered()
                .ForAll(aThumbnail =>
                {
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        var currentThumbnail = aThumbnail.ImageFile.Thumbnail;
                        currentThumbnail.Freeze();

                        aThumbnail.ThumbnailImage = currentThumbnail;
                    }
                });
        }

        #endregion
    }
}
