using System.Collections.Generic;
using ImageFanReloaded.Core.Global;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class MacOSDiscQueryEngine : UnixDiscQueryEngineBase
{
	public MacOSDiscQueryEngine(
		IGlobalParameters globalParameters,
		IFileSizeEngine fileSizeEngine,
		IImageFileFactory imageFileFactory)
		: base(globalParameters, fileSizeEngine, imageFileFactory)
	{
		_allowedDrivePrefixes = ["/Volumes/"];
		_disallowedDriveFragments = [];
	}
	
	#region Protected

	protected override IReadOnlyList<string> AllowedDrivePrefixes => _allowedDrivePrefixes;
	protected override IReadOnlyList<string> DisallowedDriveFragments => _disallowedDriveFragments;
	
	#endregion

	#region Private

	private readonly IReadOnlyList<string> _allowedDrivePrefixes;
	private readonly IReadOnlyList<string> _disallowedDriveFragments;

	#endregion
}
