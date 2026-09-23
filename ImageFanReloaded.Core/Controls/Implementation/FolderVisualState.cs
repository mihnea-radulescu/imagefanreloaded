using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.Core.DiscAccess.EntryInfo;
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
		IFileSystemEntryInfo fileSystemEntryInfo)
	{
		_globalParameters = globalParameters;
		_fileSizeEngine = fileSizeEngine;
		_thumbnailInfoFactory = thumbnailInfoFactory;
		_discQueryEngine = discQueryEngine;

		_contentTabItem = contentTabItem;
		_fileSystemEntryInfo = fileSystemEntryInfo;

		_folderChangedMutex = _contentTabItem.FolderChangedMutex!;
		_ctsThumbnailGeneration = new CancellationTokenSource();
	}

	public void NotifyStopThumbnailGeneration()
		=> _ctsThumbnailGeneration.Cancel();

	public async Task ClearVisualState()
		=> await _contentTabItem.ClearThumbnailBoxes(false);

	public async Task UpdateVisualState(ITabOptions tabOptions)
	{
		try
		{
			await _folderChangedMutex.Wait();

			await _contentTabItem.ClearThumbnailBoxes(true);
			_contentTabItem.SetTabInfo(
				_fileSystemEntryInfo.Name, _fileSystemEntryInfo.Path);

			var subFolders = await _discQueryEngine.GetSubFolders(
				_fileSystemEntryInfo, tabOptions);
			_contentTabItem.PopulateSubFoldersTree(subFolders);

			var imageFiles = await _discQueryEngine.GetImageFiles(
				_fileSystemEntryInfo, tabOptions);

			_imageFilesCount = imageFiles.Count;

			SetFolderInfoText(tabOptions);
			_contentTabItem.SetImageInfoText(string.Empty);

			var thumbnails = GetThumbnailInfoList(tabOptions, imageFiles);

			await ProcessThumbnails(thumbnails);
		}
		finally
		{
			_folderChangedMutex.Signal();
		}
	}

	public void UpdateFolderImageFilesTotalSize(
		ITabOptions tabOptions,
		int previousSelectedImageFileSizeInBytes,
		int currentSelectedImageFileSizeInBytes)
	{
		_fileSystemEntryInfo.ImageFilesTotalSizeInBytes -=
			previousSelectedImageFileSizeInBytes;
		_fileSystemEntryInfo.ImageFilesTotalSizeInBytes +=
			currentSelectedImageFileSizeInBytes;

		SetFolderInfoText(tabOptions);
	}

	public void SetFolderInfoText(ITabOptions tabOptions)
	{
		var imageFilesTotalSizeInBytes =
			_fileSystemEntryInfo.ImageFilesTotalSizeInBytes;

		var imageFilesTotalSizeInKilobytes =
			_fileSizeEngine.ConvertToKilobytes(imageFilesTotalSizeInBytes);
		var imageFilesTotalSizeInMegabytes =
			_fileSizeEngine.ConvertToMegabytes(imageFilesTotalSizeInKilobytes);

		var folderStatusBarText = GetFolderStatusBarText(
			_imageFilesCount,
			imageFilesTotalSizeInMegabytes,
			tabOptions.RecursiveFolderBrowsing);

		_contentTabItem.SetFolderInfoText(folderStatusBarText);
	}

	public void DisposeCancellationTokenSource()
		=> _ctsThumbnailGeneration.Dispose();

	private readonly IGlobalParameters _globalParameters;
	private readonly IFileSizeEngine _fileSizeEngine;
	private readonly IThumbnailInfoFactory _thumbnailInfoFactory;
	private readonly IDiscQueryEngine _discQueryEngine;

	private readonly IContentTabItem _contentTabItem;
	private readonly IFileSystemEntryInfo _fileSystemEntryInfo;

	private readonly IAsyncMutex _folderChangedMutex;
	private readonly CancellationTokenSource _ctsThumbnailGeneration;

	private int _imageFilesCount;

	private IReadOnlyList<IThumbnailInfo> GetThumbnailInfoList(
		ITabOptions tabOptions, IReadOnlyList<IImageFile> imageFiles)
			=> imageFiles
				.Select(anImageFile => _thumbnailInfoFactory.GetThumbnailInfo(
							tabOptions, anImageFile))
				.ToList();

	private async Task ProcessThumbnails(
		IReadOnlyList<IThumbnailInfo> thumbnails)
	{
		var hasSetFirstThumbnailStatus = false;

		var thumbnailCollections = thumbnails
			.Chunk(_globalParameters.ProcessorCount)
			.ToArray();

		foreach (var aThumbnailCollection in thumbnailCollections)
		{
			if (_ctsThumbnailGeneration.IsCancellationRequested)
			{
				return;
			}

			await ReadThumbnailInput(aThumbnailCollection);

			if (_ctsThumbnailGeneration.IsCancellationRequested)
			{
				return;
			}

			_contentTabItem.PopulateThumbnailBoxes(aThumbnailCollection);

			await GetThumbnails(aThumbnailCollection);

			if (_ctsThumbnailGeneration.IsCancellationRequested)
			{
				return;
			}

			_contentTabItem.RefreshThumbnailBoxes(aThumbnailCollection);

			if (_ctsThumbnailGeneration.IsCancellationRequested)
			{
				return;
			}

			if (!hasSetFirstThumbnailStatus)
			{
				_contentTabItem.UpdateSelectedImageStatus();

				hasSetFirstThumbnailStatus = true;
			}

			if (_ctsThumbnailGeneration.IsCancellationRequested)
			{
				return;
			}
		}
	}

	private async Task ReadThumbnailInput(
		IReadOnlyList<IThumbnailInfo> aThumbnailCollection)
		=> await Task.Run(() =>
			{
				for (var i = 0;
					 	 !_ctsThumbnailGeneration.IsCancellationRequested &&
					 		i < aThumbnailCollection.Count;
					 	 i++)
				{
					aThumbnailCollection[i].ReadThumbnailInputFromDisc();
				}
			});

	private async Task GetThumbnails(
		IReadOnlyList<IThumbnailInfo> aThumbnailCollection)
		=> await Task.Run(() =>
			{
				var thumbnailGenerationTasks =
					new Task[aThumbnailCollection.Count];

				for (var i = 0; i < aThumbnailCollection.Count; i++)
				{
					var currentIndex = i;

					var aThumbnailGenerationTask =
						new Task(() => aThumbnailCollection[currentIndex]
										.GetThumbnail());

					thumbnailGenerationTasks[currentIndex] =
						aThumbnailGenerationTask;
				}

				foreach (var aThumbnailGenerationTask in
							 thumbnailGenerationTasks)
				{
					if (!_ctsThumbnailGeneration.IsCancellationRequested)
					{
						aThumbnailGenerationTask.Start();
					}
				}

				try
				{
					Task.WaitAll(
						thumbnailGenerationTasks,
						_ctsThumbnailGeneration.Token);
				}
				catch
				{
				}
			});

	private string GetFolderStatusBarText(
		int imageFilesCount,
		decimal imageFilesTotalSizeInMegabytes,
		bool recursiveFolderAccess)
	{
		var imageFilesTotalSizeInMegabytesForDisplay = decimal.Round(
			imageFilesTotalSizeInMegabytes,
			_globalParameters.DecimalDigitCountForDisplay);

		var imageFilesCountAndTotalSizeText = imageFilesCount switch
		{
			0 => "no images",
			1 => $"1 image - {imageFilesTotalSizeInMegabytesForDisplay} MB",
			_ => $"{imageFilesCount} images - {imageFilesTotalSizeInMegabytesForDisplay} MB"
		};

		var recursiveFolderAccessInfo = recursiveFolderAccess
			? " (recursive)"
			: string.Empty;

		var folderStatusBarText =
			$"{_fileSystemEntryInfo.QualifiedPath}{recursiveFolderAccessInfo} - {imageFilesCountAndTotalSizeText}";

		return folderStatusBarText;
	}
}
