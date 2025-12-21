namespace PhishSim.CLI;

public static class ConsoleUI
{
    public static void PrintBanner()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(@"
 ██████╗ ██╗  ██╗██╗███████╗██╗  ██╗███████╗██╗███╗   ███╗
 ██╔══██╗██║  ██║██║██╔════╝██║  ██║██╔════╝██║████╗ ████║
 ██████╔╝███████║██║███████╗███████║███████╗██║██╔████╔██║
 ██╔═══╝ ██╔══██║██║╚════██║██╔══██║╚════██║██║██║╚██╔╝██║
 ██║     ██║  ██║██║███████║██║  ██║███████║██║██║ ╚═╝ ██║
 ╚═╝     ╚═╝  ╚═╝╚═╝╚══════╝╚═╝  ╚═╝╚══════╝╚═╝╚═╝     ╚═╝
");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine("** Phishing Tool - Modified By github.com/Nmarino8 **");
        Console.ResetColor();
        Console.WriteLine();
        
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Red; 
        Console.WriteLine(
            ":: This tool is intended for educational and security training purposes only.\n" +
            ":: The developers assume no liability and are not responsible for any misuse\n" +
            ":: or damage caused by PhishSim.\n" +
            "\n:: Unauthorized testing or attacking systems without explicit permission is illegal.\n"
        );
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("\nType 'help' or use -h to view available commands.\n");
        Console.ResetColor();
    }

    public static void PrintHelp()
    {
        Console.WriteLine(@"
Usage:

  launch        Start interactive simulation
  help, -h      Show help menu
  quit, -q      Exit PhishSim
");
    }
}