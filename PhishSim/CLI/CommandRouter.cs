using PhishSim.CLI.Commands;

namespace PhishSim.CLI;

public class CommandRouter
{
    public bool Route(string[] args)
    {
        var cmd = args[0].ToLowerInvariant();

        switch (cmd)
        {
            case "launch":
                new LaunchCommand().Execute(args.Length > 1 ? string.Join(' ', args.Skip(1)) : null);
                break;

            case "list":
            case "ls":
                new ListCommand().Execute();
                break;

            case "help":
            case "-h":
                ConsoleUI.PrintHelp();
                break;

            case "quit":
            case "-q":
            case "exit":
                return false;

            default:
                Console.WriteLine("Unknown command. Use -h for help.");
                break;
        }

        return true;
    }
}