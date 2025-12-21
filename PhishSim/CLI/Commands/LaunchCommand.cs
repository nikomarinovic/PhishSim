using PhishSim.Server;
using PhishSim.Utils;

namespace PhishSim.CLI.Commands;

public class LaunchCommand
{
    public void Execute()
    {
        if (!SelectLocalhost())
            return;

        SelectTemplate();
    }

    private bool SelectLocalhost()
    {
        Console.WriteLine();
        PrintSection("Select server type");

        PrintOption(1, "Localhost");

        Console.WriteLine();
        PrintPrompt("Select an option: ");
        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input) || input == "1")
        {
            LoadLocalhost();
            return true;
        }
        Console.WriteLine();
        PrintError("Invalid selection.");
        Console.WriteLine();
        return false;
    }

    private void LoadLocalhost()
    {
        Console.WriteLine();
        TerminalAnimation.Spinner("Initializing localhost");
        TerminalAnimation.Spinner("Binding to 127.0.0.1");
        TerminalAnimation.Spinner("Checking port availability");
        PrintStatus("Localhost ready");
        Console.WriteLine();
    }

    // ================= TEMPLATE SELECTION =================

    private void SelectTemplate()
    {
        var templatesPath = Path.Combine(AppContext.BaseDirectory, "Templates");

        if (!Directory.Exists(templatesPath))
        {
            PrintError("Templates directory not found.");
            return;
        }

        var templates = Directory
            .GetDirectories(templatesPath)
            .Where(dir => File.Exists(Path.Combine(dir, "index.html")))
            .Select(Path.GetFileName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToList();

        if (templates.Count == 0)
        {
            PrintError("No templates available.");
            return;
        }

        PrintSection("Available templates");

        for (int i = 0; i < templates.Count; i++)
            PrintOption(i + 1, templates[i]!);

        Console.WriteLine();
        PrintPrompt("Select a template: ");
        var selection = Console.ReadLine();

        if (!int.TryParse(selection, out int index) ||
            index < 1 || index > templates.Count)
        {
            Console.WriteLine();
            PrintError("Invalid template selection.");
            Console.WriteLine();
            return;
        }

        var chosenTemplate = templates[index - 1]!;
        Console.WriteLine();
        PrintStatus($"Launching {chosenTemplate} on localhost");
        Console.WriteLine();
        TerminalAnimation.Spinner("Starting local server");
        TerminalAnimation.Spinner("Deploying static content");

        var server = new LocalServer(chosenTemplate);
        server.Start();
    }

    // ================= VISUAL HELPERS =================

    private static void PrintSection(string text)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine(text);
        Console.ResetColor();
        Console.WriteLine();
    }

    private static void PrintOption(int index, string label)
    {
        Console.Write("  ");

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("[");
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write(index);
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("] ");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(label);
        Console.ResetColor();
    }

    private static void PrintPrompt(string text)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(text);
        Console.ResetColor();
    }

    private static void PrintStatus(string message)
    {
        Console.Write("  ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("[+] ");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    // ================= SERVER LINE HELPERS =================

    public static void PrintServerLine(string message)
    {
        Console.Write("  ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("[+] ");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void PrintServerLink(string label, string url)
    {
        Console.Write("  ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("[+] ");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.Write(label + " ");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(url);
        Console.ResetColor();
    }

    private static void PrintError(string message)
    {
        Console.Write("  ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("[!] " + message);
        Console.ResetColor();
    }
}