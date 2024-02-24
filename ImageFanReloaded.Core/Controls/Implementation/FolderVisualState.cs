using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.Global;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Core.Controls.Implementation;

public class FolderVisualState : IFolderVisualState
{
	public FolderVisualState(
		IGlobalParameters globalParameters,
		IThumbnailInfoFactory thumbnailInfoFactory,
		IDiscQueryEngine discQueryEngine,
		IDispatcher dispatcher,
		IContentTabItem contentTabItem,
		string folderName,
		string folderPath)
	{
		_globalParameters = globalParameters;
		_thumbnailInfoFactory = thumbnailInfoFactory;
		_discQueryEngine = discQueryEngine;
		_dispatcher = dispatcher;

		_contentTabItem = contentTabItem;
		_generateThumbnailsLockObject = _contentTabItem.GenerateThumbnailsLockObject!;

		_folderName = folderName;
		_folderPath = folderPath;

		_thumbnailGeneration = new CancellationTokenSource();
	}

	public void NotifyStopThumbnailGeneration() => _thumbnailGeneration.Cancel();

	public void UpdateVisualState()
	{
		Task.Factory.StartNew(UpdateVisualStateHelper);
	}

	public void ClearVisualState()
	{
		Task.Factory.StartNew(ClearVisualStateHelper);
	}

	#region Private
	
	private const int OneMegabyteInKilobytes = 1024;

	private readonly IGlobalParameters _globalParameters;
	private readonly IThumbnailInfoFactory _thumbnailInfoFactory;
	private readonly IDiscQueryEngine _discQueryEngine;
	private readonly IDispatcher _dispatcher;
	private readonly IContentTabItem _contentTabItem;

	private readonly string _folderName;
	private readonly string _folderPath;

	private readonly object _generateThumbnailsLockObject;
	private readonly CancellationTokenSource _thumbnailGeneration;

	private void UpdateVisualStateHelper()
	{
		lock (_generateThumbnailsLockObject)
		{
			_dispatcher.Invoke(() => _contentTabItem.ClearThumbnailBoxes(true));
			_dispatcher.Invoke(() => _contentTabItem.SetTitle(_folderName));

			var subFolders = _discQueryEngine.GetSubFolders(_folderPath);
			_dispatcher.Invoke(() => _contentTabItem.PopulateSubFoldersTree(subFolders, false));

			var imageFiles = _discQueryEngine.GetImageFiles(_folderPath);
			var imageFilesCount = imageFiles.Count;
			var imageFilesTotalSizeOnDiscInMegabytes = GetImageFilesTotalSizeOnDiscInMegabytes(
				imageFiles);
			
			var folderStatusBarText =
				$"{_folderPath} - {imageFilesCount} images - {imageFilesTotalSizeOnDiscInMegabytes} MB";
			_dispatcher.Invoke(() => _contentTabItem.SetFolderStatusBarText(folderStatusBarText));
			_dispatcher.Invoke(() => _contentTabItem.SetImageStatusBarText(string.Empty));
			
			var thumbnails = imageFiles
				.Select(anImageFile => _thumbnailInfoFactory.GetThumbnailInfo(anImageFile))
				.ToList();

			for (var thumbnailCollection = (IEnumerable<IThumbnailInfo>)thumbnails;
				 !_thumbnailGeneration.IsCancellationRequested && thumbnailCollection.Any();
				 thumbnailCollection = thumbnailCollection.Skip(_globalParameters.ProcessorCount))
			{
				var currentThumbnails = thumbnailCollection
					.Take(_globalParameters.ProcessorCount)
					.ToArray();

				ReadThumbnailInput(currentThumbnails);
				_dispatcher.Invoke(() => _contentTabItem.PopulateThumbnailBoxes(currentThumbnails));

				GetThumbnails(currentThumbnails);
				_dispatcher.Invoke(() => _contentTabItem.RefreshThumbnailBoxes(currentThumbnails));
			}
		}
	}

	private void ClearVisualStateHelper()
	{
		lock (_generateThumbnailsLockObject)
		{
			_dispatcher.Invoke(() => _contentTabItem.ClearThumbnailBoxes(false));
		}
	}

	private void ReadThumbnailInput(IReadOnlyList<IThumbnailInfo> currentThumbnails)
	{
		for (var i = 0; !_thumbnailGeneration.IsCancellationRequested && i < currentThumbnails.Count; i++)
		{
			currentThumbnails[i].ReadThumbnailInputFromDisc();
		}
	}

	private void GetThumbnails(IReadOnlyList<IThumbnailInfo> currentThumbnails)
	{
		var thumbnailGenerationTasks = new Task[currentThumbnails.Count];

		for (var i = 0; i < currentThumbnails.Count; i++)
		{
			var currentIndex = i;

			var aThumbnailGenerationTask = new Task(() =>
				currentThumbnails[currentIndex].GetThumbnail());

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

	private static int GetImageFilesTotalSizeOnDiscInMegabytes(
		IReadOnlyCollection<IImageFile> imageFiles)
	{
		var imageFilesTotalSizeOnDiscInKilobytes = imageFiles
			.Sum(anImageFile => anImageFile.SizeOnDiscInKilobytes);

		var imageFilesTotalSizeOnDiscInMegabytes = ConvertToSizeOnDiscInMegabytes(
			imageFilesTotalSizeOnDiscInKilobytes);

		return imageFilesTotalSizeOnDiscInMegabytes;
	}
	
	private static int ConvertToSizeOnDiscInMegabytes(int sizeOnDiscInKilobytes)
		=> sizeOnDiscInKilobytes / OneMegabyteInKilobytes;

	#endregion
}
