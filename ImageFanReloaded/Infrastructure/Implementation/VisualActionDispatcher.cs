using System;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace ImageFanReloaded.Infrastructure.Implementation;

public class VisualActionDispatcher : IVisualActionDispatcher
{
    public VisualActionDispatcher(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public void Invoke(Action callback)
    {
        try
        {
            _dispatcher.Invoke(callback);
        }
        catch (TaskCanceledException)
        {
        }
    }

    #region Private

    private readonly Dispatcher _dispatcher;

    #endregion
}
