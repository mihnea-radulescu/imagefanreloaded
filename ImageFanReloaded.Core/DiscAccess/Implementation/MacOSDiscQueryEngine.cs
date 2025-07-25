using System.Collections.Generic;
using ImageFanReloaded.Core.ImageHandling.Factories;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class MacOSDiscQueryEngine : UnixDiscQueryEngineBase
{
	public MacOSDiscQueryEngine(
		IGlobalParameters globalParameters,
		IFileSizeEngine fileSizeEngine,
		IImageFileFactory imageFileFactory)
		: base(globalParameters, fileSizeEngine, imageFileFactory)
	{
		_supportedDrivePrefixes = [ "/Volumes/" ];
	}
	
	#region Protected

	protected override IReadOnlyList<string> SupportedDrivePrefixes => _supportedDrivePrefixes;
	
	#endregion

	#region Private

	private readonly IReadOnlyList<string> _supportedDrivePrefixes;

	#endregion
}
