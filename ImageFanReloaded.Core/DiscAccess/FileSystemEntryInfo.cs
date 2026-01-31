using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Core.DiscAccess;

public record FileSystemEntryInfo
{
	public FileSystemEntryInfo(
		string name, string path, bool hasSubFolders, IImage icon)
	{
		Name = name;
		Path = path;
		HasSubFolders = hasSubFolders;

		Icon = icon;
	}

	public string Name { get; }
	public string Path { get; }
	public bool HasSubFolders { get; }

	public IImage Icon { get; }
}
