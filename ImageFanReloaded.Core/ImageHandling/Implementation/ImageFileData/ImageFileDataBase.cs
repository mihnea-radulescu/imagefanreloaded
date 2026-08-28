using System;
using ImageFanReloaded.Core.ImageHandling.ImageFileData;

namespace ImageFanReloaded.Core.ImageHandling.Implementation.ImageFileData;

public abstract class ImageFileDataBase : IImageFileData
{
	protected ImageFileDataBase(
		string fileName,
		string filePath,
		string fileExtension,
		string fileNameWithoutExtension,
		int fileSizeInBytes,
		DateTime fileLastModificationTime,
		string containerPath)
	{
		FileName = fileName;
		FilePath = filePath;

		FileExtension = fileExtension;
		FileNameWithoutExtension = fileNameWithoutExtension;

		FileSizeInBytes = fileSizeInBytes;
		FileLastModificationTime = fileLastModificationTime;

		ContainerPath = containerPath;
	}

	public string FileName { get; }
	public string FilePath { get; }
	public virtual string QualifiedFilePath => FilePath;

	public string FileExtension { get; }
	public string FileNameWithoutExtension { get; }

	public int FileSizeInBytes { get; set; }
	public DateTime FileLastModificationTime { get; set; }

	public string ContainerPath { get; }
	public virtual string FolderPath => ContainerPath;

	public abstract ImageData GetImageData();
}
