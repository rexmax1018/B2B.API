using B2B.API.Attributes;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IO;
using System.Text;

namespace B2B.API.Middlewares
{
    /// <summary>
    /// 紀錄 Request Log 使用的 Middleware
    /// </summary>
    public class RequestLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private readonly ILogger _logger;

        public RequestLogMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
            _logger = loggerFactory.CreateLogger<RequestLogMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            var attribute = endpoint?.Metadata.GetMetadata<ApiLogAttribute>();

            if (attribute == null)
            {
                // 無須紀錄 Log

                await _next(context);
            }
            else
            {
                // 須要紀錄 Log

                context.Request.EnableBuffering();
                await using var requestStream = _recyclableMemoryStreamManager.GetStream();
                await context.Request.Body.CopyToAsync(requestStream);

                // 產生唯一的 LogId 串起 Request & Response 兩筆 log 資料
                context.Items["ApiLogId"] = GetLogId();

                // 保存 Log 資訊
                _logger.LogInformation(
                    $"LogId:{(string)context.Items["ApiLogId"]} " +
                    $"Schema:{context.Request.Scheme} " +
                    $"Host: {context.Request.Host.ToUriComponent()} " +
                    $"Path: {context.Request.Path} " +
                    $"QueryString: {context.Request.QueryString} " +
                    $"RequestHeader: {GetHeaders(context.Request.Headers)} " +
                    $"RequestBody: {ReadStreamInChunks(requestStream)}");

                context.Request.Body.Position = 0;
                await _next(context);
            }
        }

        private static string GetLogId()
        {
            // DateTime(yyyyMMddhhmmssfff) + 1-UpperCase + 2-Digits

            var random = new Random();
            var idBuild = new StringBuilder();
            idBuild.Append(DateTime.Now.ToString("yyyyMMddhhmmssfff"));
            idBuild.Append((char)random.Next('A', 'A' + 26));
            idBuild.Append(random.Next(10, 99));
            return idBuild.ToString();
        }

        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;
            stream.Seek(0, SeekOrigin.Begin);
            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);
            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;
            do
            {
                readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);
            return textWriter.ToString();
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
    /// 建立 Extension 將此 RequestLogMiddleware 加入 HTTP pipeline
    /// </summary>
    public static class RequestLogMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLogMiddleware>();
        }
    }
}
