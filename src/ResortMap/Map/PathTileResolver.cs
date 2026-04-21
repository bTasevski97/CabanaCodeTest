namespace ResortMap.Map;

public static class PathTileResolver
{
    public static (PathVariant Variant, int Rotation) Resolve(string[] map, int row, int col)
    {
        bool hasTop    = IsPath(map, row - 1, col);
        bool hasBottom = IsPath(map, row + 1, col);
        bool hasLeft   = IsPath(map, row, col - 1);
        bool hasRight  = IsPath(map, row, col + 1);

        int connections = (hasTop ? 1 : 0) + (hasBottom ? 1 : 0) + (hasLeft ? 1 : 0) + (hasRight ? 1 : 0);

        // It's a crossing if all 4 sides connect
        if (connections == 4)
        {
            return (PathVariant.Crossing, 0);
        }

        // It's a split (T-junction) if 3 sides connect
        if (connections == 3)
        {
            if (!hasLeft)   return (PathVariant.Split, 0);
            if (!hasTop)    return (PathVariant.Split, 90);
            if (!hasRight)  return (PathVariant.Split, 180);
            if (!hasBottom) return (PathVariant.Split, 270);
        }

        // It's a straight line or a corner if 2 sides connect
        if (connections == 2)
        {
            // Straight lines
            if (hasTop && hasBottom) return (PathVariant.Straight, 0);
            if (hasLeft && hasRight) return (PathVariant.Straight, 90);

            // Corners
            if (hasTop && hasRight)    return (PathVariant.Corner, 0);
            if (hasRight && hasBottom) return (PathVariant.Corner, 90);
            if (hasBottom && hasLeft)  return (PathVariant.Corner, 180);
            if (hasTop && hasLeft)     return (PathVariant.Corner, 270);
        }

        // It's a dead end if 1 side connects
        if (connections == 1)
        {
            if (hasBottom) return (PathVariant.End, 0);
            if (hasLeft)   return (PathVariant.End, 90);
            if (hasTop)    return (PathVariant.End, 180);
            if (hasRight)  return (PathVariant.End, 270);
        }

        // Default to straight if the tile is floating (no neighbors)
        return (PathVariant.Straight, 0);
    }

    private static bool IsPath(string[] map, int row, int col)
    {
        if (row < 0 || row >= map.Length) return false;
        if (col < 0 || col >= map[row].Length) return false;
        return map[row][col] == '#';
    }
}
