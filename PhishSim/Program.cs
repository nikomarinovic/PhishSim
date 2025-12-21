using PhishSim.CLI;

ConsoleUI.PrintBanner();

var router = new CommandRouter();

while (true)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("phishsim > ");
    Console.ResetColor();
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
        continue;

    var commandArgs = input
        .Split(' ', StringSplitOptions.RemoveEmptyEntries);

    // Route command — false means quit
    if (!router.Route(commandArgs))
        break;
}