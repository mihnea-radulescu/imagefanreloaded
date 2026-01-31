using ImageFanReloaded.Core.ImageHandling.Factories;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class WindowsDiscQueryEngine : DiscQueryEngineBase
{
	public WindowsDiscQueryEngine(
		IGlobalParameters globalParameters,
		IImageFileFactory imageFileFactory,
		IFileSizeEngine fileSizeEngine)
		: base(globalParameters, imageFileFactory, fileSizeEngine)
	{
	}

	protected override bool IsSupportedDrive(string driveName) => true;
}
