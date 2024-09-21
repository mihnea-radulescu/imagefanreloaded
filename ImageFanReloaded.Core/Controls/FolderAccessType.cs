namespace ImageFanReloaded.Core.Controls;

public enum FolderAccessType
{
	Normal = 0,
	Recursive = 1,
	PersistentRecursive = 2
}

public static class FolderAccessTypeExtensions
{
	public static bool IsRecursive(this FolderAccessType folderAccessType) =>
		folderAccessType is FolderAccessType.Recursive or FolderAccessType.PersistentRecursive;
}
