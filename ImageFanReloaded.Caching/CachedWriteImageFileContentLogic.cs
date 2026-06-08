using ImageFanReloaded.Core.Caching;
using ImageFanReloaded.Core.ImageCore;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Implementation;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Caching;

public class CachedWriteImageFileContentLogic
	: ImageFileContentLogicBase, IImageFileContentLogic
{
	public CachedWriteImageFileContentLogic(
		IGlobalParameters globalParameters,
		IImageFileContentLogic imageFileContentLogic,
		IImageDataExtractor imageDataExtractor,
		IDatabaseLogic databaseLogic)
			: base(globalParameters)
	{
		_imageFileContentLogic = imageFileContentLogic;
		_imageDataExtractor = imageDataExtractor;

		_databaseLogic = databaseLogic;
		_databaseLogic.CreateDatabaseIfNotExisting();
	}

	public override ImageData GetImageData(ImageFileData imageFileData)
		=> _imageFileContentLogic.GetImageData(imageFileData);

	public override ImageData GetImageData(
		ImageFileData imageFileData,
		int thumbnailSize,
		bool applyImageOrientation)
			=> _imageFileContentLogic.GetImageData(
					imageFileData, thumbnailSize, applyImageOrientation);

	public override void UpdateThumbnail(
		ImageFileData imageFileData,
		int thumbnailSize,
		bool applyImageOrientation,
		IImage thumbnail)
	{
		try
		{
			var normalizedApplyImageOrientation =
				GetNormalizedApplyImageOrientation(
					imageFileData, applyImageOrientation);

			var thumbnailData = _imageDataExtractor.GetImageData(thumbnail);

			var thumbnailCacheEntry = new ThumbnailCacheEntry
			{
				FilePath = imageFileData.FilePath,
				FileSizeInBytes = imageFileData.FileSizeInBytes,
				FileLastModificationTime =
					imageFileData.FileLastModificationTime,

				ThumbnailSize = thumbnailSize,
				ApplyImageOrientation = normalizedApplyImageOrientation,

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
