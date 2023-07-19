using ImageFanReloaded.CommonTypes.Disc.Interface;
using ImageFanReloaded.CommonTypes.Disc.Unmanaged;

namespace ImageFanReloaded.CommonTypes.Disc
{
    public class FileSystemEntryComparer : IFileSystemEntryComparer
    {
        public int Compare(string a, string b) => UnmanagedStringComparer.StrCmpLogicalW(a, b);
    }
}
