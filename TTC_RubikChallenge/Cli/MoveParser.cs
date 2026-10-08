using TTC_RubikChallenge.Domain;

namespace TTC_RubikChallenge.Cli;

public static class MoveParser
{
    public static bool TryParse(
        string? input, out FacePosition face, out RotationDirection direction, out string? error)
    {
        face = default;
        direction = default;
        error = null;
        var tokens = input?.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        if (tokens is not { Length: 2 })
        {
            error = "Use '<face> <direction>': '>' for clockwise or '<' for counterclockwise, e.g. 'front >'. Use 'reset' to restore the solved cube.";
            return false;
        }
        FacePosition? parsedFace = tokens[0].ToLowerInvariant() switch
        {
            "top" or "up" => FacePosition.Top,
            "bottom" or "down" => FacePosition.Bottom,
            "front" => FacePosition.Front,
            "back" => FacePosition.Back,
            "left" => FacePosition.Left,
            "right" => FacePosition.Right,
            _ => null
        };
        if (parsedFace is null)
        {
            error = $"Unknown face '{tokens[0]}'. Use top, bottom, front, back, left, or right (up/down also accepted).";
            return false;
        }

        RotationDirection? parsedDirection = tokens[1] switch
        {
            ">" => RotationDirection.Clockwise,
            "<" => RotationDirection.CounterClockwise,
            _ => null
        };
        if (parsedDirection is null)
        {
            error = $"Unknown direction '{tokens[1]}'. Use '>' or '<'.";
            return false;
        }
        face = parsedFace.Value;
        direction = parsedDirection.Value;
        return true;
    }
}
