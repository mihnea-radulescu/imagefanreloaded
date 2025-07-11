using Avalonia.Controls;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.ImageHandling.Extensions;

namespace ImageFanReloaded.Controls;

public partial class FileSystemTreeViewItem : UserControl, IFileSystemTreeViewItem
{
	public FileSystemTreeViewItem()
	{
		InitializeComponent();
	}

	public FileSystemEntryInfo? FileSystemEntryInfo
	{
		get => _fileSystemEntryInfo;
		set
		{
			_fileSystemEntryInfo = value!;

			_fileSystemEntryImage.Source = _fileSystemEntryInfo.Icon.GetBitmap();
			_fileSystemEntryTextBlock.Text = _fileSystemEntryInfo.Name;
		}
	}

	#region Private

	private FileSystemEntryInfo? _fileSystemEntryInfo;

	#endregion
}
