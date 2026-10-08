using System.Collections.Immutable;

namespace TTC_RubikChallenge.Domain;

/// <summary>Owns all six faces and is the only public entry point for turns.</summary>
public sealed class Cube
{
    private readonly Face[] faces;
    private static readonly ImmutableDictionary<FacePosition, ImmutableArray<Transfer>> Transfers =
        Enum.GetValues<FacePosition>().ToImmutableDictionary(face => face, CreateClockwiseTransfers);

    public Cube() : this((face, _, _) => face switch
    {
        FacePosition.Top => TileColor.White,
        FacePosition.Bottom => TileColor.Yellow,
        FacePosition.Front => TileColor.Green,
        FacePosition.Back => TileColor.Blue,
        FacePosition.Left => TileColor.Orange,
        FacePosition.Right => TileColor.Red,
        _ => throw new ArgumentOutOfRangeException(nameof(face))
    }) { }

    // Internal construction lets tests track individual stickers without widening the public API.
    internal Cube(Func<FacePosition, int, int, TileColor> initialize)
    {
        faces = Enum.GetValues<FacePosition>()
            .Select(position => new Face((row, column) => initialize(position, row, column)))
            .ToArray();
    }

    public TileColor this[FacePosition face, int row, int column]
    {
        get
        {
            ValidateFace(face);
            if (row is < 0 or > 2) throw new ArgumentOutOfRangeException(nameof(row));
            if (column is < 0 or > 2) throw new ArgumentOutOfRangeException(nameof(column));
            return faces[(int)face][row, column];
        }
    }

    /// <summary>Turns a face as seen from outside the cube. Invalid arguments leave state unchanged.</summary>
    public void Rotate(FacePosition face, RotationDirection direction)
    {
        ValidateFace(face);
        if (!Enum.IsDefined(direction))
            throw new ArgumentOutOfRangeException(nameof(direction));
        var turns = direction == RotationDirection.Clockwise ? 1 : 3;
        for (var i = 0; i < turns; i++) RotateClockwise(face);
    }

    private void RotateClockwise(FacePosition face)
    {
        var transfers = Transfers[face];
        // Save every source strip before writing: a destination is another transfer's source.
        var strips = new TileColor[4][];
        for (var i = 0; i < transfers.Length; i++)
            strips[i] = faces[(int)transfers[i].SourceFace].GetEdge(transfers[i].SourceEdge);

        faces[(int)face].RotateClockwise();
        for (var i = 0; i < transfers.Length; i++)
        {
            var transfer = transfers[i];
            if (transfer.Reverse) Array.Reverse(strips[i]);
            faces[(int)transfer.DestinationFace].SetEdge(transfer.DestinationEdge, strips[i]);
        }
    }

    private static void ValidateFace(FacePosition face)
    {
        if (!Enum.IsDefined(face)) throw new ArgumentOutOfRangeException(nameof(face));
    }

    // Reverse means reverse the saved source strip before writing the destination.
    // Axes: +X Right, +Y Top, +Z Front. These tables follow the README's face-local axes.
    private readonly record struct Transfer(
        FacePosition SourceFace, Edge SourceEdge,
        FacePosition DestinationFace, Edge DestinationEdge, bool Reverse = false);

    private static ImmutableArray<Transfer> CreateClockwiseTransfers(FacePosition face) => face switch
    {
        FacePosition.Front => [
            new(FacePosition.Top, Edge.Bottom, FacePosition.Right, Edge.Left),
            new(FacePosition.Right, Edge.Left, FacePosition.Bottom, Edge.Top, Reverse: true),
            new(FacePosition.Bottom, Edge.Top, FacePosition.Left, Edge.Right),
            new(FacePosition.Left, Edge.Right, FacePosition.Top, Edge.Bottom, Reverse: true)],
        FacePosition.Back => [
            new(FacePosition.Top, Edge.Top, FacePosition.Left, Edge.Left, Reverse: true),
            new(FacePosition.Left, Edge.Left, FacePosition.Bottom, Edge.Bottom),
            new(FacePosition.Bottom, Edge.Bottom, FacePosition.Right, Edge.Right, Reverse: true),
            new(FacePosition.Right, Edge.Right, FacePosition.Top, Edge.Top)],
        FacePosition.Top => [
            new(FacePosition.Front, Edge.Top, FacePosition.Left, Edge.Top),
            new(FacePosition.Left, Edge.Top, FacePosition.Back, Edge.Top),
            new(FacePosition.Back, Edge.Top, FacePosition.Right, Edge.Top),
            new(FacePosition.Right, Edge.Top, FacePosition.Front, Edge.Top)],
        FacePosition.Bottom => [
            new(FacePosition.Front, Edge.Bottom, FacePosition.Right, Edge.Bottom),
            new(FacePosition.Right, Edge.Bottom, FacePosition.Back, Edge.Bottom),
            new(FacePosition.Back, Edge.Bottom, FacePosition.Left, Edge.Bottom),
            new(FacePosition.Left, Edge.Bottom, FacePosition.Front, Edge.Bottom)],
        FacePosition.Right => [
            new(FacePosition.Top, Edge.Right, FacePosition.Back, Edge.Left, Reverse: true),
            new(FacePosition.Back, Edge.Left, FacePosition.Bottom, Edge.Right, Reverse: true),
            new(FacePosition.Bottom, Edge.Right, FacePosition.Front, Edge.Right),
            new(FacePosition.Front, Edge.Right, FacePosition.Top, Edge.Right)],
        FacePosition.Left => [
            new(FacePosition.Top, Edge.Left, FacePosition.Front, Edge.Left),
            new(FacePosition.Front, Edge.Left, FacePosition.Bottom, Edge.Left),
            new(FacePosition.Bottom, Edge.Left, FacePosition.Back, Edge.Right, Reverse: true),
            new(FacePosition.Back, Edge.Right, FacePosition.Top, Edge.Left, Reverse: true)],
        _ => throw new ArgumentOutOfRangeException(nameof(face))
    };
}
