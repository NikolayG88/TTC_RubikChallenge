namespace TTC_RubikChallenge.Domain;

/// <summary>A face viewed directly from outside the cube; rows run down, columns right.</summary>
internal sealed class Face
{
    private TileColor[,] tiles = new TileColor[3, 3];

    internal Face(Func<int, int, TileColor> initialize)
    {
        for (var row = 0; row < 3; row++)
        for (var column = 0; column < 3; column++)
        {
            var color = initialize(row, column);
            if (!Enum.IsDefined(color))
                throw new ArgumentOutOfRangeException(nameof(initialize));
            tiles[row, column] = color;
        }
    }

    internal TileColor this[int row, int column] => tiles[row, column];

    internal void RotateClockwise()
    {
        var rotated = new TileColor[3, 3];
        for (var row = 0; row < 3; row++)
        for (var column = 0; column < 3; column++)
            rotated[column, 2 - row] = tiles[row, column];
        tiles = rotated;
    }

    // Horizontal edges are left-to-right; vertical edges are top-to-bottom.
    internal TileColor[] GetEdge(Edge edge)
    {
        ValidateEdge(edge);
        var values = new TileColor[3];
        for (var i = 0; i < 3; i++)
        {
            var (row, column) = Coordinates(edge, i);
            values[i] = tiles[row, column];
        }
        return values;
    }

    internal void SetEdge(Edge edge, ReadOnlySpan<TileColor> values)
    {
        ValidateEdge(edge);
        if (values.Length != 3)
            throw new ArgumentException("An edge must contain exactly three colors.", nameof(values));
        foreach (var color in values)
            if (!Enum.IsDefined(color))
                throw new ArgumentOutOfRangeException(nameof(values));
        for (var i = 0; i < 3; i++)
        {
            var (row, column) = Coordinates(edge, i);
            tiles[row, column] = values[i];
        }
    }

    private static void ValidateEdge(Edge edge)
    {
        if (!Enum.IsDefined(edge))
            throw new ArgumentOutOfRangeException(nameof(edge));
    }

    private static (int Row, int Column) Coordinates(Edge edge, int i) => edge switch
    {
        Edge.Top => (0, i),
        Edge.Right => (i, 2),
        Edge.Bottom => (2, i),
        Edge.Left => (i, 0),
        _ => throw new ArgumentOutOfRangeException(nameof(edge))
    };
}
