using Avalonia.Controls;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.DiscAccess;
using ImageFanReloaded.ImageHandling;

namespace ImageFanReloaded.Controls;

public partial class FileSystemTreeViewItem : UserControl, IFileSystemTreeViewItem
{
	public FileSystemTreeViewItem()
	{
		InitializeComponent();
	}

	public FileSystemEntryInfo FileSystemEntryInfo
	{
		get => _fileSystemEntryInfo!;
		init
		{
			_fileSystemEntryInfo = value;

			_fileSystemEntryImage.Source = _fileSystemEntryInfo.Icon.GetBitmap();
			_fileSystemEntryTextBlock.Text = _fileSystemEntryInfo.Name;
		}
	}

	#region Private

	private readonly FileSystemEntryInfo? _fileSystemEntryInfo;

	#endregion
}
