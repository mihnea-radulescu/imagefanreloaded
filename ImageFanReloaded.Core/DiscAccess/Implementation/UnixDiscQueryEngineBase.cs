using System.Collections.Generic;
using System.Linq;
using ImageFanReloaded.Core.Global;
using ImageFanReloaded.Core.ImageHandling;

namespace ImageFanReloaded.Core.DiscAccess.Implementation;

public abstract class UnixDiscQueryEngineBase : DiscQueryEngineBase
{
	protected UnixDiscQueryEngineBase(
		IGlobalParameters globalParameters,
		IImageFileFactory imageFileFactory)
		: base(globalParameters, imageFileFactory)
	{
	}

	protected override bool IsSupportedDrive(string driveName)
	{
		if (driveName == RootPath)
		{
			return true;
		}

		var isSupportedDrive = SupportedDrivePrefixes.Any(driveName.StartsWith);

		return isSupportedDrive;
	}

	protected abstract IReadOnlyCollection<string> SupportedDrivePrefixes { get; }

	#region Private

	private const string RootPath = "/";

	#endregion
}
