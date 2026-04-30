using ResortMap.Maps;
using Xunit;

namespace ResortMap.Tests.Maps;

public class MapParserServiceTests
{
    [Fact]
    public void EmptyInput_ReturnsEmptyMap()
    {
        var result = Map.Parse([]);

        Assert.Equal(0, result.Rows);
        Assert.Equal(0, result.Cols);
        Assert.Empty(result.Tiles);
    }

    [Theory]
    [InlineData('.', TileType.Empty)]
    [InlineData('c', TileType.Chalet)]
    [InlineData('#', TileType.Path)]
    [InlineData('p', TileType.Pool)]
    [InlineData('W', TileType.Cabana)]
    [InlineData('?', TileType.Empty)]
    public void SingleCharacter_MapsToCorrectTileType(char character, TileType expectedType)
    {
        var result = Map.Parse([character.ToString()]);

        var tile = Assert.Single(result.Tiles);
        Assert.Equal(expectedType, tile.Type);
    }

    [Fact]
    public void CabanaTile_GetsGeneratedId()
    {
        var result = Map.Parse(["W"]);

        var tile = Assert.Single(result.Tiles);
        Assert.Equal("W-0-0", tile.CabanaId);
    }

    [Fact]
    public void NonCabanaTiles_HaveNullCabanaId()
    {
        var result = Map.Parse(["c#p."]);

        Assert.All(result.Tiles, tile => Assert.Null(tile.CabanaId));
    }

    [Fact]
    public void MultiRowMap_SetsCorrectDimensions()
    {
        string[] lines = [".c#", "W.p", "..."];

        var result = Map.Parse(lines);

        Assert.Equal(3, result.Rows);
        Assert.Equal(3, result.Cols);
        Assert.Equal(9, result.Tiles.Count());
    }

    [Fact]
    public void TileCoordinates_MatchRowAndColumnPosition()
    {
        string[] lines = [".W", "c."];

        var result = Map.Parse(lines);

        var cabana = result.Tiles.Single(t => t.Type == TileType.Cabana);
        Assert.Equal(0, cabana.Row);
        Assert.Equal(1, cabana.Col);
        Assert.Equal("W-0-1", cabana.CabanaId);

        var chalet = result.Tiles.Single(t => t.Type == TileType.Chalet);
        Assert.Equal(1, chalet.Row);
        Assert.Equal(0, chalet.Col);
    }

    [Fact]
    public void ShortLine_PadsWithEmptyTiles()
    {
        string[] lines = ["...", "."];

        var result = Map.Parse(lines);

        Assert.Equal(3, result.Cols);
        Assert.Equal(6, result.Tiles.Count());

        var paddedTile = result.Tiles.Single(t => t.Row == 1 && t.Col == 2);
        Assert.Equal(TileType.Empty, paddedTile.Type);
    }

    [Fact]
    public void PathTile_FourConnections_ResolvesAsCrossing()
    {
        var result = Map.Parse([".#.", "###", ".#."]);

        var center = result.Tiles.Single(t => t.Row == 1 && t.Col == 1);
        Assert.Equal(PathVariant.Crossing, center.PathVariant);
        Assert.Equal(0, center.Rotation);
    }

    [Fact]
    public void PathTile_VerticalStraight_ResolvesWithZeroRotation()
    {
        var result = Map.Parse(["#", "#", "#"]);

        var middle = result.Tiles.Single(t => t.Row == 1);
        Assert.Equal(PathVariant.Straight, middle.PathVariant);
        Assert.Equal(0, middle.Rotation);
    }

    [Fact]
    public void PathTile_HorizontalStraight_ResolvesWith90Rotation()
    {
        var result = Map.Parse(["...", "###", "..."]);

        var middle = result.Tiles.Single(t => t.Row == 1 && t.Col == 1);
        Assert.Equal(PathVariant.Straight, middle.PathVariant);
        Assert.Equal(90, middle.Rotation);
    }

    [Fact]
    public void PathTile_TopRightCorner_ResolvesWithZeroRotation()
    {
        var result = Map.Parse([".#.", ".##", "..."]);

        var center = result.Tiles.Single(t => t.Row == 1 && t.Col == 1);
        Assert.Equal(PathVariant.Corner, center.PathVariant);
        Assert.Equal(0, center.Rotation);
    }

    [Fact]
    public void PathTile_BottomLeftCorner_ResolvesWith180Rotation()
    {
        var result = Map.Parse(["...", "##.", ".#."]);

        var center = result.Tiles.Single(t => t.Row == 1 && t.Col == 1);
        Assert.Equal(PathVariant.Corner, center.PathVariant);
        Assert.Equal(180, center.Rotation);
    }

    [Fact]
    public void PathTile_TSplitMissingLeft_ResolvesWithZeroRotation()
    {
        var result = Map.Parse([".#.", ".##", ".#."]);

        var center = result.Tiles.Single(t => t.Row == 1 && t.Col == 1);
        Assert.Equal(PathVariant.Split, center.PathVariant);
        Assert.Equal(0, center.Rotation);
    }

    [Fact]
    public void PathTile_DeadEndBottom_ResolvesWithZeroRotation()
    {
        var result = Map.Parse(["...", ".#.", ".#."]);

        var center = result.Tiles.Single(t => t.Row == 1 && t.Col == 1);
        Assert.Equal(PathVariant.End, center.PathVariant);
        Assert.Equal(0, center.Rotation);
    }

    [Fact]
    public void PathTile_Isolated_DefaultsToStraight()
    {
        var result = Map.Parse(["...", ".#.", "..."]);

        var center = result.Tiles.Single(t => t.Row == 1 && t.Col == 1);
        Assert.Equal(PathVariant.Straight, center.PathVariant);
        Assert.Equal(0, center.Rotation);
    }

    [Fact]
    public void PathTile_AtMapEdge_HandlesOutOfBoundsGracefully()
    {
        var result = Map.Parse(["##", "##"]);

        var topLeft = result.Tiles.Single(t => t.Row == 0 && t.Col == 0);
        Assert.Equal(PathVariant.Corner, topLeft.PathVariant);
    }

    [Fact]
    public void MultipleCabanas_EachGetUniqueId()
    {
        string[] lines = ["WW", "WW"];

        var result = Map.Parse(lines);

        var cabanaIds = result.Tiles
            .Where(t => t.Type == TileType.Cabana)
            .Select(t => t.CabanaId)
            .ToList();

        Assert.Equal(4, cabanaIds.Count);
        Assert.Equal(cabanaIds.Count, cabanaIds.Distinct().Count());
    }
}
