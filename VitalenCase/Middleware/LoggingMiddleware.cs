using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using VitalenCase.Services;

namespace VitalenCase.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private LogServices _apiLogService;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, LogServices apiLogService)
        {
            try
            {
                _apiLogService = apiLogService;

                var request = httpContext.Request;

                var stopWatch = Stopwatch.StartNew();
                var requestTime = DateTime.UtcNow;
                var requestBodyContent = await ReadRequestBody(request);
                var originalBodyStream = httpContext.Response.Body;
                using var responseBody = new MemoryStream();
                var response = httpContext.Response;
                response.Body = responseBody;
                await _next(httpContext);
                stopWatch.Stop();

                string responseBodyContent = null;
                responseBodyContent = await ReadResponseBody(response);
                await responseBody.CopyToAsync(originalBodyStream);

                await SafeLog(requestTime,
                    stopWatch.ElapsedMilliseconds,
                    response.StatusCode,
                    (int)await ReadUserId(request.Headers["Authorization"]),
                    request.Method,
                    request.Path,
                    requestBodyContent,
                    responseBodyContent);

            }
            catch (Exception)
            {
                await _next(httpContext);
            }
        }

        private static async Task<int?> ReadUserId(string token)
        {
            if (String.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadJwtToken(token.Split().Last());

                var userId = tokenS.Claims.First(claim => claim.Type == "userID").Value;

                return Convert.ToInt32(userId);
            }
            catch
            {
                return null;
            }
        }

        private static async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.EnableBuffering();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        private static async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var bodyAsText = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        private async Task SafeLog(DateTime requestTime,
                            long responseMillis,
                            int statusCode,
                            int userId,
                            string method,
                            string path,
                            string requestBody,
                            string responseBody)
        {
            if (method == "GET")
            {
                requestBody = "";
            }

            await _apiLogService.Log(new Models.Logging
            {
                RequestTime = requestTime,
                ResponseMillis = responseMillis,
                StatusCode = statusCode,
                UserId = userId,
                Method = method,
                Path = path,
                RequestBody = requestBody,
                ResponseBody = responseBody
            });
        }

    }
}
