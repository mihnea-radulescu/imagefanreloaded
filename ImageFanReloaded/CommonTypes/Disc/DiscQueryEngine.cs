using ImageFanReloaded.CommonTypes.Disc.Interface;
using ImageFanReloaded.CommonTypes.ImageHandling.Interface;
using ImageFanReloaded.CommonTypes.Info;
using ImageFanReloaded.Factories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageFanReloaded.CommonTypes.Disc
{
    public class DiscQueryEngine
        : IDiscQueryEngine
    {
        public static readonly DiscQueryEngine Instance = new DiscQueryEngine();
        
        public IEnumerable<FileSystemEntryInfo> GetAllDrives()
        {
            return DriveInfo.GetDrives()
                            .Select(aDriveInfo =>
                                new FileSystemEntryInfo(aDriveInfo.Name,
                                                        aDriveInfo.Name,
                                                        GlobalData.DriveIcon))
                            .OrderBy(aDriveInfo => aDriveInfo.Name)
                            .ToArray();
        }

        public IEnumerable<FileSystemEntryInfo> GetSpecialFolders()
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

        public IEnumerable<FileSystemEntryInfo> GetSubFolders(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                throw new ArgumentException("Folder path cannot be empty.", "folderPath");

            try
            {
                return new DirectoryInfo(folderPath)
                                            .GetDirectories()
                                            .Select(aDirectory =>
                                                new FileSystemEntryInfo(
                                                    aDirectory.Name,
                                                    aDirectory.FullName,
                                                    GlobalData.FolderIcon))
                                            .OrderBy(aDirectory => aDirectory.Name)
                                            .ToArray();
            }
            catch
            {
                return Enumerable.Empty<FileSystemEntryInfo>();
            }
        }

        public IEnumerable<IImageFile> GetImageFiles(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
                throw new ArgumentException("Folder path cannot be empty.", "folderPath");
            
            IEnumerable<FileInfo> filesInformation;
            try
            {
                filesInformation = new DirectoryInfo(folderPath)
                                            .GetFiles("*", SearchOption.TopDirectoryOnly);
            }
            catch
            {
                return Enumerable.Empty<IImageFile>();
            }

            var imageFiles = filesInformation
                                .Where(aFileInfo =>
                                       ImageFileExtensions.Contains(aFileInfo.Extension))
                                .OrderBy(aFileInfo => aFileInfo.Name)
                                .Select(aFileInfo =>
                                       TypesFactoryResolver.TypesFactoryInstance
                                                           .GetImageFile(aFileInfo.FullName))
                                .OrderBy(aFileInfo => aFileInfo.FileName)
                                .ToArray();

            return imageFiles;
        }


        #region Private

        private DiscQueryEngine()
        {
        }

		static DiscQueryEngine()
		{
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

		private static readonly string UserProfilePath;

        private static readonly ICollection<string> SpecialFolders;

		private static readonly HashSet<string> ImageFileExtensions;

        #endregion
    }
}
