using System;
using System.Windows.Threading;

using ImageFanReloaded.Infrastructure.Interface;

namespace ImageFanReloaded.Infrastructure
{
    public class VisualActionDispatcher : IVisualActionDispatcher
    {
        public VisualActionDispatcher(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public void Invoke(Action callback)
        {
            _dispatcher.Invoke(callback);
        }

        #region Private

        private readonly Dispatcher _dispatcher;

        #endregion
    }
}
