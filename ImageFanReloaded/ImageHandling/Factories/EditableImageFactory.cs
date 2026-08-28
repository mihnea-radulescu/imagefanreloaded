using System.Threading.Tasks;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Factories;
using ImageFanReloaded.Core.ImageHandling.ImageFileData;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.ImageHandling.Factories;

public class EditableImageFactory : IEditableImageFactory
{
	public EditableImageFactory(IGlobalParameters globalParameters)
	{
		_globalParameters = globalParameters;
	}

	public async Task<IEditableImage?> CreateEditableImage(
		IImageFileData imageFileData)
	{
		IEditableImage? editableImage = null;

		try
		{
			editableImage = await Task.Run(()
				=> new EditableImage(imageFileData,
									 _globalParameters.ImageQualityLevel));
		}
		catch
		{
		}

		return editableImage;
	}

	private readonly IGlobalParameters _globalParameters;
}
