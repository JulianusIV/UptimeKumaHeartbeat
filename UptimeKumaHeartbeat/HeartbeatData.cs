namespace UptimeKumaHeartbeat
{
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
