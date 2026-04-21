using System.Text.Json;
using ConsoleApp1.Models;

namespace ConsoleApp1.Services;

public class InboxWatcher : IDisposable
{
    private readonly string _inboxPath;
    private readonly string _processedPath;
    private readonly string _failedPath;
    private readonly OrderPipeline _pipeline;
    private readonly FileSystemWatcher _watcher;
    private readonly SemaphoreSlim _semaphore = new(2);

    public InboxWatcher(string inboxPath, OrderPipeline pipeline)
    {
        _inboxPath = inboxPath;
        _pipeline = pipeline;
        _processedPath = Path.Combine(inboxPath, "processed");
        _failedPath = Path.Combine(inboxPath, "failed");
        
        Directory.CreateDirectory(_inboxPath);
        Directory.CreateDirectory(_processedPath);
        Directory.CreateDirectory(_failedPath);

        _watcher = new FileSystemWatcher(_inboxPath, "*.json")
        {
            EnableRaisingEvents = true,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
        };
        
        _watcher.Created += async (s, e) => await OnCreatedAsync(e.FullPath);
    }

    private async Task OnCreatedAsync(string fullPath)
    {
        await _semaphore.WaitAsync();
        try
        {
            Console.WriteLine($"[WATCHER] Wykryto nowy plik: {Path.GetFileName(fullPath)}");

            byte retryCount = 0;
            while (retryCount < 5)
            {
                try
                {
                    await Task.Delay(300);

                    await using FileStream stream = File.OpenRead(fullPath);
                    var orders = await JsonSerializer.DeserializeAsync<List<Order>>(stream, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (orders != null)
                    {
                        foreach (var order in orders)
                        {
                            _pipeline.ProcessOrder(order);
                        }
                    }

                    stream.Close();
                    string destFile = Path.Combine(_processedPath, Path.GetFileName(fullPath));
                    if (File.Exists(destFile)) File.Delete(destFile);
                    File.Move(fullPath, destFile);
                    Console.WriteLine($"[WATCHER] Sukces: Przeniesiono {Path.GetFileName(fullPath)} do /processed");
                    break;
                }
                catch (IOException)
                {
                    retryCount++;
                    await Task.Delay(200);
                }
                catch (Exception ex)
                {
                    string fileName = Path.GetFileName(fullPath);
                    File.Move(fullPath, Path.Combine(_failedPath, fileName), true);
                    await File.WriteAllTextAsync(Path.Combine(_failedPath, fileName + ".error.txt"), ex.Message);
                    Console.WriteLine($"[WATCHER] BŁĄD: Plik {fileName} trafił do /failed. Info: {ex.Message}");
                    break;
                }
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        _watcher.Dispose();
        _semaphore.Dispose();
        GC.SuppressFinalize(this);
    }
    
}