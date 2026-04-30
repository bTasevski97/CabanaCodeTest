namespace ResortMap.Maps;

public enum TileType { Empty, Chalet, Path, Pool, Cabana }
public enum PathVariant { Straight, Corner, Crossing, End, Split }

public record Tile
{
    public TileType Type { get; init; } = TileType.Empty;
    public int Row { get; init; }
    public int Col { get; init; }
    public PathVariant? PathVariant { get; init; }
    public int? Rotation { get; init; }
    public string? CabanaId { get; init; }
}
