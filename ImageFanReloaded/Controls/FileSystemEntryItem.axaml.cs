using Avalonia.Controls;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.DiscAccess.EntryInfo;
using ImageFanReloaded.ImageHandling.Extensions;

namespace ImageFanReloaded.Controls;

public partial class FileSystemTreeViewItem
	: UserControl, IFileSystemTreeViewItem
{
	public FileSystemTreeViewItem()
	{
		InitializeComponent();
	}

	public IFileSystemEntryInfo? FileSystemEntryInfo
	{
		get => _fileSystemEntryInfo;
		set
		{
			_fileSystemEntryInfo = value!;

			_fileSystemEntryImage.Source = _fileSystemEntryInfo.Icon.Bitmap;
			_fileSystemEntryTextBlock.Text = _fileSystemEntryInfo.Name;
		}
	}

	private IFileSystemEntryInfo? _fileSystemEntryInfo;
}
