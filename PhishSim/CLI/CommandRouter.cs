using PhishSim.CLI.Commands;

namespace PhishSim.CLI;

public class CommandRouter
{
    public bool Route(string[] args)
    {
        var cmd = args[0].ToLower();

        switch (cmd)
        {
            case "launch":
                new LaunchCommand().Execute();
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