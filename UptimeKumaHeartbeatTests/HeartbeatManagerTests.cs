using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UptimeKumaHeartbeat.Tests
{
    [TestClass]
    public class HeartbeatManagerTests
    {
        [TestMethod]
        public void SendSingleHeartBeatAsyncTest()
        {
            HeartbeatManager.SendSingleHeartBeatAsync("Insert your url to test on here", new("", "")).GetAwaiter().GetResult();
        }

        [TestMethod]
        public void StartHeartbeatsAsyncTests()
        {
            CancellationTokenSource cancellationTokenSource = new();

            using HeartbeatManager heartbeatManager = new();
            HeartbeatData data = new("", "");
            var task = heartbeatManager.StartHeartbeatsAsync("insert your url to test on here", data, cancellationTokenSource.Token);

            cancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(3));
            data.Message = "test";
            data.Ping = 1000;

            cancellationTokenSource.Token.WaitHandle.WaitOne();
            if (task.Exception is not null)
                Assert.Fail();
            Assert.IsTrue(task.IsCompletedSuccessfully);
        }
    }
}