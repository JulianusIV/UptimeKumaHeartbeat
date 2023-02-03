namespace UptimeKumaHeartbeat
{
    public class HeartbeatManager : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        PeriodicTimer? _heartbeatTimer;

        /// <summary>
        /// Sends a single Heartbeat to your Kuma monitor
        /// </summary>
        /// <param name="targetUrl">The Push Url from Kuma, up to the first parameter</param>
        /// <param name="data">A <seealso cref="HeartbeatData"/> object containing a status and message string, and a ping integer</param>
        /// <returns></returns>
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

        /// <summary>
        /// Starts sending continuous Heartbeats to your Kuma monitor
        /// </summary>
        /// <param name="targetUrl">The Push Url from Kuma, up to the first parameter</param>
        /// <param name="data">A <see cref="HeartbeatData"/> object containing a status and message string, and a ping integer. You can modify this to change the data sent with the next heartbeat</param>
        /// <param name="cancellationToken">A cancellation token for cancelling the background task (If none is passed, the Task is cancelled when <seealso cref="Dispose"/> is called</param>
        /// <param name="interval">Time between Heartbeats in milliseconds</param>
        /// <returns></returns>
        [Obsolete("This Overload is obsolete, use an overload accepting a TimeSpan instead.")]
        public Task StartHeartbeatsAsync(string targetUrl, HeartbeatData data, int interval, CancellationToken? cancellationToken = null)
            => StartHeartbeatsAsync(targetUrl, data, cancellationToken, TimeSpan.FromMilliseconds(interval));

        /// <summary>
        /// Starts sending continuous Heartbeats to your Kuma monitor
        /// </summary>
        /// <param name="targetUrl">The Push Url from Kuma, up to the first parameter</param>
        /// <param name="data">A <see cref="HeartbeatData"/> object containing a status and message string, and a ping integer. You can modify this to change the data sent with the next heartbeat</param>
        /// <param name="cancellationToken">A cancellation token for cancelling the background task (If none is passed, the Task is cancelled when <seealso cref="Dispose"/> is called</param>
        /// <param name="interval">Time between Heartbeats - default is 60 seconds</param>
        /// <returns></returns>
        public Task StartHeartbeatsAsync(string targetUrl, HeartbeatData data, CancellationToken? cancellationToken = null, TimeSpan? interval = null)
        {
            interval ??= TimeSpan.FromMinutes(1);

            CancellationToken token = cancellationToken is null ? _cancellationTokenSource.Token : (CancellationToken)cancellationToken;

            _ = Task.Factory.StartNew(async () =>
            {
                _heartbeatTimer = new(interval.Value);
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
}