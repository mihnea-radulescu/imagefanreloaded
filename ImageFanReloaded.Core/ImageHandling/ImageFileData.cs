using System;

namespace ImageFanReloaded.Core.ImageHandling;

public record ImageFileData
{
	public ImageFileData(
		string fileName,
		string filePath,
		string fileExtension,
		string fileNameWithoutExtension,
		int fileSizeInBytes,
		DateTime fileLastModificationTime,
		string folderPath)
	{
		FileName = fileName;
		FilePath = filePath;
		FileExtension = fileExtension;
		FileNameWithoutExtension = fileNameWithoutExtension;

		FileSizeInBytes = fileSizeInBytes;
		FileLastModificationTime = fileLastModificationTime;

		FolderPath = folderPath;
	}

	public string FileName { get; }
	public string FilePath { get; }
	public string FileExtension { get; }
	public string FileNameWithoutExtension { get; }

	public int FileSizeInBytes { get; set; }
	public DateTime FileLastModificationTime { get; set; }

	public string FolderPath { get; }
}
