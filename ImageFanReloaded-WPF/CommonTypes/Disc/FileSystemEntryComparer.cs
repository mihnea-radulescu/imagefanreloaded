using ImageFanReloadedWPF.CommonTypes.Disc.Interface;
using ImageFanReloadedWPF.CommonTypes.Disc.Unmanaged;

namespace ImageFanReloadedWPF.CommonTypes.Disc
{
    public class FileSystemEntryComparer : IFileSystemEntryComparer
    {
        public int Compare(string a, string b) => UnmanagedStringComparer.StrCmpLogicalW(a, b);
    }
}
