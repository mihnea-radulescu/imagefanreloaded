using System;
using System.Threading.Tasks;

namespace ImageFanReloaded.Core.Synchronization;

public interface IFolderChangedMutex : IDisposable
{
	Task Wait();

	void Signal();
}
