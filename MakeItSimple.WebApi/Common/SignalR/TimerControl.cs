namespace MakeItSimple.WebApi.Common.SignalR
{
    public class TimerControl
    {
        private Timer? _timer;
        private AutoResetEvent? _autoResetEvent;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DateTime TimerStarted { get; set; }
        public bool IsTimerStarted { get; set; }

        public TimerControl(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void ScheduleTimer(Func<IServiceScopeFactory, Task> scopedAction, int paramTime)
        {
            _timer?.Dispose(); // Dispose previous timer if one exists
            _timer = new Timer(async _ =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                await scopedAction(scope.ServiceProvider.GetRequiredService<IServiceScopeFactory>());
            }, null, 1000, paramTime);

            TimerStarted = DateTime.Now;
            IsTimerStarted = true;
        }

        public void StopTimer()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
                IsTimerStarted = false;
            }
        }


    }
}
