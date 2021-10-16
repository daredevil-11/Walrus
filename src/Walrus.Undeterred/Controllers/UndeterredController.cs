using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Walrus.Undeterred.Core;
using Walrus.Undeterred.Hubs;

namespace Walrus.Undeterred.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UndeterredController : ControllerBase
    {
        private readonly IRougeCore _rougeCore;
        private readonly ILogger<UndeterredController> _logger;
        private readonly IHubContext<UndeterredApiHub> _apiHubContext;

        public UndeterredController(IRougeCore rougeCore, 
            IHubContext<UndeterredApiHub> apiHubContext, ILogger<UndeterredController> logger)
        {
            _rougeCore = rougeCore ?? throw new ArgumentNullException(nameof(rougeCore));
            _apiHubContext = apiHubContext ?? throw new ArgumentNullException(nameof(apiHubContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ObjectResult> Get()
        {
            var swOverall = Stopwatch.StartNew();
            var swRouge = new Stopwatch();
            _logger.LogInformation($"UndeterredController.Get: Starting Api call...");

            // do some stuff...

            // call the rouge api...
            var rougeRetryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    5,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retries, context) => 
                    {

                        _logger.LogError($"UndeterredController.Get: Exception while calling Rouge Api [Retries: {retries}, executionTime: {swRouge.ElapsedMilliseconds} ms]...");
                        _logger.LogError($"UndeterredController.Get: Next call to Rouge Api will be after {timeSpan.TotalSeconds} secs.");

                        // notify ui of failure...
                        _apiHubContext.Clients.All.SendAsync(UndeterredApiHubAction.ROUGE_API_ISSUE, retries);
                    }
                );

            var response = await rougeRetryPolicy.ExecuteAsync(async () => 
            {
                swRouge.Restart();
                return await _rougeCore.Get();
            });

            // do some additional stuff...

            swOverall.Stop();
            _logger.LogInformation($"[{DateTimeOffset.UtcNow}]UndeterredController.Get: Finishing Api call [ExecutionTime: {swOverall.ElapsedMilliseconds} ms]...");
            if (response)
            {
                return new ObjectResult(response)
                {
                    StatusCode = 200
                };
            }
            else
            {
                return new ObjectResult(response)
                {
                    StatusCode = 500
                };
            }
        }
    }
}
