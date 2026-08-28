using System;
using System.IO;
using System.IO.Compression;
using ImageFanReloaded.Core.DiscAccess.Implementation.Extensions;

namespace ImageFanReloaded.Core.ImageHandling.Implementation.ImageFileData;

public class ZipArchiveImageFileData : ImageFileDataBase
{
	public ZipArchiveImageFileData(
		string fileName,
		string filePath,
		string fileExtension,
		string fileNameWithoutExtension,
		int fileSizeInBytes,
		DateTime fileLastModificationTime,
		string zipArchivePath)
			: base(
				fileName,
				filePath,
				fileExtension,
				fileNameWithoutExtension,
				fileSizeInBytes,
				fileLastModificationTime,
				zipArchivePath)
	{
		_zipArchivePath = zipArchivePath;
	}

	public override string QualifiedFilePath
		=> $"{_zipArchivePath}{Path.DirectorySeparatorChar}{FilePath}";

	public override string FolderPath => Path.GetDirectoryName(ContainerPath)!;

	public override ImageData GetImageData()
	{
		try
		{
			using var zipArchive = ZipFile.OpenRead(ContainerPath);

			var zipArchiveEntry = zipArchive.GetEntry(FilePath)!;
			using var zipArchiveEntryStream = zipArchiveEntry.Open();

			var imageFileContentStream = new MemoryStream();
			zipArchiveEntryStream.CopyTo(imageFileContentStream);
			imageFileContentStream.Reset();

			return new ImageData(imageFileContentStream);
		}
		catch
		{
			return new ImageData(null);
		}
	}

	private readonly string _zipArchivePath;
}
