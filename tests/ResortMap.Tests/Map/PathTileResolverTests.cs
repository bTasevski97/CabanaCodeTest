using ResortMap.Map;
using Xunit;

namespace ResortMap.Tests.Map;

public class PathTileResolverTests
{
    [Fact]
    public void FourConnections_ReturnsCrossing()
    {
        string[] map = [".#.", "###", ".#."];

        var (variant, rotation) = PathTileResolver.Resolve(map, 1, 1);

        Assert.Equal(PathVariant.Crossing, variant);
        Assert.Equal(0, rotation);
    }

    [Theory]
    [InlineData(new[] { ".#.", ".##", ".#." }, 0,   "missing left")]
    [InlineData(new[] { "...", "###", ".#." }, 90,  "missing top")]
    [InlineData(new[] { ".#.", "##.", ".#." }, 180, "missing right")]
    [InlineData(new[] { ".#.", "###", "..." }, 270, "missing bottom")]
    public void ThreeConnections_ReturnsSplitWithCorrectRotation(string[] map, int expectedRotation, string _)
    {
        var (variant, rotation) = PathTileResolver.Resolve(map, 1, 1);

        Assert.Equal(PathVariant.Split, variant);
        Assert.Equal(expectedRotation, rotation);
    }

    [Theory]
    [InlineData(new[] { ".#.", ".#.", ".#." }, 0,  "vertical")]
    [InlineData(new[] { "...", "###", "..." }, 90, "horizontal")]
    public void TwoOppositeConnections_ReturnsStraight(string[] map, int expectedRotation, string _)
    {
        var (variant, rotation) = PathTileResolver.Resolve(map, 1, 1);

        Assert.Equal(PathVariant.Straight, variant);
        Assert.Equal(expectedRotation, rotation);
    }

    [Theory]
    [InlineData(new[] { ".#.", ".##", "..." }, 0,   "top-right")]
    [InlineData(new[] { "...", ".##", ".#." }, 90,  "right-bottom")]
    [InlineData(new[] { "...", "##.", ".#." }, 180, "bottom-left")]
    [InlineData(new[] { ".#.", "##.", "..." }, 270, "top-left")]
    public void TwoAdjacentConnections_ReturnsCornerWithCorrectRotation(string[] map, int expectedRotation, string _)
    {
        var (variant, rotation) = PathTileResolver.Resolve(map, 1, 1);

        Assert.Equal(PathVariant.Corner, variant);
        Assert.Equal(expectedRotation, rotation);
    }

    [Theory]
    [InlineData(new[] { "...", ".#.", ".#." }, 0,   "bottom only")]
    [InlineData(new[] { "...", "##.", "..." }, 90,  "left only")]
    [InlineData(new[] { ".#.", ".#.", "..." }, 180, "top only")]
    [InlineData(new[] { "...", ".##", "..." }, 270, "right only")]
    public void OneConnection_ReturnsEndWithCorrectRotation(string[] map, int expectedRotation, string _)
    {
        var (variant, rotation) = PathTileResolver.Resolve(map, 1, 1);

        Assert.Equal(PathVariant.End, variant);
        Assert.Equal(expectedRotation, rotation);
    }

    [Fact]
    public void NoConnections_DefaultsToStraight()
    {
        string[] map = ["...", ".#.", "..."];

        var (variant, rotation) = PathTileResolver.Resolve(map, 1, 1);

        Assert.Equal(PathVariant.Straight, variant);
        Assert.Equal(0, rotation);
    }

    [Fact]
    public void TopLeftCorner_HandlesOutOfBoundsGracefully()
    {
        string[] map = ["##", "##"];

        var (variant, _) = PathTileResolver.Resolve(map, 0, 0);

        Assert.Equal(PathVariant.Corner, variant);
    }

    [Fact]
    public void SingleTile_TreatsAsIsolated()
    {
        string[] map = ["#"];

        var (variant, rotation) = PathTileResolver.Resolve(map, 0, 0);

        Assert.Equal(PathVariant.Straight, variant);
        Assert.Equal(0, rotation);
    }
}
