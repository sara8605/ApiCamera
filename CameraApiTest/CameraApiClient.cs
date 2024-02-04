using Microsoft.AspNetCore.WebUtilities;

namespace CameraApiTest;

public sealed class CameraApiClient
{
    private readonly HttpClient httpClient = new HttpClient();

    public async Task OpenStream()
    {
        var url = "http://192.168.1.230/cgi-bin/videoStatServer.cgi?action=attach&channel=1&heartbeat=60";

        HttpResponseMessage? countPass = await GetResponseAsync(url);

        Console.WriteLine("Status Code: {0}", countPass.StatusCode);

        if (countPass.IsSuccessStatusCode)
        {
            var stream = await countPass.Content.ReadAsStreamAsync();

            // Use MultipartReader to read the content of the response
            string? boundary = countPass.Content.Headers.ContentType.Parameters.First().Value;
            var reader = new MultipartReader(boundary, stream);

            // Read parts in a loop
            while (true)
            {
                var section = await reader.ReadNextSectionAsync();
                if (section == null) break;  // No more sections

                // Process the section's content
                var content = await new StreamReader(section.Body).ReadToEndAsync();
                Console.WriteLine("Content: {0}", content);
            }

            Console.WriteLine("Steam Stopped");
        }
    }


    public async Task<HttpResponseMessage?> GetResponseAsync(string actionUrl)
    {
        var url = new Uri(actionUrl);

        var unauthorizedMessage = await httpClient.GetAsync(url);

        if (unauthorizedMessage.IsSuccessStatusCode)
        {
            return unauthorizedMessage;
        }
        else
        {
            await Task.Delay(30000);
            var authorization = AuthorizationHelper.GetAuthorizationHader(url, 
                unauthorizedMessage.Headers.GetValues("WWW-Authenticate").First(),
                "000000011");

            var request = new HttpRequestMessage(HttpMethod.Get, actionUrl);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Digest", authorization);
            request.Headers.Add("Cookie", "secure");

            return await httpClient.SendAsync(request);
        }

    }

}
