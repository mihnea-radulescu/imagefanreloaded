using System.Collections.Generic;
using ImageFanReloaded.Core.Global;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class LinuxDiscQueryEngine : UnixDiscQueryEngineBase
{
	public LinuxDiscQueryEngine(
		IGlobalParameters globalParameters,
		IFileSizeEngine fileSizeEngine,
		IImageFileFactory imageFileFactory)
		: base(globalParameters, fileSizeEngine, imageFileFactory)
	{
		_supportedDrivePrefixes = ["/media/", "/mnt/"];
	}

	#region Protected

	protected override IReadOnlyList<string> SupportedDrivePrefixes => _supportedDrivePrefixes;
	
	#endregion

	#region Private

	private readonly IReadOnlyList<string> _supportedDrivePrefixes;

	#endregion
}
