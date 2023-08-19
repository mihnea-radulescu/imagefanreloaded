using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImageFanReloaded.CommonTypes.Disc;
using ImageFanReloaded.CommonTypes.Info;
using ImageFanReloaded.Views;

namespace ImageFanReloaded.Infrastructure.Implementation
{
	public class FolderVisualState
		: IFolderVisualState
	{
		public FolderVisualState(
			IDiscQueryEngine discQueryEngine,
			IMainView mainView,
			IVisualActionDispatcher dispatcher,
			object generateThumbnailsLockObject,
			string folderPath)
		{
			_discQueryEngine = discQueryEngine;
			_mainView = mainView;
			_dispatcher = dispatcher;
			_generateThumbnailsLockObject = generateThumbnailsLockObject;
			_folderPath = folderPath;

			_thumbnailGeneration = new CancellationTokenSource();
		}

		public void NotifyStopThumbnailGeneration()
			=> _thumbnailGeneration.Cancel();

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

		private readonly CancellationTokenSource _thumbnailGeneration;

		private void UpdateVisualStateHelper()
		{
			lock (_generateThumbnailsLockObject)
			{
				_dispatcher.Invoke(() => _mainView.ClearThumbnailBoxes());

				var subFolders = _discQueryEngine.GetSubFolders(_folderPath);
				_dispatcher.Invoke(() => _mainView.PopulateSubFoldersTree(subFolders, false));

				var thumbnails = GetImageFiles(_folderPath);

				for (var thumbnailCollection = (IEnumerable<ThumbnailInfo>)thumbnails;
					 !_thumbnailGeneration.IsCancellationRequested && thumbnailCollection.Any();
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
				.Select(anImageFile => new ThumbnailInfo(_dispatcher, anImageFile))
				.ToList();

			return thumbnailInfoList;
		}

		private void ReadThumbnailInput(IList<ThumbnailInfo> currentThumbnails)
		{
			for (var i = 0; !_thumbnailGeneration.IsCancellationRequested && i < currentThumbnails.Count; i++)
			{
				currentThumbnails[i].ReadThumbnailInputFromDisc();
			}
		}

		private void GetThumbnails(IList<ThumbnailInfo> currentThumbnails)
		{
			var thumbnailGenerationTasks = new Task[currentThumbnails.Count];

			for (var i = 0; i < currentThumbnails.Count; i++)
			{
				var currentIndex = i;

				var aThumbnailGenerationTask = new Task(() =>
					currentThumbnails[currentIndex].SaveThumbnail());

				thumbnailGenerationTasks[currentIndex] = aThumbnailGenerationTask;
			}

			for (var i = 0; i < thumbnailGenerationTasks.Length; i++)
			{
				if (!_thumbnailGeneration.IsCancellationRequested)
				{
					thumbnailGenerationTasks[i].Start();
				}
			}

			try
			{
				Task.WaitAll(thumbnailGenerationTasks, _thumbnailGeneration.Token);
			}
			catch (OperationCanceledException)
			{
			}
		}

		#endregion
	}
}
