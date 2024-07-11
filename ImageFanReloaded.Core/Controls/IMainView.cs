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
	event EventHandler? HelpMenuRequested;

	void AddFakeTabItem();
	
	Task ShowInfoMessage(string title, string text);

	void Show();
}
