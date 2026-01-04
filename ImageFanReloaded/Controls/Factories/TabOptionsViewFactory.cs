using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Controls.Factories;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Controls.Factories;

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
		tabOptionsView.TabOptions = contentTabItem.TabOptions;

		tabOptionsView.ContentTabItem = contentTabItem;

		tabOptionsView.PopulateTabOptions();

		return tabOptionsView;
	}

	private readonly IGlobalParameters _globalParameters;
}
