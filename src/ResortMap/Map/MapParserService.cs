namespace ResortMap.Map;

public enum TileType { Empty, Chalet, Path, Pool, Cabana }
public enum PathVariant { Straight, Corner, Crossing, End, Split }

public record Tile
{
    public TileType Type { get; set; } = TileType.Empty;
    public int Row { get; set; }
    public int Col { get; set; }
    public PathVariant? PathVariant { get; set; }
    public int? Rotation { get; set; }
    public string? CabanaId { get; set; }
}

public class MapResponse
{
    public int Rows { get; set; }
    public int Cols { get; set; }
    public List<Tile> Tiles { get; set; } = new();
}

public class MapParserService
{
    public MapResponse ParseMap(string[] lines)
    {
        var response = new MapResponse();

        if (lines.Length == 0)
        {
            return response;
        }

        response.Rows = lines.Length;
        response.Cols = lines[0].Length;

        // Step 1: Read all characters and create tiles
        for (int row = 0; row < response.Rows; row++)
        {
            string currentLine = lines[row];

            for (int col = 0; col < response.Cols; col++)
            {
                char character = col < currentLine.Length ? currentLine[col] : '.';
                TileType tileType = GetTileTypeFromCharacter(character);

                var tile = new Tile
                {
                    Type = tileType,
                    Row = row,
                    Col = col,
                    CabanaId = tileType == TileType.Cabana ? $"W-{row}-{col}" : null
                };

                response.Tiles.Add(tile);
            }
        }

        // Step 2: Resolve path tile variants (straight, corner, split, crossing, end)
        foreach (var tile in response.Tiles)
        {
            if (tile.Type == TileType.Path)
            {
                var resolvedPath = PathTileResolver.Resolve(lines, tile.Row, tile.Col);
                tile.PathVariant = resolvedPath.Variant;
                tile.Rotation = resolvedPath.Rotation;
            }
        }

        return response;
    }

    private static TileType GetTileTypeFromCharacter(char c) => c switch
    {
        '.' => TileType.Empty,
        'c' => TileType.Chalet,
        '#' => TileType.Path,
        'p' => TileType.Pool,
        'W' => TileType.Cabana,
        _   => TileType.Empty
    };
}
