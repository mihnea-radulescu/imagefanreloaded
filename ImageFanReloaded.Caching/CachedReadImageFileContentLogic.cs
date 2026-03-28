using System.IO;
using ImageFanReloaded.Core.Caching;
using ImageFanReloaded.Core.DiscAccess.Implementation;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Caching;

public class CachedReadImageFileContentLogic : IImageFileContentLogic
{
	public CachedReadImageFileContentLogic(
		IImageFileContentLogic imageFileContentLogic,
		IDatabaseLogic databaseLogic)
	{
		_imageFileContentLogic = imageFileContentLogic;

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
	{
		try
		{
			var thumbnailCacheEntry = _databaseLogic
				.GetThumbnailCacheEntry(
					imageFileData,
					thumbnailSize,
					applyImageOrientation);

			if (thumbnailCacheEntry is not null)
			{
				var cachedImageDataStream = GetCachedImageDataStream(
					thumbnailCacheEntry.ThumbnailData);

				return new ImageData(cachedImageDataStream, false);
			}

			return _imageFileContentLogic.GetImageData(
				imageFileData, applyImageOrientation);
		}
		catch
		{
			return _imageFileContentLogic.GetImageData(
				imageFileData, applyImageOrientation);
		}
	}

	public void UpdateThumbnail(
		ImageFileData imageFileData,
		int thumbnailSize,
		bool applyImageOrientation,
		IImage thumbnail)
	{
	}

	private readonly IImageFileContentLogic _imageFileContentLogic;

	private readonly IDatabaseLogic _databaseLogic;

	private static Stream GetCachedImageDataStream(byte[] cachedImageData)
	{
		var cachedImageDataStream = new MemoryStream();

		cachedImageDataStream.Write(cachedImageData, 0, cachedImageData.Length);
		cachedImageDataStream.Reset();

		return cachedImageDataStream;
	}
}
