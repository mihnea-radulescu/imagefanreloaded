using System.Threading.Tasks;
using ImageFanReloaded.Core.ImageHandling;
using ImageFanReloaded.Core.ImageHandling.Factories;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.ImageHandling.Factories;

public class EditableImageFactory : IEditableImageFactory
{
	public EditableImageFactory(IGlobalParameters globalParameters)
	{
		_globalParameters = globalParameters;
	}

	public async Task<IEditableImage?> CreateEditableImage(string imageFilePath)
	{
		IEditableImage? editableImage = null;

		try
		{
			editableImage = await Task.Run(()
				=> new EditableImage(imageFilePath,
									 _globalParameters.ImageQualityLevel));
		}
		catch
		{
		}

		return editableImage;
	}

	private readonly IGlobalParameters _globalParameters;
}
