using B2B.API.Attributes;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IO;
using System.Text;

namespace B2B.API.Middlewares
{
    /// <summary>
    /// 紀錄 Response Log 使用的 Middleware
    /// </summary>
    public class ResponseLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private readonly ILogger<ResponseLogMiddleware> _logger;

        public ResponseLogMiddleware(RequestDelegate next, ILogger<ResponseLogMiddleware> logger)
        {
            _next = next;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;
            await using var responseBody = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBody;

            // 流入 pipeline
            await _next(context);
            // 流出 pipeline

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBodyTxt = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);

            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            var attribute = endpoint?.Metadata.GetMetadata<ApiLogAttribute>();

            if (attribute != null)
            {
                // 須要紀錄 Log

                _logger.LogInformation("\n" +
                    $"LogId:{(string)context.Items["ApiLogId"]} \n" +
                    $"Schema:{context.Request.Scheme} \n" +
                    $"Host: {context.Request.Host.ToUriComponent()} \n" +
                    $"Path: {context.Request.Path} \n" +
                    $"QueryString: {context.Request.QueryString} \n" +
                    $"ResponseStatus: {context.Response.StatusCode}\n" +
                    $"ResponseHeader: {GetHeaders(context.Response.Headers)} \n" +
                    $"ResponseBody: {responseBodyTxt}\n");
            }
        }

        private static string GetHeaders(IHeaderDictionary headers)
        {
            var headerStr = new StringBuilder();
            foreach (var header in headers)
            {
                headerStr.Append($"{header.Key}: {header.Value}。");
            }

            return headerStr.ToString();
        }
    }

    /// <summary>
    /// 建立 Extension 將此 ResponseLogMiddleware 加入 HTTP pipeline
    /// </summary>
    public static class ResponseLogMiddlewareExtensions
    {
        public static IApplicationBuilder UseResponseLogMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ResponseLogMiddleware>();
        }
    }
}
