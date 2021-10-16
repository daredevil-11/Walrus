using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Walrus.Undeterred.Core
{
    public class RougeCore : IRougeCore
    {
        private const string TEST_URL = "Rouge/Test";
        private const string GET_URL = "Rouge/Get";

        private readonly HttpClient _client;
        private readonly ILogger<RougeCore> _logger;

        public RougeCore(HttpClient client, ILogger<RougeCore> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Test()
        {
            var response = await _client.GetAsync(TEST_URL);
            var responseContent =  await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<bool>(responseContent);
        }

        public async Task<bool> Get()
        {
            _logger.LogInformation("RougeCore.Get: Starting...");

            var response = await _client.GetAsync(GET_URL);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("RougeCore.Get: Finishing...");
            return JsonConvert.DeserializeObject<bool>(responseContent);
        }
    }
}
