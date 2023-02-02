namespace UptimeKumaHeartbeat
{
    public class HeartbeatManager : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        PeriodicTimer? _heartbeatTimer;

        public static async Task SendSingleHeartBeatAsync(string targetUrl, HeartbeatData data)
        {
            try
            {
                using var client = new HttpClient();

                var requestUri = new Uri($"{targetUrl}?status={data.Status}&msg={data.Message}&ping={(data.Ping is null ? string.Empty : ((int)data.Ping).ToString())}");

                await client.GetAsync(requestUri);
            }
            catch (Exception)
            {
                //probably just connection issues, so we likely got bigger problems right here
            }
        }

        public Task StartHeartbeatsAsync(string targetUrl, HeartbeatData data, CancellationToken? cancellationToken = null, int interval = 60000)
        {
            CancellationToken token = cancellationToken is null ? _cancellationTokenSource.Token : (CancellationToken)cancellationToken;

            _ = Task.Factory.StartNew(async () =>
            {
                _heartbeatTimer = new(TimeSpan.FromMilliseconds(interval));
                do
                    await SendSingleHeartBeatAsync(targetUrl, data);
                while (await _heartbeatTimer.WaitForNextTickAsync(token));

            }, TaskCreationOptions.LongRunning);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _heartbeatTimer?.Dispose();
            _cancellationTokenSource.Cancel();
            GC.SuppressFinalize(this);
        }
    }

    public class HeartbeatData
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public int? Ping { get; set; }

        public HeartbeatData(string status, string message, int? ping = null)
        {
            Status = status;
            Message = message;
            Ping = ping;
        }
    }
}