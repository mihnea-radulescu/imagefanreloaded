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
		_allowedDrivePrefixes = ["/home/", "/media/", "/mnt/"];
		_disallowedDriveFragments = ["/.local/share/flatpak", "/.var/app"];
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
