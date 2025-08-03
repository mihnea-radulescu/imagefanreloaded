using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Factories;
using ImageFanReloaded.Core.Settings;
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

		_folderChangedMutex = _contentTabItem.FolderChangedMutex!;
		_ctsThumbnailGeneration = new CancellationTokenSource();
	}

	public void NotifyStopThumbnailGeneration() => _ctsThumbnailGeneration.Cancel();

	public async Task ClearVisualState() => await _contentTabItem.ClearThumbnailBoxes(false);

	public async Task UpdateVisualState(ITabOptions tabOptions)
	{
		try
		{
			await _folderChangedMutex.Wait();

			await _contentTabItem.ClearThumbnailBoxes(true);
			_contentTabItem.SetTabInfo(_folderName, _folderPath);

			var subFolders = await _discQueryEngine.GetSubFolders(
				_folderPath,
				tabOptions.FolderOrdering,
				tabOptions.FolderOrderingDirection);
			_contentTabItem.PopulateSubFoldersTree(subFolders);

			var imageFiles = await _discQueryEngine.GetImageFiles(
				_folderPath,
				tabOptions.ImageFileOrdering,
				tabOptions.ImageFileOrderingDirection,
				tabOptions.RecursiveFolderBrowsing);

			_imageFilesCount = imageFiles.Count;
			_imageFilesTotalSizeOnDiscInKilobytes =
				GetImageFilesTotalSizeOnDiscInKilobytes(imageFiles);

			SetFolderInfoText(tabOptions);
			_contentTabItem.SetImageInfoText(string.Empty);

			var thumbnails = GetThumbnailInfoCollection(tabOptions, imageFiles);

			await ProcessThumbnails(thumbnails);
		}
		finally
		{
			_folderChangedMutex.Signal();
		}
	}

	public void UpdateFolderInfoText(
		ITabOptions tabOptions,
		decimal previousSelectedImageSizeOnDiscInKilobytes,
		decimal currentSelectedImageSizeOnDiscInKilobytes)
	{
		_imageFilesTotalSizeOnDiscInKilobytes +=
			currentSelectedImageSizeOnDiscInKilobytes - previousSelectedImageSizeOnDiscInKilobytes;

		SetFolderInfoText(tabOptions);
	}

	public void DisposeCancellationTokenSource() => _ctsThumbnailGeneration.Dispose();

	#region Private

	private readonly IGlobalParameters _globalParameters;
	private readonly IFileSizeEngine _fileSizeEngine;
	private readonly IThumbnailInfoFactory _thumbnailInfoFactory;
	private readonly IDiscQueryEngine _discQueryEngine;

	private readonly IContentTabItem _contentTabItem;

	private readonly string _folderName;
	private readonly string _folderPath;

	private readonly IAsyncMutex _folderChangedMutex;
	private readonly CancellationTokenSource _ctsThumbnailGeneration;

	private int _imageFilesCount;
	private decimal _imageFilesTotalSizeOnDiscInKilobytes;

	private IReadOnlyList<IThumbnailInfo> GetThumbnailInfoCollection(
		ITabOptions tabOptions, IReadOnlyList<IImageFile> imageFiles)
		=> imageFiles
			.Select(anImageFile => _thumbnailInfoFactory.GetThumbnailInfo(tabOptions, anImageFile))
			.ToList();

	private async Task ProcessThumbnails(IReadOnlyList<IThumbnailInfo> thumbnails)
	{
		for (var thumbnailCollection = (IEnumerable<IThumbnailInfo>)thumbnails;
			 !_ctsThumbnailGeneration.IsCancellationRequested && thumbnailCollection.Any();
			 thumbnailCollection = thumbnailCollection.Skip(_globalParameters.ProcessorCount))
		{
			if (_ctsThumbnailGeneration.IsCancellationRequested)
			{
				return;
			}

			var currentThumbnails = thumbnailCollection
				.Take(_globalParameters.ProcessorCount)
				.ToList();

			if (_ctsThumbnailGeneration.IsCancellationRequested)
			{
				return;
			}

			await ReadThumbnailInput(currentThumbnails);

			if (_ctsThumbnailGeneration.IsCancellationRequested)
			{
				return;
			}

			_contentTabItem.PopulateThumbnailBoxes(currentThumbnails);

			await GetThumbnails(currentThumbnails);

			if (_ctsThumbnailGeneration.IsCancellationRequested)
			{
				return;
			}

			_contentTabItem.RefreshThumbnailBoxes(currentThumbnails);

			if (_ctsThumbnailGeneration.IsCancellationRequested)
			{
				return;
			}
		}
	}

	private async Task ReadThumbnailInput(IReadOnlyList<IThumbnailInfo> currentThumbnails)
		=> await Task.Run(() =>
			{
				for (var i = 0;
					 !_ctsThumbnailGeneration.IsCancellationRequested && i < currentThumbnails.Count;
					 i++)
				{
					currentThumbnails[i].ReadThumbnailInputFromDisc();
				}
			});

	private async Task GetThumbnails(IReadOnlyList<IThumbnailInfo> currentThumbnails)
		=> await Task.Run(() =>
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
					if (!_ctsThumbnailGeneration.IsCancellationRequested)
					{
						aThumbnailGenerationTask.Start();
					}
				}

				try
				{
					Task.WaitAll(thumbnailGenerationTasks, _ctsThumbnailGeneration.Token);
				}
				catch
				{
				}
			});

	private static decimal GetImageFilesTotalSizeOnDiscInKilobytes(
		IReadOnlyList<IImageFile> imageFiles)
	{
		var imageFilesTotalSizeOnDiscInKilobytes = imageFiles
			.Sum(anImageFile =>
				anImageFile.TransientImageFileData.SizeOnDiscInKilobytes.GetValueOrDefault());

		return imageFilesTotalSizeOnDiscInKilobytes;
	}

	private void SetFolderInfoText(ITabOptions tabOptions)
	{
		var imageFilesTotalSizeOnDiscInMegabytes = _fileSizeEngine.ConvertToMegabytes(
			_imageFilesTotalSizeOnDiscInKilobytes);

		var folderStatusBarText = GetFolderStatusBarText(
			_imageFilesCount,
			imageFilesTotalSizeOnDiscInMegabytes,
			tabOptions.RecursiveFolderBrowsing);

		_contentTabItem.SetFolderInfoText(folderStatusBarText);
	}

	private string GetFolderStatusBarText(
		int imageFilesCount,
		decimal imageFilesTotalSizeOnDiscInMegabytes,
		bool recursiveFolderAccess)
	{
		var imageFilesTotalSizeOnDiscInMegabytesForDisplay = decimal.Round(
			imageFilesTotalSizeOnDiscInMegabytes, _globalParameters.DecimalDigitCountForDisplay);

		var imageFilesCountAndTotalSizeText = imageFilesCount switch
		{
			0 => "no images",
			1 => $"1 image - {imageFilesTotalSizeOnDiscInMegabytesForDisplay} MB",
			_ => $"{imageFilesCount} images - {imageFilesTotalSizeOnDiscInMegabytesForDisplay} MB"
		};

		var recursiveFolderAccessInfo = recursiveFolderAccess
			? " (recursive)"
			: string.Empty;

		var folderStatusBarText =
			$"{_folderPath}{recursiveFolderAccessInfo} - {imageFilesCountAndTotalSizeText}";

		return folderStatusBarText;
	}

	#endregion
}
