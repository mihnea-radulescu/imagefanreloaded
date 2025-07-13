using System;
using System.Threading.Tasks;
using ImageFanReloaded.Core.CustomEventArgs;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.Controls;

public interface ITabOptionsView
{
	IGlobalParameters? GlobalParameters { get; set; }
	ITabOptions? TabOptions { get; set; }

	IContentTabItem? ContentTabItem { get; set; }

	event EventHandler<TabOptionsChangedEventArgs>? TabOptionsChanged;

	void PopulateTabOptions();

	Task ShowDialog(IMainView owner);
}
