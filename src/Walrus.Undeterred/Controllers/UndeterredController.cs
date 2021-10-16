﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Walrus.Undeterred.Core;

namespace Walrus.Undeterred.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UndeterredController : ControllerBase
    {
        private readonly IRougeCore _rougeCore;
        private readonly ILogger<UndeterredController> _logger;

        public UndeterredController(IRougeCore rougeCore, ILogger<UndeterredController> logger)
        {
            _rougeCore = rougeCore ?? throw new ArgumentNullException(nameof(rougeCore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ObjectResult> Get()
        {
            var swOverall = Stopwatch.StartNew();
            var swRouge = new Stopwatch();
            _logger.LogInformation($"[{DateTimeOffset.UtcNow}]UndeterredController.Get: Starting Api call...");

            // do some stuff...

            // call the rouge api...
            var rougeRetryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    5,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retries, context) => 
                    {

                        _logger.LogError($"[{DateTimeOffset.UtcNow}]UndeterredController.Get: Exception while calling Rouge Api [Retries: {retries}, Current executionTime: {swRouge.ElapsedMilliseconds} ms]...");
                        _logger.LogError($"[{DateTimeOffset.UtcNow}]UndeterredController.Get: Next call to Rouge Api will be after {timeSpan.TotalSeconds} secs.");

                        // notify ui of failure...

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