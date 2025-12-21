namespace PhishSim.CLI.Commands;

public class ListCommand
{
    public static List<string> Templates { get; private set; } = new();

    public void Execute()
    {
        var templatesPath = Path.Combine(AppContext.BaseDirectory, "Templates");

        if (!Directory.Exists(templatesPath))
        {
            Console.WriteLine("No templates found.");
            return;
        }

        Templates = Directory
            .GetDirectories(templatesPath)
            .Select(Path.GetFileName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToList();

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