using System;
using System.IO;
using ImageFanReloaded.Core.Caching;
using ImageFanReloaded.Core.DiscAccess.Implementation;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Caching;

public class CachedImageFileContentLogic : IImageFileContentLogic
{
	public CachedImageFileContentLogic(
		IImageFileContentLogic imageFileContentLogic,
		IImageDataExtractor imageDataExtractor,
		IDatabaseLogic databaseLogic)
	{
		_imageFileContentLogic = imageFileContentLogic;
		_imageDataExtractor = imageDataExtractor;

		_databaseLogic = databaseLogic;
	}

	public ImageData GetImageData(
		string imageFilePath, bool applyImageOrientation)
			=> _imageFileContentLogic.GetImageData(
					imageFilePath, applyImageOrientation);

	public ImageData GetImageData(
		string imageFilePath, int thumbnailSize, bool applyImageOrientation)
	{
		try
		{
			var lastModificationTime = GetImageFileLastModificationTime(
				imageFilePath);

			var thumbnailCacheEntry = _databaseLogic
				.GetThumbnailCacheEntry(
					imageFilePath,
					thumbnailSize,
					applyImageOrientation,
					lastModificationTime);

			if (thumbnailCacheEntry is not null)
			{
				var cachedImageDataStream = GetCachedImageDataStream(
					thumbnailCacheEntry.ThumbnailData);

				return new ImageData(cachedImageDataStream, false);
			}

			return _imageFileContentLogic.GetImageData(
				imageFilePath, applyImageOrientation);
		}
		catch
		{
			return _imageFileContentLogic.GetImageData(
				imageFilePath, applyImageOrientation);
		}
	}

	public void UpdateThumbnail(
		string imageFilePath,
		int thumbnailSize,
		bool applyImageOrientation,
		IImage thumbnail)
	{
		try
		{
			var thumbnailData = _imageDataExtractor.GetImageData(thumbnail);

			var lastModificationTime = GetImageFileLastModificationTime(
				imageFilePath);

			var thumbnailCacheEntry = new ThumbnailCacheEntry
			{
				ImageFilePath = imageFilePath,
				ThumbnailSize = thumbnailSize,
				ApplyImageOrientation = applyImageOrientation,
				LastModificationTime = lastModificationTime,
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

	private static DateTime GetImageFileLastModificationTime(
		string imageFilePath)
			=> File.GetLastWriteTimeUtc(imageFilePath);

	private Stream GetCachedImageDataStream(byte[] cachedImageData)
	{
		var cachedImageDataStream = new MemoryStream();

		cachedImageDataStream.Write(cachedImageData, 0, cachedImageData.Length);
		cachedImageDataStream.Reset();

		return cachedImageDataStream;
	}
}
