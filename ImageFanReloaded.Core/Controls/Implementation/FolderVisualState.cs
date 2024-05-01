using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.Global;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Synchronization;

namespace ImageFanReloaded.Core.Controls.Implementation;

public class FolderVisualState : IFolderVisualState
{
	public FolderVisualState(
		IGlobalParameters globalParameters,
		IFileSizeEngine fileSizeEngine,
		IThumbnailInfoFactory thumbnailInfoFactory,
		IDiscQueryEngine discQueryEngine,
		IContentTabItem contentTabItem,
		string folderName,
		string folderPath)
	{
		_globalParameters = globalParameters;
		_fileSizeEngine = fileSizeEngine;
		_thumbnailInfoFactory = thumbnailInfoFactory;
		_discQueryEngine = discQueryEngine;

		_contentTabItem = contentTabItem;
		
		_folderName = folderName;
		_folderPath = folderPath;
		
		_asyncAutoResetEvent = _contentTabItem.AsyncAutoResetEvent!;
		_thumbnailGeneration = new CancellationTokenSource();
	}

	public void NotifyStopThumbnailGeneration() => _thumbnailGeneration.Cancel();
	
	public void ClearVisualState() => _contentTabItem.ClearThumbnailBoxes(false);

	public async Task UpdateVisualState()
	{
		await _asyncAutoResetEvent.WaitOne();
		
		_contentTabItem.ClearThumbnailBoxes(true);
		_contentTabItem.SetTitle(_folderName);

		var subFolders = await _discQueryEngine.GetSubFolders(_folderPath);
		_contentTabItem.PopulateSubFoldersTree(subFolders, false);

		var imageFiles = await _discQueryEngine.GetImageFiles(_folderPath);
		var imageFilesCount = imageFiles.Count;
		
		var imageFilesTotalSizeOnDiscInMegabytes = await GetImageFilesTotalSizeOnDiscInMegabytes(imageFiles);
			
		var folderStatusBarText =
			$"{_folderPath} - {imageFilesCount} images - {imageFilesTotalSizeOnDiscInMegabytes} MB";
		_contentTabItem.SetFolderStatusBarText(folderStatusBarText);
		_contentTabItem.SetImageStatusBarText(string.Empty);

		var thumbnails = imageFiles
			.Select(anImageFile => _thumbnailInfoFactory.GetThumbnailInfo(anImageFile))
			.ToList();

		await ProcessThumbnails(thumbnails);

		await _asyncAutoResetEvent.Set();
	}

	#region Private

	private readonly IGlobalParameters _globalParameters;
	private readonly IFileSizeEngine _fileSizeEngine;
	private readonly IThumbnailInfoFactory _thumbnailInfoFactory;
	private readonly IDiscQueryEngine _discQueryEngine;
	
	private readonly IContentTabItem _contentTabItem;
	
	private readonly string _folderName;
	private readonly string _folderPath;
	
	private readonly IAsyncAutoResetEvent _asyncAutoResetEvent;
	private readonly CancellationTokenSource _thumbnailGeneration;
	
	private async Task ProcessThumbnails(IReadOnlyList<IThumbnailInfo> thumbnails)
	{
		for (var thumbnailCollection = (IEnumerable<IThumbnailInfo>)thumbnails;
		     !_thumbnailGeneration.IsCancellationRequested && thumbnailCollection.Any();
		     thumbnailCollection = thumbnailCollection.Skip(_globalParameters.ProcessorCount))
		{
			var currentThumbnails = thumbnailCollection
				.Take(_globalParameters.ProcessorCount)
				.ToArray();

			await Task.Run(() => ReadThumbnailInput(currentThumbnails));
			if (!_thumbnailGeneration.IsCancellationRequested)
			{
				_contentTabItem.PopulateThumbnailBoxes(currentThumbnails);
			}
			
			await Task.Run(() => GetThumbnails(currentThumbnails));
			if (!_thumbnailGeneration.IsCancellationRequested)
			{
				_contentTabItem.RefreshThumbnailBoxes(currentThumbnails);
			}
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

	private async Task<int> GetImageFilesTotalSizeOnDiscInMegabytes(
		IReadOnlyCollection<IImageFile> imageFiles)
		=> await Task.Run(() =>
		{
			var imageFilesTotalSizeOnDiscInKilobytes = imageFiles
				.Sum(anImageFile => anImageFile.SizeOnDiscInKilobytes);

			var imageFilesTotalSizeOnDiscInMegabytes = _fileSizeEngine.ConvertToMegabytes(
				imageFilesTotalSizeOnDiscInKilobytes);

			return imageFilesTotalSizeOnDiscInMegabytes;
		});

	#endregion
}
