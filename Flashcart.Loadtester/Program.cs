using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;

const int totalRequests = 100;

const string url =
    "http://localhost:5011/api/inventory/reserve";

const string token =
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiU3JpcmFtIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZW1haWxhZGRyZXNzIjoic3JpcmFtQGV4YW1wbGUuY29tIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQ3VzdG9tZXIiLCJleHAiOjE3ODkzMDg5MzcsImlzcyI6IkZsYXNoQ2FydC5BUEkiLCJhdWQiOiJGbGFzaENhcnQuQ2xpZW50In0.MXA-t3ChDzfLrzNgopkvewQ4_AMRx5cdocPHket_RIE";

var httpClient = new HttpClient();

httpClient.DefaultRequestHeaders.Authorization =
    new System.Net.Http.Headers.AuthenticationHeaderValue(
        "Bearer",
        token);

var stopwatch = Stopwatch.StartNew();

var tasks = Enumerable.Range(1, totalRequests)
    .Select(async requestNumber =>
    {
        var request = new
        {
            productId = 2,
            quantity = 1
        };

        try
        {
            var response =
                await httpClient.PostAsJsonAsync(
                    url,
                    request);

            var responseBody =
                await response.Content.ReadAsStringAsync();

            return new RequestResult
            {
                RequestNumber = requestNumber,
                StatusCode = response.StatusCode,
                ResponseBody = responseBody
            };
        }
        catch (Exception ex)
        {
            return new RequestResult
            {
                RequestNumber = requestNumber,
                StatusCode = 0,
                ResponseBody = ex.Message
            };
        }
    })
    .ToList();

var results = await Task.WhenAll(tasks);

stopwatch.Stop();

Console.WriteLine();
Console.WriteLine("========== LOAD TEST RESULT ==========");

Console.WriteLine($"Total Requests : {totalRequests}");
Console.WriteLine(
    $"Total Time     : {stopwatch.ElapsedMilliseconds} ms");

Console.WriteLine(
    $"Requests/sec   : " +
    $"{totalRequests / stopwatch.Elapsed.TotalSeconds:F2}");

Console.WriteLine();
Console.WriteLine("Status Codes:");

var statusGroups = results
    .GroupBy(r => (int)r.StatusCode)
    .OrderBy(g => g.Key);

foreach (var group in statusGroups)
{
    Console.WriteLine(
        $"{group.Key} : {group.Count()} requests");
}

Console.WriteLine("======================================");

public class RequestResult
{
    public int RequestNumber { get; set; }

    public HttpStatusCode StatusCode { get; set; }

    public string ResponseBody { get; set; } = string.Empty;
}