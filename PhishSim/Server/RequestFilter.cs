using System.Net;

namespace PhishSim.Server;

public static class RequestFilter
{
    public static bool IsAllowed(HttpListenerRequest request)
    {
        return request.HttpMethod == "GET";
    }
}