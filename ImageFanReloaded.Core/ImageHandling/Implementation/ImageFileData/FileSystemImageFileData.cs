using System;
using System.IO;
using ImageFanReloaded.Core.DiscAccess.Implementation.Extensions;

namespace ImageFanReloaded.Core.ImageHandling.Implementation.ImageFileData;

public class FileSystemImageFileData : ImageFileDataBase
{
	public FileSystemImageFileData(
		string fileName,
		string filePath,
		string fileExtension,
		string fileNameWithoutExtension,
		int fileSizeInBytes,
		DateTime fileLastModificationTime,
		string folderPath)
			: base(
				fileName,
				filePath,
				fileExtension,
				fileNameWithoutExtension,
				fileSizeInBytes,
				fileLastModificationTime,
				folderPath)
	{
	}

	public override ImageData GetImageData()
	{
		if (!File.Exists(FilePath))
		{
			return ImageData.Null;
		}

		try
		{
			var imageFileContent = File.ReadAllBytes(FilePath);

			var imageFileContentStream = new MemoryStream(imageFileContent);
			imageFileContentStream.Reset();

			return new ImageData(imageFileContentStream);
		}
		catch
		{
			return ImageData.Null;
		}
	}
}
