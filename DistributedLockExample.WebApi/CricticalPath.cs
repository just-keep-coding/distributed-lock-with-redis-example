namespace DistributedLockExample.WebApi;

public static class CricticalPath
{
    public static Task RunAsync()
    {
        var ms = Random.Shared.Next(5000, 10000);
        return Task.Delay(ms);
    }
}
