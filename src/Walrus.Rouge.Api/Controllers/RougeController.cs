using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;

namespace Walrus.Rouge.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class RougeController : ControllerBase
    {
        private const int MAX_FAILURE_COUNT = 3;
        private readonly IMemoryCache _memoryCache;

        public RougeController(IMemoryCache memoryCache) 
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        [HttpGet]
        public ObjectResult Get()
        {
            _memoryCache.TryGetValue(Constants._failureType, out string failureTypeStr);
            if(failureTypeStr == default || string.IsNullOrWhiteSpace(failureTypeStr) 
                || !Enum.TryParse(failureTypeStr, out Constants.FailureTypes failureType))
            {
                failureType = (Constants.FailureTypes)(DateTimeOffset.Now.Millisecond % 2 + 1);
                _memoryCache.Set(Constants._failureType, failureType.ToString(), new MemoryCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3)
                });
                _memoryCache.Set(Constants._failureCount, 0);
            }

            _memoryCache.TryGetValue(Constants._failureCount, out int failureCount);
            if(failureCount == default || failureCount < 0)
            {
                failureCount = 0;
                _memoryCache.Set(Constants._failureCount, failureCount);
            }

            // short bust failures
            if(failureType == Constants.FailureTypes.ShortBursts)
            {
                if(++failureCount <= MAX_FAILURE_COUNT)
                {
                    _memoryCache.Set(Constants._failureCount, failureCount);
                    return new ObjectResult(null)
                    {
                        StatusCode = 503,
                    };
                }
                else
                {
                    _memoryCache.Set(Constants._failureCount, 0);
                    return new ObjectResult(true)
                    {
                        StatusCode = 200,
                    };
                }
            }
            // long noticable failure
            else
            {
                if (++failureCount <= MAX_FAILURE_COUNT)
                {
                    Thread.Sleep(TimeSpan.FromSeconds(30));
                    _memoryCache.Set(Constants._failureCount, failureCount);
                    return new ObjectResult(null)
                    {
                        StatusCode = 504,
                    };
                }
                else
                {
                    _memoryCache.Set(Constants._failureCount, 0);
                    return new ObjectResult(true)
                    {
                        StatusCode = 200,
                    };
                }
            }
        }

        [HttpGet]
        public ObjectResult Test()
        {
            return new OkObjectResult("OK");
        }
    }
}