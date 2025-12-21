using System.Net;
using System.Diagnostics;
using System.Text.Json;
using PhishSim.CLI.Commands;

namespace PhishSim.Server;

public class LocalServer
{
    private readonly string _templatePath;
    private readonly string _templateName;
    private HttpListener? _listener;
    private bool _running = true;

    public LocalServer(string templateName)
    {
        _templateName = templateName;
        _templatePath = Path.Combine(
            AppContext.BaseDirectory,
            "Templates",
            templateName
        );
    }

    public void Start()
    {
        if (!Directory.Exists(_templatePath))
        {
            Console.WriteLine("Template not found.");
            return;
        }

        _listener = new HttpListener();
        var prefix = $"http://127.0.0.1:{ServerConfig.Port}/";
        _listener.Prefixes.Add(prefix);
        _listener.Start();

        OpenBrowser(prefix);

        LaunchCommand.PrintServerLine($"Serving {_templateName} (localhost)");
        LaunchCommand.PrintServerLink("Open", prefix);
        LaunchCommand.PrintServerLine("Press CTRL+C to stop server");
        Console.WriteLine();
        Console.WriteLine("Waiting for victim to enter credentials...");
        Console.WriteLine();

        Console.CancelKeyPress += OnCancelKeyPress;

        while (_running)
        {
            try
            {
                var context = _listener.GetContext();

                if (context.Request.HttpMethod == "POST" &&
                    context.Request.Url!.AbsolutePath == "/demo")
                {
                    HandleDemo(context);
                    continue;
                }

                ServeFile(context);
            }
            catch (HttpListenerException)
            {
                break;
            }
        }

        Cleanup();
    }

    private void HandleDemo(HttpListenerContext context)
    {
        using var reader = new StreamReader(context.Request.InputStream);
        var body = reader.ReadToEnd();

        try
        {
            var json = JsonDocument.Parse(body).RootElement;

            var username = json.GetProperty("username").GetString() ?? "(empty)";
            var password = json.GetProperty("password").GetString() ?? "(empty)";

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[*] Victim login received");
            Console.ResetColor();

            Console.WriteLine($"    Username: {username}");
            Console.WriteLine($"    Password: {password}");
        }
        catch
        {
            Console.WriteLine("[!] Invalid demo payload");
        }

        context.Response.StatusCode = 200;
        context.Response.Close();
    }

    private static void OpenBrowser(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch { }
    }

    private void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
    {
        e.Cancel = true;
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("\n  [!] Stopping local server...");
        Console.ResetColor();

        _running = false;
        _listener?.Stop();
    }

    private void Cleanup()
    {
        _listener?.Close();
        Console.CancelKeyPress -= OnCancelKeyPress;

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("  [+] Server stopped.\n");
        Console.ResetColor();
    }

    private void ServeFile(HttpListenerContext context)
    {
        var relativePath = context.Request.Url!.AbsolutePath.Trim('/');

        if (string.IsNullOrEmpty(relativePath))
            relativePath = "index.html";

        var filePath = Path.Combine(_templatePath, relativePath);

        if (!File.Exists(filePath))
        {
            context.Response.StatusCode = 404;
            context.Response.Close();
            return;
        }

        var bytes = File.ReadAllBytes(filePath);
        context.Response.ContentType = GetContentType(filePath);
        context.Response.OutputStream.Write(bytes, 0, bytes.Length);
        context.Response.Close();
    }

    private static string GetContentType(string path) =>
        Path.GetExtension(path) switch
        {
            ".html" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".svg" => "image/svg+xml",
            _ => "application/octet-stream"
        };
}