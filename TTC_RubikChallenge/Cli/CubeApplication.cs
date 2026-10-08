using TTC_RubikChallenge.Domain;

namespace TTC_RubikChallenge.Cli;

public sealed class CubeApplication(
    TextReader input, TextWriter output, bool useColor = false, Action? clearScreen = null)
{
    public const string Help = """
        Moves: <face> <direction> (e.g. front >, right <).
        Directions: > clockwise, < counterclockwise.
        Faces: top, bottom, front, back, left, right (up/down also accepted).
        Clockwise is viewed directly from outside the selected face.
        Command: reset (restore the solved cube).
        To exit on Windows: Ctrl+Z, then Enter.
        """;

    public void Run()
    {
        var cube = new Cube();
        output.WriteLine(Help);
        Show(cube, refresh: false);
        while (true)
        {
            output.Write("> ");
            var line = input.ReadLine();
            if (line is null) return;
            if (line.Trim().Equals("reset", StringComparison.OrdinalIgnoreCase))
            {
                cube = new Cube();
                Show(cube);
            }
            else if (MoveParser.TryParse(line, out var face, out var direction, out var error))
            {
                cube.Rotate(face, direction);
                Show(cube);
            }
            else output.WriteLine($"Invalid command. {error}");
        }
    }

    private void Show(Cube cube, bool refresh = true)
    {
        if (refresh && clearScreen is not null)
        {
            clearScreen();
            output.WriteLine(Help);
        }
        output.Write(CubeConsoleRenderer.Render(cube, useColor));
    }
}
