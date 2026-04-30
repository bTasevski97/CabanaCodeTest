using Microsoft.AspNetCore.Mvc;
namespace ResortMap.Maps;

[ApiController]
[Route("api/[controller]")]
public class MapController(Map map) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var response = new MapResponse(
            map.Rows,
            map.Cols,
            map.Tiles.Select(t => new TileDto(t.Type, t.Row, t.Col, t.PathVariant, t.Rotation, t.CabanaId)).ToList()
        );
        return Ok(response);
    }
}

public record MapResponse(int Rows, int Cols, List<TileDto> Tiles);
public record TileDto(TileType Type, int Row, int Col, PathVariant? PathVariant, int? Rotation, string? CabanaId);
