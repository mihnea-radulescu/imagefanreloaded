using ImageFanReloaded.Core.ImageHandling.Factories;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class WindowsDiscQueryEngine : DiscQueryEngineBase
{
	public WindowsDiscQueryEngine(
		IGlobalParameters globalParameters, IImageFileFactory imageFileFactory)
			: base(globalParameters, imageFileFactory)
	{
	}

	protected override bool IsSupportedDrive(string driveName) => true;
}
