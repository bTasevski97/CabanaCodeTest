using ResortMap.Maps;
using Xunit;

namespace ResortMap.Tests.Map;

public class PathTileResolverTests
{
    private static Tile[,] ParseGrid(string[] lines)
    {
        int rows = lines.Length;
        int cols = lines[0].Length;
        var grid = new Tile[rows, cols];

        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                grid[r, c] = new Tile
                {
                    Type = lines[r][c] == '#' ? TileType.Path : TileType.Empty,
                    Row = r,
                    Col = c
                };

        return grid;
    }

    [Fact]
    public void FourConnections_ReturnsCrossing()
    {
        var grid = ParseGrid([".#.", "###", ".#."]);

        var (variant, rotation) = PathTileResolver.Resolve(grid, 1, 1);

        Assert.Equal(PathVariant.Crossing, variant);
        Assert.Equal(0, rotation);
    }

    [Theory]
    [InlineData(new[] { ".#.", ".##", ".#." }, 0,   "missing left")]
    [InlineData(new[] { "...", "###", ".#." }, 90,  "missing top")]
    [InlineData(new[] { ".#.", "##.", ".#." }, 180, "missing right")]
    [InlineData(new[] { ".#.", "###", "..." }, 270, "missing bottom")]
    public void ThreeConnections_ReturnsSplitWithCorrectRotation(string[] lines, int expectedRotation, string _)
    {
        var grid = ParseGrid(lines);

        var (variant, rotation) = PathTileResolver.Resolve(grid, 1, 1);

        Assert.Equal(PathVariant.Split, variant);
        Assert.Equal(expectedRotation, rotation);
    }

    [Theory]
    [InlineData(new[] { ".#.", ".#.", ".#." }, 0,  "vertical")]
    [InlineData(new[] { "...", "###", "..." }, 90, "horizontal")]
    public void TwoOppositeConnections_ReturnsStraight(string[] lines, int expectedRotation, string _)
    {
        var grid = ParseGrid(lines);

        var (variant, rotation) = PathTileResolver.Resolve(grid, 1, 1);

        Assert.Equal(PathVariant.Straight, variant);
        Assert.Equal(expectedRotation, rotation);
    }

    [Theory]
    [InlineData(new[] { ".#.", ".##", "..." }, 0,   "top-right")]
    [InlineData(new[] { "...", ".##", ".#." }, 90,  "right-bottom")]
    [InlineData(new[] { "...", "##.", ".#." }, 180, "bottom-left")]
    [InlineData(new[] { ".#.", "##.", "..." }, 270, "top-left")]
    public void TwoAdjacentConnections_ReturnsCornerWithCorrectRotation(string[] lines, int expectedRotation, string _)
    {
        var grid = ParseGrid(lines);

        var (variant, rotation) = PathTileResolver.Resolve(grid, 1, 1);

        Assert.Equal(PathVariant.Corner, variant);
        Assert.Equal(expectedRotation, rotation);
    }

    [Theory]
    [InlineData(new[] { "...", ".#.", ".#." }, 0,   "bottom only")]
    [InlineData(new[] { "...", "##.", "..." }, 90,  "left only")]
    [InlineData(new[] { ".#.", ".#.", "..." }, 180, "top only")]
    [InlineData(new[] { "...", ".##", "..." }, 270, "right only")]
    public void OneConnection_ReturnsEndWithCorrectRotation(string[] lines, int expectedRotation, string _)
    {
        var grid = ParseGrid(lines);

        var (variant, rotation) = PathTileResolver.Resolve(grid, 1, 1);

        Assert.Equal(PathVariant.End, variant);
        Assert.Equal(expectedRotation, rotation);
    }

    [Fact]
    public void NoConnections_DefaultsToStraight()
    {
        var grid = ParseGrid(["...", ".#.", "..."]);

        var (variant, rotation) = PathTileResolver.Resolve(grid, 1, 1);

        Assert.Equal(PathVariant.Straight, variant);
        Assert.Equal(0, rotation);
    }

    [Fact]
    public void TopLeftCorner_HandlesOutOfBoundsGracefully()
    {
        var grid = ParseGrid(["##", "##"]);

        var (variant, _) = PathTileResolver.Resolve(grid, 0, 0);

        Assert.Equal(PathVariant.Corner, variant);
    }

    [Fact]
    public void SingleTile_TreatsAsIsolated()
    {
        var grid = ParseGrid(["#"]);

        var (variant, rotation) = PathTileResolver.Resolve(grid, 0, 0);

        Assert.Equal(PathVariant.Straight, variant);
        Assert.Equal(0, rotation);
    }
}
