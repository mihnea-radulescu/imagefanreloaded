using System.Threading.Tasks;

namespace ImageFanReloaded.Core.Synchronization;

public interface IAsyncAutoResetEvent
{
	Task WaitOne();

	Task Set();
}
