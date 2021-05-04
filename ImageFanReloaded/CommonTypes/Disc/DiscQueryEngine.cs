using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ImageFanReloaded.CommonTypes.Disc.Interface;
using ImageFanReloaded.CommonTypes.ImageHandling.Interface;
using ImageFanReloaded.CommonTypes.Info;
using ImageFanReloaded.Factories.Interface;

namespace ImageFanReloaded.CommonTypes.Disc
{
    public class DiscQueryEngine
        : IDiscQueryEngine
    {
        public DiscQueryEngine(
            IImageFileFactory imageFileFactory,
            IImageResizer imageResizer,
            IFileSystemEntryComparer fileSystemEntryComparer)
        {
            _imageFileFactory = imageFileFactory ?? throw new ArgumentNullException(nameof(imageFileFactory));
            _imageResizer = imageResizer ?? throw new ArgumentNullException(nameof(imageResizer));
            _fileSystemEntryComparer = fileSystemEntryComparer ?? throw new ArgumentNullException(nameof(fileSystemEntryComparer));
        }

        public ICollection<FileSystemEntryInfo> GetAllDrives()
        {
            return DriveInfo.GetDrives()
                            .Select(aDriveInfo =>
                                new FileSystemEntryInfo(aDriveInfo.Name,
                                                        aDriveInfo.Name,
                                                        GlobalData.DriveIcon))
                            .OrderBy(aDriveInfo => aDriveInfo.Name)
                            .ToArray();
        }

        public ICollection<FileSystemEntryInfo> GetSpecialFolders()
        {
            return SpecialFolders
                            .Select(aSpecialFolder =>
                                new FileSystemEntryInfo(
                                    aSpecialFolder,
                                    Path.Combine(UserProfilePath, aSpecialFolder),
                                    GlobalData.FolderIcon))
                            .OrderBy(aSpecialFolder => aSpecialFolder.Name)
                            .ToArray();
        }

        public ICollection<FileSystemEntryInfo> GetSubFolders(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                throw new ArgumentException("Folder path cannot be empty.", nameof(folderPath));
            }

            try
            {
                return new DirectoryInfo(folderPath)
                                            .GetDirectories()
                                            .Select(aDirectory =>
                                                new FileSystemEntryInfo(
                                                    aDirectory.Name,
                                                    aDirectory.FullName,
                                                    GlobalData.FolderIcon))
                                            .OrderBy(aDirectory => aDirectory.Name, _fileSystemEntryComparer)
                                            .ToArray();
            }
            catch
            {
                return EmptyFileSystemEntryInfoCollection;
            }
        }

        public ICollection<IImageFile> GetImageFiles(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                throw new ArgumentException("Folder path cannot be empty.", nameof(folderPath));
            }

            ICollection<FileInfo> filesInformation;
            try
            {
                filesInformation = new DirectoryInfo(folderPath)
                                            .GetFiles("*", SearchOption.TopDirectoryOnly)
                                            .ToArray();
            }
            catch
            {
                return EmptyImageFileCollection;
            }

            var imageFiles = filesInformation
                                .Where(aFileInfo =>
                                       ImageFileExtensions.Contains(aFileInfo.Extension))
                                .OrderBy(aFileInfo => aFileInfo.Name)
                                .Select(aFileInfo => _imageFileFactory.GetImageFile(_imageResizer, aFileInfo.FullName))
                                .OrderBy(aFileInfo => aFileInfo.FileName, _fileSystemEntryComparer)
                                .ToArray();

            return imageFiles;
        }

        #region Private

		static DiscQueryEngine()
		{
            EmptyFileSystemEntryInfoCollection = Enumerable
                .Empty<FileSystemEntryInfo>()
                .ToArray();

            EmptyImageFileCollection = Enumerable
                .Empty<IImageFile>()
                .ToArray();

            UserProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

			SpecialFolders = new List<string>
            {
                "Desktop",
                "Documents",
				"Downloads",
                "Pictures"
            };

			ImageFileExtensions = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
            {
                ".bmp",
                ".gif",
                ".jpg", ".jpe", ".jpeg",
                ".png",
                ".tif", ".tiff"
            };
		}

        private static readonly ICollection<FileSystemEntryInfo> EmptyFileSystemEntryInfoCollection;
        private static readonly ICollection<IImageFile> EmptyImageFileCollection;

        private static readonly string UserProfilePath;

        private static readonly ICollection<string> SpecialFolders;

		private static readonly HashSet<string> ImageFileExtensions;

        private readonly IImageFileFactory _imageFileFactory;
        private readonly IImageResizer _imageResizer;
        private readonly IFileSystemEntryComparer _fileSystemEntryComparer;

        #endregion
    }
}
