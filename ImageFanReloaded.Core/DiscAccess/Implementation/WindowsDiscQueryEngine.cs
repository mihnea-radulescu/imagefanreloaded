using ImageFanReloaded.Core.Global;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class WindowsDiscQueryEngine : DiscQueryEngineBase
{
	public WindowsDiscQueryEngine(
		IGlobalParameters globalParameters,
		IFileSizeEngine fileSizeEngine,
		IImageFileFactory imageFileFactory)
		: base(globalParameters, fileSizeEngine, imageFileFactory)
	{
	}

	protected override bool IsSupportedDrive(string driveName) => true;
}
