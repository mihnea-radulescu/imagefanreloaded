using ImageFanReloaded.Core.Caching;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Caching;

public class CachedWriteImageFileContentLogic : IImageFileContentLogic
{
	public CachedWriteImageFileContentLogic(
		IImageFileContentLogic imageFileContentLogic,
		IImageDataExtractor imageDataExtractor,
		IDatabaseLogic databaseLogic)
	{
		_imageFileContentLogic = imageFileContentLogic;
		_imageDataExtractor = imageDataExtractor;

		_databaseLogic = databaseLogic;
		_databaseLogic.CreateDatabaseIfNotExisting();
	}

	public ImageData GetImageData(
		ImageFileData imageFileData, bool applyImageOrientation)
			=> _imageFileContentLogic.GetImageData(
					imageFileData, applyImageOrientation);

	public ImageData GetImageData(
		ImageFileData imageFileData,
		int thumbnailSize,
		bool applyImageOrientation)
		=> _imageFileContentLogic.GetImageData(
				imageFileData, thumbnailSize, applyImageOrientation);

	public void UpdateThumbnail(
		ImageFileData imageFileData,
		int thumbnailSize,
		bool applyImageOrientation,
		IImage thumbnail)
	{
		try
		{
			var thumbnailData = _imageDataExtractor.GetImageData(thumbnail);

			var thumbnailCacheEntry = new ThumbnailCacheEntry
			{
				FilePath = imageFileData.FilePath,
				FileSizeInBytes = imageFileData.FileSizeInBytes,
				FileLastModificationTime =
					imageFileData.FileLastModificationTime,

				ThumbnailSize = thumbnailSize,
				ApplyImageOrientation = applyImageOrientation,

				ThumbnailData = thumbnailData
			};

			_databaseLogic.UpsertThumbnailCacheEntry(thumbnailCacheEntry);
		}
		catch
		{
		}
	}

	private readonly IImageFileContentLogic _imageFileContentLogic;
	private readonly IImageDataExtractor _imageDataExtractor;

	private readonly IDatabaseLogic _databaseLogic;
}
