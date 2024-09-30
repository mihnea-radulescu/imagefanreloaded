using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Controls;

public class TabOptionsViewFactory : ITabOptionsViewFactory
{
	public TabOptionsViewFactory(IGlobalParameters globalParameters)
	{
		_globalParameters = globalParameters;
	}
	
	public ITabOptionsView GetTabOptionsView(IContentTabItem contentTabItem)
	{
		ITabOptionsView tabOptionsView = new TabOptionsWindow();
		
		tabOptionsView.GlobalParameters = _globalParameters;
		
		tabOptionsView.ContentTabItem = contentTabItem;
		
		tabOptionsView.FileSystemEntryInfoOrdering = contentTabItem.FileSystemEntryInfoOrdering;
		tabOptionsView.ThumbnailSize = contentTabItem.ThumbnailSize;
		tabOptionsView.RecursiveFolderBrowsing = contentTabItem.RecursiveFolderBrowsing;
		tabOptionsView.ShowImageViewImageInfo = contentTabItem.ShowImageViewImageInfo;
		
		tabOptionsView.PopulateTabOptions();

		return tabOptionsView;
	}
	
	#region Private
	
	private readonly IGlobalParameters _globalParameters;
	
	#endregion
}
