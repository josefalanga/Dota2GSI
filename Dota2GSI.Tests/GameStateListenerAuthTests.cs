using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Xunit;

namespace Dota2GSI.Tests
{
    public class GameStateListenerAuthTests
    {
        private static int GetFreePort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        private static string Payload(int clockTime, string token)
        {
            return $@"{{ ""auth"": {{ ""token"": ""{token}"" }}, ""map"": {{ ""clock_time"": {clockTime} }} }}";
        }

        private static bool TryPost(int port, string body, out int status)
        {
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:{port}/")
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json"),
            };
            using var resp = client.Send(request);
            status = (int)resp.StatusCode;
            return resp.IsSuccessStatusCode;
        }

        [Fact]
        public void Listener_RejectsPayload_WhenTokenMismatches()
        {
            int accepted = 0;
            using var listener = new GameStateListener(GetFreePort(), "expected-token");
            listener.NewGameState += _ => Interlocked.Increment(ref accepted);

            listener.Start();

            // Wrong token -> 401, no game state.
            var ok = TryPost(listener.Port, Payload(clockTime: 1, token: "wrong"), out var status);
            Assert.False(ok);
            Assert.Equal(401, status);

            // Missing auth -> 401, no game state.
            ok = TryPost(listener.Port, @"{ ""map"": { ""clock_time"": 2 } }", out status);
            Assert.False(ok);
            Assert.Equal(401, status);

            // Correct token -> 200, one accepted frame.
            Assert.True(TryPost(listener.Port, Payload(clockTime: 3, token: "expected-token"), out status));
            Assert.Equal(200, status);

            // The listener processes on a background thread; give it a moment.
            Assert.True(SpinWait.SpinUntil(() => accepted >= 1, TimeSpan.FromSeconds(5)), "accepted frame never surfaced");
            Assert.Equal(1, accepted);
        }

        [Fact]
        public void Listener_AcceptsAnyPayload_WhenNoTokenConfigured()
        {
            using var listener = new GameStateListener(GetFreePort());
            listener.NewGameState += _ => { };
            listener.Start();

            // Backward-compatible: no expected token -> any payload accepted.
            Assert.True(TryPost(listener.Port, Payload(clockTime: 5, token: "anything"), out var status));
            Assert.Equal(200, status);
        }

        [Fact]
        public void Listener_DelegatesValidation_ToPredicate()
        {
            var accepted = 0;
            using var listener = new GameStateListener(GetFreePort(), token => token == "delegated-token");
            listener.NewGameState += _ => Interlocked.Increment(ref accepted);
            listener.Start();

            // Predicate rejects a mismatch -> 401, no state.
            Assert.False(TryPost(listener.Port, Payload(clockTime: 7, token: "nope"), out var status));
            Assert.Equal(401, status);

            // Predicate accepts -> 200, state surfaces.
            Assert.True(TryPost(listener.Port, Payload(clockTime: 8, token: "delegated-token"), out status));
            Assert.Equal(200, status);
            Assert.True(SpinWait.SpinUntil(() => accepted >= 1, TimeSpan.FromSeconds(5)), "accepted frame never surfaced");
        }
    }
}