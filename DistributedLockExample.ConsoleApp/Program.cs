using var httpClient = new HttpClient();

var tasks = new List<Task<HttpResponseMessage>>();

for (var i = 0; i < 3; i++)
{
    await Task.Delay(200);
    var task = httpClient.GetAsync($"http://localhost:5119/locks?requestId={i}");
    tasks.Add(task);
}

await foreach (var task in Task.WhenEach(tasks))
{
    var requestId = await task.Result.Content.ReadAsStringAsync();
    Console.WriteLine($"Http status {task.Result.StatusCode} for request {requestId}.");
}
