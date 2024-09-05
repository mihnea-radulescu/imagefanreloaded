using System;
using System.Threading.Tasks;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.Settings;
using ImageFanReloaded.Core.Synchronization;

namespace ImageFanReloaded.Core.Controls;

public interface IMainView
{
	IGlobalParameters? GlobalParameters { get; set; }
	IFolderChangedMutexFactory? FolderChangedMutexFactory { get; set; }
	
	event EventHandler<ContentTabItemEventArgs>? ContentTabItemAdded;
	event EventHandler<ContentTabItemEventArgs>? ContentTabItemClosed;
	event EventHandler<TabCountChangedEventArgs>? TabCountChanged;
	event EventHandler? AboutInfoRequested;

	void AddFakeTabItem();

	void Show();
	
	Task ShowAboutInfo(IAboutView aboutView);
}
