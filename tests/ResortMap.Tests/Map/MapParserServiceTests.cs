using ResortMap.Maps;
using Xunit;

using MapModel = ResortMap.Maps.Map;

namespace ResortMap.Tests.Map;

public class MapParserServiceTests
{
    [Fact]
    public void EmptyInput_ReturnsEmptyMap()
    {
        var result = MapModel.Parse([]);

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
        var result = MapModel.Parse([character.ToString()]);

        var tile = Assert.Single(result.Tiles);
        Assert.Equal(expectedType, tile.Type);
    }

    [Fact]
    public void CabanaTile_GetsGeneratedId()
    {
        var result = MapModel.Parse(["W"]);

        var tile = Assert.Single(result.Tiles);
        Assert.Equal("W-0-0", tile.CabanaId);
    }

    [Fact]
    public void NonCabanaTiles_HaveNullCabanaId()
    {
        var result = MapModel.Parse(["c#p."]);

        Assert.All(result.Tiles, tile => Assert.Null(tile.CabanaId));
    }

    [Fact]
    public void MultiRowMap_SetsCorrectDimensions()
    {
        string[] lines = [".c#", "W.p", "..."];

        var result = MapModel.Parse(lines);

        Assert.Equal(3, result.Rows);
        Assert.Equal(3, result.Cols);
        Assert.Equal(9, result.Tiles.Count());
    }

    [Fact]
    public void TileCoordinates_MatchRowAndColumnPosition()
    {
        string[] lines = [".W", "c."];

        var result = MapModel.Parse(lines);

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

        var result = MapModel.Parse(lines);

        Assert.Equal(3, result.Cols);
        Assert.Equal(6, result.Tiles.Count());

        var paddedTile = result.Tiles.Single(t => t.Row == 1 && t.Col == 2);
        Assert.Equal(TileType.Empty, paddedTile.Type);
    }

    [Fact]
    public void PathTiles_GetVariantAndRotationResolved()
    {
        // Vertical straight path
        string[] lines = ["#", "#", "#"];

        var result = MapModel.Parse(lines);

        var middlePath = result.Tiles.Single(t => t.Row == 1);
        Assert.Equal(PathVariant.Straight, middlePath.PathVariant);
        Assert.Equal(0, middlePath.Rotation);
    }

    [Fact]
    public void MultipleCabanas_EachGetUniqueId()
    {
        string[] lines = ["WW", "WW"];

        var result = MapModel.Parse(lines);

        var cabanaIds = result.Tiles
            .Where(t => t.Type == TileType.Cabana)
            .Select(t => t.CabanaId)
            .ToList();

        Assert.Equal(4, cabanaIds.Count);
        Assert.Equal(cabanaIds.Count, cabanaIds.Distinct().Count());
    }
}
