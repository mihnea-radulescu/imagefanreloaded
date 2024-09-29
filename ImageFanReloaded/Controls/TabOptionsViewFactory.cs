using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Controls;

public class TabOptionsViewFactory : ITabOptionsViewFactory
{
	public TabOptionsViewFactory(IGlobalParameters globalParameters)
	{
		_globalParameters = globalParameters;
	}
	
	public ITabOptionsView GetTabOptionsView()
	{
		ITabOptionsView tabOptionsView = new TabOptionsWindow();
		tabOptionsView.GlobalParameters = _globalParameters;

		return tabOptionsView;
	}
	
	#region Private
	
	private readonly IGlobalParameters _globalParameters;
	
	#endregion
}
