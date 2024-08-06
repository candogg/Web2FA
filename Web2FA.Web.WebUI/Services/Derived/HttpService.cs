using System.Net.Http.Headers;
using System.Text;
using Web2FA.Backend.Shared.Extensions;
using Web2FA.Backend.Shared.Services.Base;
using Web2FA.Backend.Shared.Services.Derived;

namespace Web2FA.Web.WebUI.Services.Derived
{
    public class HttpService<TResponse, TRequest> : ServiceSingularBase<HttpService<TResponse, TRequest>>
    {
        public async Task<TResponse?> PostAsync(TRequest requestItem, string url, CancellationToken continuationToken, string? authentication = null)
        {
            var resultItem = Activator.CreateInstance<TResponse>();

            if (requestItem == null) return resultItem;

            var body = requestItem.Serialize();

            if (body.IsNullOrEmpty()) return resultItem;

            try
            {
                var buffer = Encoding.UTF8.GetBytes(body);

                using var byteContent = new ByteArrayContent(buffer);

                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using var clientHandler = new HttpClientHandler();

                using var client = new HttpClient(clientHandler)
                {
                    Timeout = TimeSpan.FromSeconds(120)
                };

                if (authentication != null && authentication.IsNotNullOrEmpty())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authentication);
                }

                using var result = await client.PostAsync(url, byteContent, continuationToken);

                if (result == null) return default;

                var postResult = await result.Content.ReadAsStringAsync();

                if (postResult == null || postResult.IsNullOrEmpty()) return default;

                resultItem = postResult.Deserialize<TResponse>();

                return resultItem;
            }
            catch (Exception ex)
            {
                LogService.DerivedObject.Write(log: $"Error URL: {url}\n\nRequestBody: {body}\n\nException: {ex}", methodName: System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, exceptionId: 0);
            }

            return default;
        }

        public async Task<TResponse?> GetAsync(string url, CancellationToken continuationToken, string? authentication = null)
        {
            try
            {
                using var clientHandler = new HttpClientHandler();

                using var client = new HttpClient(clientHandler)
                {
                    Timeout = TimeSpan.FromSeconds(120)
                };

                if (authentication != null && authentication.IsNotNullOrEmpty())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authentication);
                }

                using var result = await client.GetAsync(url, continuationToken);

                if (result == null) return default;

                var getResult = await result.Content.ReadAsStringAsync();

                if (getResult == null || getResult.IsNullOrEmpty()) return default;

                var resultItem = Activator.CreateInstance<TResponse>();
                resultItem = getResult.Deserialize<TResponse>();

                return resultItem;
            }
            catch (Exception ex)
            {
                LogService.DerivedObject.Write(log: $"Error URL: {url}\n\nException: {ex}", methodName: System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Name, exceptionId: 0);
            }

            return default;
        }
    }
}
