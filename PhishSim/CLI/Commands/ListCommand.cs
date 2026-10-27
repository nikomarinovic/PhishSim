namespace PhishSim.CLI.Commands;

public class ListCommand
{
    public static List<string> Templates { get; private set; } = new();

    public static List<string> GetAvailableTemplates()
    {
        var templatesPath = Path.Combine(AppContext.BaseDirectory, "Templates");

        if (!Directory.Exists(templatesPath))
        {
            return new List<string>();
        }

        return Directory
            .GetDirectories(templatesPath)
            .Where(dir => File.Exists(Path.Combine(dir, "index.html")))
            .Select(Path.GetFileName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(name => name!)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public void Execute()
    {
        Templates = GetAvailableTemplates();

        if (Templates.Count == 0)
        {
            Console.WriteLine("No templates found.");
            return;
        }

        Console.WriteLine("\nAvailable templates:\n");

        for (int i = 0; i < Templates.Count; i++)
        {
            Console.WriteLine($"[{i + 1}] {Templates[i]}");
        }

        Console.WriteLine();
    }
}