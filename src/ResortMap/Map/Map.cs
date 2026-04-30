namespace ResortMap.Maps;

public class Map
{
    private readonly Tile[,] _grid;

    public int Rows { get; }
    public int Cols { get; }

    private Map(int rows, int cols)
    {
        Rows = rows;
        Cols = cols;
        _grid = new Tile[rows, cols];
    }

    public IReadOnlyList<Tile> Tiles { get; private set; } = [];


    public static Map Parse(string[] lines)
    {
        if (lines.Length == 0) return new Map(0, 0);

        int rows = lines.Length;
        int cols = lines[0].Length;
        var map = new Map(rows, cols);

        // Step 1: Read all characters and create tiles
        for (int r = 0; r < rows; r++)
        {
            string currentLine = lines[r];

            for (int c = 0; c < cols; c++)
            {
                char character = c < currentLine.Length ? currentLine[c] : '.';
                TileType tileType = GetTileTypeFromCharacter(character);

                map._grid[r, c] = new Tile
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
                var tile = map._grid[r, c];
                if (tile.Type == TileType.Path)
                {
                    var resolvedPath = PathTileResolver.Resolve(map._grid, r, c);
                    map._grid[r, c] = tile with 
                    { 
                        PathVariant = resolvedPath.Variant, 
                        Rotation = resolvedPath.Rotation 
                    };
                }
            }
        }

        map.Tiles = map._grid.Cast<Tile>().ToArray();

        return map;
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
