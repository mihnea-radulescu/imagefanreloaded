using System;

namespace ImageFanReloaded.Core.ImageHandling.ImageFileData;

public interface IImageFileData
{
	string FileName { get; }
	string FilePath { get; }
	string QualifiedFilePath { get; }

	string FileExtension { get; }
	string FileNameWithoutExtension { get; }

	int FileSizeInBytes { get; set; }
	DateTime FileLastModificationTime { get; set; }

	string ContainerPath { get; }
	string FolderPath { get; }

	ImageData GetImageData();
}
