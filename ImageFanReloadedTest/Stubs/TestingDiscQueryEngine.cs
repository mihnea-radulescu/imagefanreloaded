using ImageFanReloaded;
using ImageFanReloaded.CommonTypes.Disc.Interface;
using ImageFanReloaded.CommonTypes.ImageHandling.Interface;
using ImageFanReloaded.CommonTypes.Info;
using System.Collections.Generic;
using System.IO;

namespace ImageFanReloadedTest.Stubs
{
    public class TestingDiscQueryEngine
        : IDiscQueryEngine
    {
        public IEnumerable<FileSystemEntryInfo> GetAllDrives()
        {
            return new[]
            { 
                new FileSystemEntryInfo(@"C:\", @"C:\", GlobalData.DriveIcon),
                new FileSystemEntryInfo(@"D:\", @"D:\", GlobalData.DriveIcon),
                new FileSystemEntryInfo(@"E:\", @"E:\", GlobalData.DriveIcon)
            };
        }

        public IEnumerable<FileSystemEntryInfo> GetSpecialFolders()
        {
            return new[]
            { 
                new FileSystemEntryInfo(@"Special", @"C:\Special", GlobalData.FolderIcon)
            };
        }

        public IEnumerable<FileSystemEntryInfo> GetSubFolders(string folderPath)
        {
            return new[]
            { 
                new FileSystemEntryInfo("Folder1",
                                        Path.Combine(folderPath, "Folder1"),
                                        GlobalData.FolderIcon),
                new FileSystemEntryInfo("Folder2",
                                        Path.Combine(folderPath, "Folder2"),
                                        GlobalData.FolderIcon),
                new FileSystemEntryInfo("Folder3",
                                        Path.Combine(folderPath, "Folder3"),
                                        GlobalData.FolderIcon),
            };
        }

        public IEnumerable<IImageFile> GetImageFiles(string folderPath)
        {
            return new[]
            { 
                new TestingImageFile(Path.Combine(folderPath, "image1.jpg")),
                new TestingImageFile(Path.Combine(folderPath, "image2.jpg")),
                new TestingImageFile(Path.Combine(folderPath, "image3.jpg"))
            };
        }
    }
}
