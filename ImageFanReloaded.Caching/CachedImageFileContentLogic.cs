using System.IO;
using ImageFanReloaded.Core.Caching;
using ImageFanReloaded.Core.DiscAccess.Implementation.Extensions;
using ImageFanReloaded.Core.ImageCore;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.ImageFileData;
using ImageFanReloaded.Core.ImageHandling.Implementation;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Caching;

public class CachedImageFileContentLogic
	: ImageFileContentLogicBase, IImageFileContentLogic
{
	public CachedImageFileContentLogic(
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

	public override ImageData GetImageData(IImageFileData imageFileData)
		=> _imageFileContentLogic.GetImageData(imageFileData);

	public override ImageData GetImageData(
		IImageFileData imageFileData,
		int thumbnailSize,
		bool applyImageOrientation)
	{
		try
		{
			var normalizedApplyImageOrientation =
				GetNormalizedApplyImageOrientation(
					imageFileData, applyImageOrientation);

			var thumbnailCacheEntry = _databaseLogic
				.GetThumbnailCacheEntry(
					imageFileData,
					thumbnailSize,
					normalizedApplyImageOrientation);

			if (thumbnailCacheEntry is not null)
			{
				var cachedImageDataStream = GetCachedImageDataStream(
					thumbnailCacheEntry.ThumbnailData);

				return new ImageData(cachedImageDataStream, true);
			}

			return _imageFileContentLogic.GetImageData(imageFileData);
		}
		catch
		{
			return _imageFileContentLogic.GetImageData(imageFileData);
		}
	}

	public override void UpdateThumbnail(
		IImageFileData imageFileData,
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
				FilePath = imageFileData.QualifiedFilePath,
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

	private static Stream GetCachedImageDataStream(byte[] cachedImageData)
	{
		var cachedImageDataStream = new MemoryStream(cachedImageData);
		cachedImageDataStream.Reset();

		return cachedImageDataStream;
	}
}
