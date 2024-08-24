using IndexProfitAPI.Conrollers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace IndexProfitAPI.Cache
{
    public class CacheFilter : Attribute,IAsyncResourceFilter
    {
        // 使用内存缓存
        private MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
        private readonly string _cacheKeyPrefix = "cache_key_";
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            string cacheKey = GetCacheKey(context.HttpContext);
            object cacheKeyValue = new object();
            // 尝试从缓存中获取数据
            bool hasKey = _cache.TryGetValue(cacheKey, out cacheKeyValue);
            if (hasKey && cacheKeyValue != null)
            {
                context.Result = cacheKeyValue as IActionResult;
            }
            else
            {
                // 执行管道的其余部分
                var resultContext = await next.Invoke();
                if (resultContext.Result is ObjectResult objectResult && objectResult.StatusCode == StatusCodes.Status200OK)
                {
                    // 设置缓存，带有过期时间
                    _cache.Set(cacheKey, resultContext.Result, TimeSpan.FromMinutes(10));
                }
                context.Result = resultContext.Result;
            }
            await Task.CompletedTask;
        }
        private string GetCacheKey(HttpContext httpContext)
        {
            // 根据请求构建一个唯一的缓存键
            return $"{_cacheKeyPrefix}{httpContext.Request.Method}_{httpContext.Request.Path}_{httpContext.Request.QueryString}";
        }
    }
}
