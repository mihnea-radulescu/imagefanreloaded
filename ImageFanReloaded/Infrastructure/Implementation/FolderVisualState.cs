using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImageFanReloaded.CommonTypes.Disc;
using ImageFanReloaded.CommonTypes.Info;
using ImageFanReloaded.Controls;

namespace ImageFanReloaded.Infrastructure.Implementation;

public class FolderVisualState : IFolderVisualState
{
	public FolderVisualState(
		IDiscQueryEngine discQueryEngine,
		IVisualActionDispatcher dispatcher,
		IContentTabItem contentTabItem,
		string folderPath)
	{
		_discQueryEngine = discQueryEngine;
		_dispatcher = dispatcher;

		_contentTabItem = contentTabItem;
		_generateThumbnailsLockObject = _contentTabItem.GenerateThumbnailsLockObject!;

		_folderPath = folderPath;

		_thumbnailGeneration = new CancellationTokenSource();
	}

	public void NotifyStopThumbnailGeneration() => _thumbnailGeneration.Cancel();

	public void UpdateVisualState()
	{
		Task.Factory.StartNew(UpdateVisualStateHelper);
	}

	#region Private

	private readonly IDiscQueryEngine _discQueryEngine;
	private readonly IVisualActionDispatcher _dispatcher;
	private readonly IContentTabItem _contentTabItem;

	private readonly string _folderPath;

	private readonly object _generateThumbnailsLockObject;
	private readonly CancellationTokenSource _thumbnailGeneration;

	private void UpdateVisualStateHelper()
	{
		lock (_generateThumbnailsLockObject)
		{
			_dispatcher.Invoke(() => _contentTabItem.ClearThumbnailBoxes());
			_dispatcher.Invoke(() => _contentTabItem.SetTitle(_folderPath));

			var subFolders = _discQueryEngine.GetSubFolders(_folderPath);
			_dispatcher.Invoke(() => _contentTabItem.PopulateSubFoldersTree(subFolders, false));

			var thumbnails = GetImageFiles(_folderPath);
			
			var statusBarText = $"{_folderPath} - {thumbnails.Count} images";
			_dispatcher.Invoke(() => _contentTabItem.SetStatusBarText(statusBarText));

			for (var thumbnailCollection = (IEnumerable<ThumbnailInfo>)thumbnails;
				 !_thumbnailGeneration.IsCancellationRequested && thumbnailCollection.Any();
				 thumbnailCollection = thumbnailCollection.Skip(GlobalData.ProcessorCount))
			{
				var currentThumbnails = thumbnailCollection
					.Take(GlobalData.ProcessorCount)
					.ToArray();

				ReadThumbnailInput(currentThumbnails);
				_dispatcher.Invoke(() => _contentTabItem.PopulateThumbnailBoxes(currentThumbnails));

				GetThumbnails(currentThumbnails);
				_dispatcher.Invoke(() => _contentTabItem.RefreshThumbnailBoxes(currentThumbnails));
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

		foreach (var aThumbnailGenerationTask in thumbnailGenerationTasks)
		{
			if (!_thumbnailGeneration.IsCancellationRequested)
			{
				aThumbnailGenerationTask.Start();
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
