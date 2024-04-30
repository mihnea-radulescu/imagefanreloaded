using System;

namespace ImageFanReloaded.Core.DiscAccess;

public interface IInputPathContainer : IDisposable
{
	string? InputPath { get; }
	
	bool HasPopulatedInputPath { get; set; }

	bool ShouldPopulateInputPath();
}
