using TTC_RubikChallenge.Cli;
using TTC_RubikChallenge.Domain;

namespace TTC_RubikChallenge.Tests;

public sealed class CliTests
{
    private const string AcceptanceCommands = "front >\nright <\ntop >\nback <\nleft >\nbottom <\n";

    // Fixed expected output from the challenge PDF, independent of rotation code.
    private static string AcceptanceOutput => string.Join(Environment.NewLine,
        "          Top",
        "          R O G",
        "          B W W",
        "          B B B",
        "Left      Front     Right     Back",
        "G Y Y     O R R     Y B O     Y B W",
        "O O G     O G W     R R W     O B Y",
        "B G O     W W W     O Y R     Y Y W",
        "          Bottom",
        "          G G B",
        "          R Y R",
        "          R G G",
        "W White | Y Yellow | G Green | B Blue | R Red | O Orange", "");

    [Fact]
    public void AcceptanceCommandsPrintTheRequiredResultAndExitAtEof()
    {
        var transcript = Run(AcceptanceCommands);
        Assert.EndsWith(AcceptanceOutput + "> ", transcript);
        Assert.DoesNotContain("Invalid command.", transcript);
        Assert.DoesNotContain("\u001b", transcript, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(" UP  > ", FacePosition.Top, RotationDirection.Clockwise)]
    [InlineData("DOWN\t<", FacePosition.Bottom, RotationDirection.CounterClockwise)]
    public void ParserAcceptsDirectionsAliasesCaseAndWhitespace(
        string input, FacePosition face, RotationDirection direction)
    {
        Assert.True(MoveParser.TryParse(input, out var parsedFace, out var parsedDirection, out var error));
        Assert.Equal(face, parsedFace);
        Assert.Equal(direction, parsedDirection);
        Assert.Null(error);
    }

    [Theory]
    [InlineData("front", "Use '<face> <direction>'")]
    [InlineData("middle >", "Unknown face 'middle'.")]
    [InlineData("front backwards", "Unknown direction 'backwards'.")]
    public void InvalidInputReportsTheProblemWithoutChangingState(string command, string expectedError)
    {
        var cube = new Cube();
        cube.Rotate(FacePosition.Front, RotationDirection.Clockwise);
        cube.Rotate(FacePosition.Right, RotationDirection.CounterClockwise);
        var transcript = Run($"front >\n{command}\nright <\n");
        Assert.Contains($"Invalid command. {expectedError}", transcript);
        Assert.EndsWith(CubeConsoleRenderer.Render(cube) + "> ", transcript);
    }

    [Fact]
    public void ResetRestoresSolvedCube()
    {
        var transcript = Run("front >\n ReSeT \n");
        Assert.EndsWith(CubeConsoleRenderer.Render(new Cube()) + "> ", transcript);
        Assert.DoesNotContain("Invalid command.", transcript);
    }

    [Fact]
    public void ColoredRedrawKeepsHelpAndOnlyTheCurrentCube()
    {
        using var input = new StringReader(AcceptanceCommands);
        using var output = new StringWriter();
        new CubeApplication(input, output, useColor: true,
            clearScreen: () => output.GetStringBuilder().Clear()).Run();

        var colored = output.ToString();
        Assert.Contains("\u001b[", colored, StringComparison.Ordinal);
        Assert.Equal(54, colored.Split("\u001b[0m", StringSplitOptions.None).Length - 1);
        var plain = System.Text.RegularExpressions.Regex.Replace(colored, @"\x1B\[[0-9;]*m", "");
        Assert.Equal(CubeApplication.Help + Environment.NewLine + AcceptanceOutput + "> ", plain);
    }

    private static string Run(string commands)
    {
        using var input = new StringReader(commands);
        using var output = new StringWriter();
        new CubeApplication(input, output).Run();
        return output.ToString();
    }
}
