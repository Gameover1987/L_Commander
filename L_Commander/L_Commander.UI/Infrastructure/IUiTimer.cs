using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;

namespace L_Commander.UI.Infrastructure
{
    public interface IUiTimer
    {
        event EventHandler<EventArgs> Tick;

        void Initialize(TimeSpan interval);

        void Start();

        void Stop();
    }

    public class UiTimer : IUiTimer
    {
        private readonly DispatcherTimer _timer = new DispatcherTimer(DispatcherPriority.Background);

        public UiTimer()
        {
            _timer.Tick += TimerOnTick;
        }

        public void Initialize(TimeSpan interval)
        {
            _timer.Interval = interval;
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public event EventHandler<EventArgs> Tick;

        private void TimerOnTick(object? sender, EventArgs e)
        {
            Tick?.Invoke(this, e);
        }
    }
}
