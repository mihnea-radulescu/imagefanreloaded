using Avalonia.Controls;
using ImageFanReloaded.CommonTypes.Info;

namespace ImageFanReloaded.Controls.Implementation;

public partial class FileSystemEntryItem : UserControl, IFileSystemEntryItem
{
	public FileSystemEntryItem()
	{
		InitializeComponent();
	}

	public FileSystemEntryInfo FileSystemEntryInfo
	{
		get => _fileSystemEntryInfo!;
		init
		{
			_fileSystemEntryInfo = value;

			_fileSystemEntryImage.Source = _fileSystemEntryInfo.Icon;
			_fileSystemEntryTextBlock.Text = _fileSystemEntryInfo.Name;
		}
	}

	#region Private

	private readonly FileSystemEntryInfo? _fileSystemEntryInfo;

	#endregion
}
