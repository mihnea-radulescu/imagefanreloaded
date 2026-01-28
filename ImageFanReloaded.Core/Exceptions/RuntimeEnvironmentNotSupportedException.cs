using System;

namespace ImageFanReloaded.Core.Exceptions;

public class RuntimeEnvironmentNotSupportedException
	: PlatformNotSupportedException
{
	public RuntimeEnvironmentNotSupportedException()
		: base("Runtime environment not supported!")
	{
	}
}
