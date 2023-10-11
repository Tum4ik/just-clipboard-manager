using System.Net.Http;

namespace Tum4ik.JustClipboardManager.Services;
internal class HttpClientFactory : IHttpClientFactory
{
  public HttpClient CreateHttpClient()
  {
    return new HttpClient();
  }
}
