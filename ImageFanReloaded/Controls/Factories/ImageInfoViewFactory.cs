using System.Threading.Tasks;
using ImageFanReloaded.Core.Controls;
using ImageFanReloaded.Core.Controls.Factories;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Controls.Factories;

public class ImageInfoViewFactory : IImageInfoViewFactory
{
	public ImageInfoViewFactory(
		IGlobalParameters globalParameters,
		IImageInfoExtractor imageInfoExtractor)
	{
		_globalParameters = globalParameters;

		_imageInfoExtractor = imageInfoExtractor;
	}
	
	public async Task<IImageInfoView> GetImageInfoView(IImageFile imageFile)
	{
		IImageInfoView imageInfoView = new ImageInfoWindow();
		imageInfoView.GlobalParameters = _globalParameters;

		var imageInfo = await _imageInfoExtractor.BuildImageInfo(imageFile);
		imageInfoView.SetImageInfoText(imageInfo);
		
		return imageInfoView;
	}
	
	#region Private
	
	private readonly IGlobalParameters _globalParameters;

	private readonly IImageInfoExtractor _imageInfoExtractor;

	#endregion
}
