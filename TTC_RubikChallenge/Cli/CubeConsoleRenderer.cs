using System.Text;
using TTC_RubikChallenge.Domain;

namespace TTC_RubikChallenge.Cli;

public static class CubeConsoleRenderer
{
    private const string Indent = "          ";
    public const string Legend = "W White | Y Yellow | G Green | B Blue | R Red | O Orange";

    public static string Render(Cube cube, bool useColor = false)
    {
        ArgumentNullException.ThrowIfNull(cube);
        var output = new StringBuilder();
        output.AppendLine(Indent + "Top");
        for (var row = 0; row < 3; row++)
            output.AppendLine(Indent + Row(cube, FacePosition.Top, row, useColor));
        output.AppendLine("Left      Front     Right     Back");
        for (var row = 0; row < 3; row++)
            output.AppendLine(string.Join("     ",
                Row(cube, FacePosition.Left, row, useColor), Row(cube, FacePosition.Front, row, useColor),
                Row(cube, FacePosition.Right, row, useColor), Row(cube, FacePosition.Back, row, useColor)));
        output.AppendLine(Indent + "Bottom");
        for (var row = 0; row < 3; row++)
            output.AppendLine(Indent + Row(cube, FacePosition.Bottom, row, useColor));
        output.AppendLine(Legend);
        return output.ToString();
    }

    private static string Row(Cube cube, FacePosition face, int row, bool useColor) =>
        $"{Sticker(cube[face, row, 0], useColor)} {Sticker(cube[face, row, 1], useColor)} {Sticker(cube[face, row, 2], useColor)}";

    private static string Sticker(TileColor color, bool useColor)
    {
        var symbol = Symbol(color);
        if (!useColor) return symbol.ToString();

        // True-color backgrounds distinguish orange from red and yellow.
        var background = color switch
        {
            TileColor.White => "255;255;255",
            TileColor.Yellow => "255;220;0",
            TileColor.Green => "0;155;72",
            TileColor.Blue => "0;70;173",
            TileColor.Red => "190;0;30",
            TileColor.Orange => "255;130;0",
            _ => throw new ArgumentOutOfRangeException(nameof(color))
        };
        var foreground = color is TileColor.Green or TileColor.Blue or TileColor.Red ? 97 : 30;
        return $"\u001b[{foreground};48;2;{background}m{symbol}\u001b[0m";
    }

    private static char Symbol(TileColor color) => color switch
    {
        TileColor.White => 'W', TileColor.Yellow => 'Y', TileColor.Green => 'G',
        TileColor.Blue => 'B', TileColor.Red => 'R', TileColor.Orange => 'O',
        _ => throw new ArgumentOutOfRangeException(nameof(color))
    };
}
