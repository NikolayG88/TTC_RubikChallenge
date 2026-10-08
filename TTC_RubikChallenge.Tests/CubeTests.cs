using TTC_RubikChallenge.Domain;

namespace TTC_RubikChallenge.Tests;

public sealed class CubeTests
{
    // Fixed nonuniform starting state from the challenge acceptance fixture.
    // Groups are Top, Bottom, Front, Back, Left, Right; each group is row-major.
    private const string TurnStart = "ROGBWWBBB GGBRYRRGG ORROGWWWW YBWOBYYYW GYYOOGBGO YBORRWOYR";

    internal static TileColor[] Snapshot(Cube cube) =>
        (from face in Enum.GetValues<FacePosition>()
         from row in Enumerable.Range(0, 3)
         from column in Enumerable.Range(0, 3)
         select cube[face, row, column]).ToArray();

    [Fact]
    public void CubeStartsSolvedWithCorrectOrientation()
    {
        var cube = new Cube();
        TileColor[] expected = [TileColor.White, TileColor.Yellow, TileColor.Green,
            TileColor.Blue, TileColor.Orange, TileColor.Red];
        foreach (var face in Enum.GetValues<FacePosition>())
        for (var row = 0; row < 3; row++)
        for (var column = 0; column < 3; column++)
            Assert.Equal(expected[(int)face], cube[face, row, column]);
    }

    [Theory]
    [InlineData(FacePosition.Top, RotationDirection.Clockwise, "BBRBWOBWG GGBRYRRGG YBOOGWWWW GYYOBYYYW ORROOGBGO YBWRRWOYR")]
    [InlineData(FacePosition.Top, RotationDirection.CounterClockwise, "GWBOWBRBB GGBRYRRGG GYYOGWWWW YBOOBYYYW YBWOOGBGO ORRRRWOYR")]
    [InlineData(FacePosition.Bottom, RotationDirection.Clockwise, "ROGBWWBBB RRGGYGGRB ORROGWBGO YBWOBYOYR GYYOOGYYW YBORRWWWW")]
    [InlineData(FacePosition.Bottom, RotationDirection.CounterClockwise, "ROGBWWBBB BRGGYGGRR ORROGWOYR YBWOBYBGO GYYOOGWWW YBORRWYYW")]
    [InlineData(FacePosition.Front, RotationDirection.Clockwise, "ROGBWWOGY ORYRYRRGG WOOWGRWWR YBWOBYYYW GYGOOGBGB BBOBRWBYR")]
    [InlineData(FacePosition.Front, RotationDirection.CounterClockwise, "ROGBWWYRO YGORYRRGG RWWRGWOOW YBWOBYYYW GYBOOBBGB BBOGRWGYR")]
    [InlineData(FacePosition.Back, RotationDirection.Clockwise, "OWRBWWBBB GGBRYRGOB ORROGWWWW YOYYBBWYW GYYOOGRGO YBGRRGOYR")]
    [InlineData(FacePosition.Back, RotationDirection.CounterClockwise, "BOGBWWBBB GGBRYRRWO ORROGWWWW WYWBBYYOY RYYGOGGGO YBRRROOYG")]
    [InlineData(FacePosition.Left, RotationDirection.Clockwise, "WOGYWWWBB OGBOYRWGG RRRBGWBWW YBROBRYYG BOGGOYOGY YBORRWOYR")]
    [InlineData(FacePosition.Left, RotationDirection.CounterClockwise, "OOGOWWWBB WGBYYRWGG GRRRGWRWW YBBOBBYYR YGOYOGGOB YBORRWOYR")]
    [InlineData(FacePosition.Right, RotationDirection.Clockwise, "RORBWWBBW GGYRYORGY ORBOGRWWG BBWWBYGYW GYYOOGBGO ORYYRBRWO")]
    [InlineData(FacePosition.Right, RotationDirection.CounterClockwise, "ROYBWOBBY GGRRYWRGW ORGOGWWWB GBWRBYBYW GYYOOGBGO OWRBRYYRO")]
    public void TurnMatchesFixedStateAndInverseRestoresStart(
        FacePosition face, RotationDirection direction, string expected)
    {
        var start = ParseState(TurnStart);
        var cube = new Cube((f, r, c) => start[(int)f * 9 + r * 3 + c]);

        cube.Rotate(face, direction);
        Assert.Equal(ParseState(expected), Snapshot(cube));

        cube.Rotate(face, Inverse(direction));
        Assert.Equal(start, Snapshot(cube));
    }

    [Fact]
    public void InvalidArgumentsThrowBeforeMutation()
    {
        var start = ParseState(TurnStart);
        var cube = new Cube((f, r, c) => start[(int)f * 9 + r * 3 + c]);
        var before = Snapshot(cube);
        Assert.Throws<ArgumentOutOfRangeException>(() => cube.Rotate((FacePosition)6, RotationDirection.Clockwise));
        Assert.Throws<ArgumentOutOfRangeException>(() => cube.Rotate(FacePosition.Front, (RotationDirection)9));
        Assert.Equal(before, Snapshot(cube));
    }

    private static TileColor[] ParseState(string state) => state.Replace(" ", "")
        .Select(symbol => symbol switch
        {
            'W' => TileColor.White, 'Y' => TileColor.Yellow, 'G' => TileColor.Green,
            'B' => TileColor.Blue, 'R' => TileColor.Red, 'O' => TileColor.Orange,
            _ => throw new ArgumentException("Invalid color in test fixture.", nameof(state))
        }).ToArray();

    private static RotationDirection Inverse(RotationDirection direction) =>
        direction == RotationDirection.Clockwise ? RotationDirection.CounterClockwise : RotationDirection.Clockwise;

}
