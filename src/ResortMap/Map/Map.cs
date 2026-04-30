namespace ResortMap.Maps;

public class Map
{
    public int Rows { get; }
    public int Cols { get; }
    public IReadOnlyList<Tile> Tiles { get; }

    private Map(int rows, int cols, IReadOnlyList<Tile> tiles)
    {
        Rows = rows;
        Cols = cols;
        Tiles = tiles;
    }

    public static Map Parse(string[] lines)
    {
        if (lines.Length == 0) return new Map(0, 0, []);

        int rows = lines.Length;
        int cols = lines[0].Length;
        var grid = new Tile[rows, cols];

        // Step 1: Read all characters and create tiles
        for (int r = 0; r < rows; r++)
        {
            string currentLine = lines[r];

            for (int c = 0; c < cols; c++)
            {
                char character = c < currentLine.Length ? currentLine[c] : '.';
                TileType tileType = GetTileTypeFromCharacter(character);

                grid[r, c] = new Tile
                {
                    Type = tileType,
                    Row = r,
                    Col = c,
                    CabanaId = tileType == TileType.Cabana ? $"W-{r}-{c}" : null
                };
            }
        }

        // Step 2: Resolve path tile variants (straight, corner, split, crossing, end)
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                var tile = grid[r, c];
                if (tile.Type == TileType.Path)
                {
                    var resolvedPath = PathTileResolver.Resolve(grid, r, c);
                    grid[r, c] = tile with 
                    { 
                        PathVariant = resolvedPath.Variant, 
                        Rotation = resolvedPath.Rotation 
                    };
                }
            }
        }

        return new Map(rows, cols, grid.Cast<Tile>().ToArray());
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
