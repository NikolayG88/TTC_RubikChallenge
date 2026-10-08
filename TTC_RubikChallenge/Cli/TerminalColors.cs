using System.Runtime.InteropServices;

namespace TTC_RubikChallenge.Cli;

internal static class TerminalColors
{
    internal static bool TryEnable()
    {
        if (Console.IsOutputRedirected ||
            !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("NO_COLOR")) ||
            Environment.GetEnvironmentVariable("TERM") == "dumb")
            return false;

        if (!OperatingSystem.IsWindows()) return true;

        // Windows console hosts require virtual-terminal processing for ANSI colors.
        const int standardOutputHandle = -11;
        const uint enableVirtualTerminalProcessing = 0x0004;
        var handle = GetStdHandle(standardOutputHandle);
        return GetConsoleMode(handle, out var mode) &&
            SetConsoleMode(handle, mode | enableVirtualTerminalProcessing);
    }

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetStdHandle(int standardHandle);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetConsoleMode(IntPtr handle, out uint mode);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetConsoleMode(IntPtr handle, uint mode);
}
