using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Controls.Factories;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Controls.Factories;

public class ImageInfoViewFactory : IImageInfoViewFactory
{
	public ImageInfoViewFactory(IGlobalParameters globalParameters)
	{
		_globalParameters = globalParameters;
	}
	
	public IImageInfoView GetImageInfoView(IImageFile imageFile)
	{
		IImageInfoView imageInfoView = new ImageInfoWindow();
		imageInfoView.GlobalParameters = _globalParameters;

		imageInfoView.SetImageInfoText(imageFile.ImageInfo!.ImageInfoText);
		
		return imageInfoView;
	}
	
	#region Private
	
	private readonly IGlobalParameters _globalParameters;

	#endregion
}
