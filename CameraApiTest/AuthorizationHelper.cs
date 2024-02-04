using System.Security.Cryptography;
using System.Text;

namespace CameraApiTest;

public static class AuthorizationHelper
{
    public static string GetAuthorizationHader(Uri url, string wwwAuthenticateHeader, string nonceCount)
    {
        var challengeValues = ParseDigestAuthenticationHeader(wwwAuthenticateHeader);
        string nonce = challengeValues["nonce"];
        string opaque = challengeValues["opaque"];

        return GetAuthorizationHader(url, nonce, opaque, nonceCount);
    }

    public static string GetAuthorizationHader(Uri url, string nonce, string opaque, string nonceCount)
    {
        string uri = url.PathAndQuery;

        const string username = "admin";
        const string realm = "Login to 5def96b9ee267a0c743c237c0a10ab3b";
        const string algorithm = "MD5";
        const string qop = "auth";
        const string cnonce = "21312";

        string responseValue = GetResponseValue(url, nonce, opaque, nonceCount);

        return $"username=\"{username}\", realm=\"{realm}\", nonce=\"{nonce}\", uri=\"{uri}\", algorithm=\"{algorithm}\", qop={qop}, nc={nonceCount}, cnonce=\"{cnonce}\", response=\"{responseValue}\", opaque=\"{opaque}\"";
    }

    public static string GetResponseValue(Uri url, string nonce, string opaque, string nonceCount)
    {
        string uri = url.PathAndQuery;

        const string username = "admin";
        const string password = "admin123";
        const string realm = "Login to 5def96b9ee267a0c743c237c0a10ab3b";
        const string qop = "auth";

        const string method = "GET";
        const string cnonce = "21312";

        string ha1 = $"{username}:{realm}:{password}".ToMD5Hash();

        string ha2 = $"{method}:{uri}".ToMD5Hash();

        return $"{ha1}:{nonce}:{nonceCount}:{cnonce}:{qop}:{ha2}".ToMD5Hash();
    }

    public static string ToMD5Hash(this byte[] bytes)
    {
        StringBuilder hash = new();
        MD5.HashData(bytes).ToList()
                      .ForEach(b => hash.AppendFormat("{0:x2}", b));

        return hash.ToString();
    }

    public static string ToMD5Hash(this string inputString)
    {
        return Encoding.UTF8.GetBytes(inputString).ToMD5Hash();
    }

    static Dictionary<string, string> ParseDigestAuthenticationHeader(string header)
    {
        var values = new Dictionary<string, string>();
        foreach (var part in header.Split(','))
        {
            var kvp = part.Split('=');
            values[kvp[0].Trim()] = kvp[1].Replace("\"", "").Trim();
        }

        return values;
    }
}
