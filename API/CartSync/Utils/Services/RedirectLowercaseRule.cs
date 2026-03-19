using System.Net;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Net.Http.Headers;

namespace CartSync.Utils.Services;

public class RedirectLowerCaseRule : IRule
{
    private static int StatusCode => (int)HttpStatusCode.MovedPermanently;

    public void ApplyRule(RewriteContext context)
    {
        HttpRequest request = context.HttpContext.Request;
        PathString path = context.HttpContext.Request.Path;
        HostString host = context.HttpContext.Request.Host;

        if (path.HasValue && path.Value.Any(char.IsUpper) || host.HasValue && host.Value.Any(char.IsUpper))
        {
            HttpResponse response = context.HttpContext.Response;
            response.StatusCode = StatusCode;
            response.Headers[HeaderNames.Location] = (request.Scheme + "://" + host.Value + request.PathBase.Value + request.Path.Value).ToLower() + request.QueryString;
            context.Result = RuleResult.EndResponse; 
        }
        else
        {
            context.Result = RuleResult.ContinueRules;
        } 
    }
}