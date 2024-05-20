namespace CameraApiTest;

public sealed class CameraApiClient
{
    private readonly HttpClient httpClient = new();

    public async Task TryCameraApi()
    {
        _ = Task.Run(async () =>
            // כמדומני שזו המתודה שמחזירה נתונים, בכל תזוזה של אדם
            await ReadApiMethod("videoStatServer.cgi?action=attach&heartbeat=5")
        );

        //await Task.Delay(TimeSpan.FromSeconds(10));
        // "לא עובד בדגם שלנו"
        await ReadApiMethod("videoStatServer.cgi?action=getSummary&channel=1");

        // אינני יודע בדיוק מה זה, לכאורה זה אמור להגיב לטריגרים שהוגדרו
        await ReadApiMethod("eventManager.cgi?action=attach&heartbeat=20&channel=1&codes=[StayDetection,ManNumDetection,CrowdDetection]");

        //await ReadApiMethod("devVideoAnalyse.cgi?action=getcaps&channel=1");
    }

    public async Task ReadApiMethod(string url)
    {
        var response = await GetAuthorizedResponseAsync("http://192.168.1.230/cgi-bin/" + url);

        Console.WriteLine("Status Code: {0}", response.StatusCode);

        if (response.IsSuccessStatusCode)
        {
            using var body = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(body);

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                Console.WriteLine(line);
            }
        }
    }


    public async Task<HttpResponseMessage> GetAuthorizedResponseAsync(string actionUrl)
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
