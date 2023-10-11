using System.Net.Http;

namespace Tum4ik.JustClipboardManager.Services;
internal interface IHttpClientFactory
{
  HttpClient CreateHttpClient();
}
