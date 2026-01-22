using System.Net;

namespace LeaveMeAloneFuncSkillForge.API
{
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private static readonly Random _random = new();

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // simulate network delay
            int delayMs = _random.Next(500, 5000);
            Console.WriteLine($"[FAKE HTTP] Delay {delayMs} ms");

            await Task.Delay(delayMs, cancellationToken);

            // 40% chance to simulate a network error
            if (_random.NextDouble() < 0.4)
            {
                Console.WriteLine("[FAKE HTTP] Simulated network error");
                throw new HttpRequestException("Simulated network failure");
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($"Fake response from {request.RequestUri}")
            };
        }
    }
}
