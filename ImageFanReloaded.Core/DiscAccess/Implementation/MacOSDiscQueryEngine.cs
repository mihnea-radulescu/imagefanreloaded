using System.Collections.Generic;
using ImageFanReloaded.Core.Global;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public class MacOSDiscQueryEngine : UnixDiscQueryEngineBase
{
	public MacOSDiscQueryEngine(
		IGlobalParameters globalParameters,
		IImageFileFactory imageFileFactory)
		: base(globalParameters, imageFileFactory)
	{
		_supportedDrivePrefixes = new List<string>
		{
			"/Volumes/"
		};
	}

	protected override IReadOnlyCollection<string> SupportedDrivePrefixes => _supportedDrivePrefixes;

	#region Private

	private readonly IReadOnlyCollection<string> _supportedDrivePrefixes;

	#endregion
}
