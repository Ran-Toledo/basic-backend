namespace BasicBackend.Utilities
{
    public readonly struct StopwatchSlim
    {
        private readonly long _start;

        private StopwatchSlim(long s)
        {
            _start = s;
        }

        public static StopwatchSlim StartNew()
        {
            return new StopwatchSlim(DateTime.UtcNow.Ticks);
        }

        public long ElapsedMilliseconds
        {
            get { return (DateTime.UtcNow.Ticks - _start) / TimeSpan.TicksPerMillisecond; }
        }
    }
}
