using TTC_RubikChallenge.Cli;

Action? clearScreen = !Console.IsInputRedirected && !Console.IsOutputRedirected &&
    Environment.GetEnvironmentVariable("TERM") != "dumb" ? ClearScreen : null;

new CubeApplication(Console.In, Console.Out, TerminalColors.TryEnable(), clearScreen).Run();

static void ClearScreen()
{
    try
    {
        Console.Clear();
    }
    catch (IOException)
    {
        // Hosts without screen control can still append readable cube output.
    }
}
