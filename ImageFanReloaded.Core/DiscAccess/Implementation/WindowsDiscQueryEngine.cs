using ImageFanReloaded.Core.Global;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class WindowsDiscQueryEngine : DiscQueryEngineBase
{
	public WindowsDiscQueryEngine(
		IGlobalParameters globalParameters,
		IImageFileFactory imageFileFactory)
		: base(globalParameters, imageFileFactory)
	{
	}

	protected override bool IsSupportedDrive(string driveName) => true;
}
