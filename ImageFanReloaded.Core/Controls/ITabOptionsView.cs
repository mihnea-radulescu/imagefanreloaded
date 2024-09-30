using System;
using System.Threading.Tasks;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.Controls;

public interface ITabOptionsView
{
    IGlobalParameters? GlobalParameters { get; set; }
    
	IContentTabItem? ContentTabItem { get; set; }
    
	FileSystemEntryInfoOrdering FileSystemEntryInfoOrdering { get; set; }
	int ThumbnailSize { get; set; }
	bool RecursiveFolderBrowsing { get; set; }
	bool ShowImageViewImageInfo { get; set; }
	
	public event EventHandler<TabOptionsChangedEventArgs>? TabOptionsChanged;

	void PopulateTabOptions();
    
    Task ShowDialog(IMainView owner);
}
