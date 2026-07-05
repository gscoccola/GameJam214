using System.Collections.Generic;
using System.Linq;
//using KwaaktjePathfinder2D;
using UnityEngine;
using UnityEngine.Tilemaps;

// This class generates a grid representation of the level based on a Tilemap and provides pathfinding functionality.
public class LevelGrid : Singleton<LevelGrid>
{
    [Header("References")]
    [SerializeField] private Tilemap _solidTilemap;


    private TileType[,] _tileTypeGrid;
    private BoundsInt _bounds;
    /*private float _cellSize;
    private float _cellSizeInverse;*/

    private new void Awake()
    {
        base.Awake();
        GenerateMovementGrid();
    }

    private void GenerateMovementGrid()
    {
        /*_cellSize = _solidTilemap.cellSize.x;
        _cellSizeInverse = 1f / _cellSize;*/
        _bounds = _solidTilemap.cellBounds;
        TileBase[] solidTiles = _solidTilemap.GetTilesBlock(_bounds);
        _tileTypeGrid = new TileType[_bounds.size.x, _bounds.size.y];

        for (int x = 0; x < _bounds.size.x; x++)
        {
            for (int y = 0; y < _bounds.size.y; y++)
            {
                TileBase wallTile = solidTiles[x + y * _bounds.size.x];
                if (wallTile != null)
                {
                    if (wallTile.name.Contains("building"))
                    {
                        _tileTypeGrid[x, y] = TileType.Building;
                    }
                    else if (wallTile.name.Contains("sand"))
                    {
                        _tileTypeGrid[x, y] = TileType.Sand;
                    }
                    else if (wallTile.name.Contains("resource_0"))
                    {
                        _tileTypeGrid[x, y] = TileType.Resource0;
                    }
                    else if (wallTile.name.Contains("resource_1"))
                    {
                        _tileTypeGrid[x, y] = TileType.Resource1;
                    }
                    else if (wallTile.name.Contains("resource_2"))
                    {
                        _tileTypeGrid[x, y] = TileType.Resource2;
                    }
                    else if (wallTile.name.Contains("resource_3"))
                    {
                        _tileTypeGrid[x, y] = TileType.Resource3;
                    }
                    else
                    {
                        Debug.LogWarning($"Unknown tile type at ({x}, {y}): {wallTile.name}");
                        _tileTypeGrid[x, y] = TileType.Sand; // Default to Sand if unknown
                    }
                    //Debug.Log($"Tile at ({x}, {y}): {_tileTypeGrid[x, y]}");
                }
            }
        }
    }

    /*public List<Vector2Int> CalculatePath(TileType[] traversableTileTypes, Vector2Int origin, Vector2Int target,
        bool excludePoint = false, Vector2Int excludedPoint = new Vector2Int(), bool seeBlockings = false)
    {
        List<Blocking> blockings = CollisionManager.Instance.Blockings;
        Dictionary<Vector2Int, float> traversableTilemap = new Dictionary<Vector2Int, float>();
        for (int x = 0; x < _bounds.size.x; x++)
        {
            for (int y = 0; y < _bounds.size.y; y++)
            {
                if (x == excludedPoint.x && y == excludedPoint.y && excludePoint) continue;
                if (seeBlockings && blockings.Any(b => b.IsEnabled && WorldToGridPos(b.transform.position) == new Vector2Int(x, y))) continue;
                if (traversableTileTypes.Contains(_tileTypeGrid[x, y])) traversableTilemap[new Vector2Int(x, y)] = 1f;
            }
        }
        Pathfinder2DResult pathfinderResult = new Pathfinder2D(traversableTilemap, NodeConnectionType.RectangleNoDiagonals)
            .FindPath(origin, target);
        pathfinderResult.Path.Reverse();
        //result.AddRange(pathfinderResult.Path);
        return pathfinderResult.Path;
    }
    */

    /*public List<Vector2Int> CalculatePath(TileType[] traversableTileTypes, Vector3 origin, Vector3 target,
        bool excludePoint = false, Vector2Int excludedPoint = new Vector2Int())
    {
        return CalculatePath(traversableTileTypes, WorldToGridPos(origin), WorldToGridPos(target), excludePoint, excludedPoint);
    }*/

    public Vector2Int WorldToGridPos(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x - _bounds.x),
            Mathf.RoundToInt(worldPosition.y - _bounds.y));
    }

    public Vector3 GridToWorldPos(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x + _bounds.x,
            gridPosition.y  + _bounds.y, 0f);
    }

    public Vector3 SnapToGrid(Vector3 worldPos)
    {
        return GridToWorldPos(WorldToGridPos(worldPos));
    }

    public TileType GetTileTypeAtPos(Vector2Int pos)
    {
        return _tileTypeGrid[pos.x, pos.y];
    }
    public TileType GetTileTypeAtPos(Vector3 pos)
    {
        return _tileTypeGrid[WorldToGridPos(pos).x, WorldToGridPos(pos).y];
    }

    public bool IsPosValid(Component comp, Vector3 worldPos)
    {
        TileType tileType = GetTileTypeAtPos(worldPos);
        return comp.ValidTiles.Contains(tileType);
    }
}

public enum TileType
{
    Sand,
    Building,
    Resource0,
    Resource1,
    Resource2,
    Resource3,
}