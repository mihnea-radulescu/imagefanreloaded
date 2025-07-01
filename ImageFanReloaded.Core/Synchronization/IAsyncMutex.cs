using System;
using System.Threading.Tasks;

namespace ImageFanReloaded.Core.Synchronization;

public interface IAsyncMutex : IDisposable
{
	Task Wait();

	void Signal();
}
