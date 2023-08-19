namespace ImageFanReloaded.CommonTypes.Disc.Implementation
{
    public class FileSystemEntryComparer
        : IFileSystemEntryComparer
    {
        public int Compare(string a, string b)
            => UnmanagedStringComparer.StrCmpLogicalW(a, b);
    }
}
