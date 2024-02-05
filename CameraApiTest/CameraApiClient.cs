namespace CameraApiTest;

public sealed class CameraApiClient
{
    private readonly HttpClient httpClient = new();

    public async Task TryCameraApi()
    {
        await ReadApiMethod("http://192.168.1.230/cgi-bin/videoStatServer.cgi?action=getSummary");
        await ReadApiMethod("http://192.168.1.230/cgi-bin/videoStatServer.cgi?action=attach&heartbeat=5");
    }

    public async Task ReadApiMethod(string url)
    {
        var response = await GetResponseAsync(url);

        Console.WriteLine("Status Code: {0}", response.StatusCode);

        if (response.IsSuccessStatusCode)
        {
            using var body = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(body);

            while (!reader.EndOfStream)
                Console.WriteLine(reader.ReadLine());
        }
    }


    public async Task<HttpResponseMessage> GetResponseAsync(string actionUrl)
    {
        var url = new Uri(actionUrl);

        var unauthorizedMessage = await httpClient.GetAsync(url);

        if (unauthorizedMessage.IsSuccessStatusCode)
        {
            return unauthorizedMessage;
        }
        else
        {
            var authorization = AuthorizationHelper.GetAuthorizationHader(url,
                unauthorizedMessage.Headers.GetValues("WWW-Authenticate").First(),
                "00001");

            var request = new HttpRequestMessage(HttpMethod.Get, actionUrl);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Digest", authorization);
            request.Headers.Add("Cookie", "secure");

            Console.WriteLine("Send digest");

            return await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        }

    }

}
