using System;
using Avalonia.Threading;

namespace ImageFanReloaded.Infrastructure.Implementation;

public class VisualActionDispatcher
    : IVisualActionDispatcher
{
    public VisualActionDispatcher(Dispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public void Invoke(Action callback)
    {
        _dispatcher.Invoke(callback);
    }

    #region Private

    private readonly Dispatcher _dispatcher;

    #endregion
}
