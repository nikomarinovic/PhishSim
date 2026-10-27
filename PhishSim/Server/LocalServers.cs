using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.ComponentModel;
using PhishSim.CLI.Commands;

namespace PhishSim.Server;

public class LocalServer
{
    private const int MaxRequestBodyBytes = 1024;
    private readonly string _templatePath;
    private readonly string _templateRoot;
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
        _templateRoot = Path.GetFullPath(_templatePath) + Path.DirectorySeparatorChar;
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

        try
        {
            _listener.Start();
        }
        catch (HttpListenerException ex)
        {
            Console.Error.WriteLine($"Unable to start the local server on port {ServerConfig.Port}: {ex.Message}");
            _listener.Close();
            return;
        }

        Console.CancelKeyPress += OnCancelKeyPress;

        try
        {
            OpenBrowser(prefix);

            LaunchCommand.PrintServerLine($"Serving {_templateName} on localhost");
            LaunchCommand.PrintServerLink("Open", prefix);
            LaunchCommand.PrintServerLine("Only form-submission events are reported; entered values are never sent.");
            LaunchCommand.PrintServerLine("Press CTRL+C to stop server");
            Console.WriteLine();

            while (_running)
            {
                try
                {
                    var context = _listener.GetContext();
                    try
                    {
                        HandleRequest(context);
                    }
                    catch (ObjectDisposedException)
                    {
                        Console.Error.WriteLine("The client closed a request before it could be handled.");
                    }
                    catch (HttpListenerException ex)
                    {
                        Console.Error.WriteLine($"Unable to complete an HTTP request: {ex.Message}");
                    }
                    catch (IOException ex)
                    {
                        Console.Error.WriteLine($"Unable to complete an HTTP request: {ex.Message}");
                    }
                }
                catch (HttpListenerException) when (!_running)
                {
                    break;
                }
                catch (HttpListenerException ex)
                {
                    Console.Error.WriteLine($"The local server stopped unexpectedly: {ex.Message}");
                    break;
                }
            }
        }
        finally
        {
            Cleanup();
        }
    }

    private void HandleRequest(HttpListenerContext context)
    {
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["Cache-Control"] = "no-store";

        if (context.Request.HttpMethod == "POST" &&
            context.Request.Url?.AbsolutePath == "/demo")
        {
            HandleDemo(context);
            return;
        }

        if (RequestFilter.IsAllowed(context.Request))
        {
            ServeFile(context);
            return;
        }

        context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
        context.Response.Close();
    }

    private void HandleDemo(HttpListenerContext context)
    {
        if (context.Request.ContentType?.StartsWith(
                "application/json",
                StringComparison.OrdinalIgnoreCase) != true)
        {
            context.Response.StatusCode = (int)HttpStatusCode.UnsupportedMediaType;
            context.Response.Close();
            return;
        }

        if (context.Request.ContentLength64 > MaxRequestBodyBytes)
        {
            context.Response.StatusCode = (int)HttpStatusCode.RequestEntityTooLarge;
            context.Response.Close();
            return;
        }

        using var body = new MemoryStream();
        var buffer = new byte[256];
        int bytesRead;
        while ((bytesRead = context.Request.InputStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            if (body.Length + bytesRead > MaxRequestBodyBytes)
            {
                context.Response.StatusCode = (int)HttpStatusCode.RequestEntityTooLarge;
                context.Response.Close();
                return;
            }

            body.Write(buffer, 0, bytesRead);
        }

        try
        {
            using var json = JsonDocument.Parse(body.ToArray());
            if (json.RootElement.ValueKind != JsonValueKind.Object ||
                json.RootElement.EnumerateObject().Count() != 1 ||
                !json.RootElement.TryGetProperty("event", out var eventName) ||
                eventName.ValueKind != JsonValueKind.String ||
                eventName.GetString() != "form-submitted")
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.Close();
                return;
            }
        }
        catch (JsonException)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.Close();
            return;
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("[*] SIMULATION EVENT RECEIVED");
        Console.ResetColor();
        Console.WriteLine($"    Template: {_templateName}");
        Console.WriteLine("    Event: Login form submitted");
        Console.WriteLine($"    Time: {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss zzz}");
        Console.WriteLine("    Entered values: not transmitted");

        context.Response.StatusCode = (int)HttpStatusCode.NoContent;
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
        catch (Win32Exception ex)
        {
            Console.Error.WriteLine($"Could not open a browser automatically: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.Error.WriteLine($"Could not open a browser automatically: {ex.Message}");
        }
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
        if (!TryResolveFilePath(context.Request.Url?.AbsolutePath ?? "/", out var filePath))
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.Response.Close();
            return;
        }

        try
        {
            if (!File.Exists(filePath))
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Response.Close();
                return;
            }

            var bytes = File.ReadAllBytes(filePath);
            context.Response.ContentType = GetContentType(filePath);
            context.Response.ContentLength64 = bytes.Length;
            context.Response.OutputStream.Write(bytes, 0, bytes.Length);
            context.Response.Close();
        }
        catch (IOException ex)
        {
            Console.Error.WriteLine($"Unable to serve requested template file: {ex.Message}");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.Close();
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.Error.WriteLine($"Unable to access requested template file: {ex.Message}");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.Close();
        }
    }

    private bool TryResolveFilePath(string requestPath, out string filePath)
    {
        filePath = string.Empty;

        try
        {
            var relativePath = Uri.UnescapeDataString(requestPath).Trim('/');
            if (string.IsNullOrEmpty(relativePath))
                relativePath = "index.html";

            var candidate = Path.GetFullPath(Path.Combine(_templatePath, relativePath));
            var pathComparison = OperatingSystem.IsWindows()
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;
            if (!candidate.StartsWith(_templateRoot, pathComparison))
                return false;

            filePath = candidate;
            return true;
        }
        catch (UriFormatException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }
        catch (NotSupportedException)
        {
            return false;
        }
        catch (PathTooLongException)
        {
            return false;
        }
    }

    private static string GetContentType(string path) =>
        Path.GetExtension(path) switch
        {
            ".html" => "text/html; charset=utf-8",
            ".css" => "text/css; charset=utf-8",
            ".js" => "application/javascript; charset=utf-8",
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".svg" => "image/svg+xml; charset=utf-8",
            _ => "application/octet-stream"
        };
}