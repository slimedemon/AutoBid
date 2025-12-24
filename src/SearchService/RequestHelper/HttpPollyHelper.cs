using System;
using System.Net;
using Polly;
using Polly.Extensions.Http;

namespace SearchService.RequestHelper;

public class HttpPollyHelper
{
    public static IAsyncPolicy<HttpResponseMessage> GetAsyncPolicy()
    => HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));
}