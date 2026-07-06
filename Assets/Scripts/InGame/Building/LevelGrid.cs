using System.Collections.Generic;
using System.Linq;
//using KwaaktjePathfinder2D;
using UnityEngine;
using UnityEngine.Tilemaps;

// This class generates a grid representation of the level based on a Tilemap and provides pathfinding functionality.
public class LevelGrid : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float _updateTime = 1f;


    [Header("References")]
    [SerializeField] private Tilemap _solidTilemap;
    [SerializeField] private Tile _plantTile;
    [SerializeField] private Tile _connectionTile;


    private TileType[,] _tileTypeGrid;
    private Directions[,] _spreadDirections;
    [SerializeField] [ReadOnly] private List<Tile> _plantSources = new List<Tile>();
    [SerializeField] [ReadOnly] private List<Tile> _plantConnections = new List<Tile>();
    private BoundsInt _bounds;
    private float _updateTimer;
    private Vector3Int _gridOrigin;

    #region Setup
    private void Awake()
    {
        //base.Awake();
        GenerateMovementGrid();
    }

    private void GenerateMovementGrid()
    {
        /*_cellSize = _solidTilemap.cellSize.x;
        _cellSizeInverse = 1f / _cellSize;*/
        _bounds = _solidTilemap.cellBounds;
        TileBase[] solidTiles = _solidTilemap.GetTilesBlock(_bounds);
        _tileTypeGrid = new TileType[_bounds.size.x, _bounds.size.y];
        _spreadDirections = new Directions[_bounds.size.x, _bounds.size.y];

        int tileCount = _solidTilemap.GetTiles(_bounds, out Tilemap.PositionArray positions, out Tilemap.TileArray tiles);

        _gridOrigin = positions[0];
        for (int x = 0; x < _bounds.size.x; x++)
        {
            for (int y = 0; y < _bounds.size.y; y++)
            {
                _spreadDirections[x, y] = new Directions();
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
                    else if (wallTile.name.Contains("plant"))
                    {
                        _tileTypeGrid[x, y] = TileType.PlantSource;
                        _plantSources.Add((Tile)wallTile);
                    }
                    else if (wallTile.name.Contains("connection"))
                    {
                        _tileTypeGrid[x, y] = TileType.PlantConnection;
                        //_plantConnections.Add((Tile)wallTile);
                    }
                    else
                    {
                        Debug.LogWarning($"Unknown tile type at ({x}, {y}): {wallTile.name}");
                        _tileTypeGrid[x, y] = TileType.Empty;
                    }
                }
                else _tileTypeGrid[x, y] = TileType.Empty;
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
    #endregion

    #region Interface
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

    public bool IsPosValid(MachineComponent comp, Vector3 worldPos)
    {
        TileType tileType = GetTileTypeAtPos(worldPos);
        return comp.ValidTiles.Contains(tileType);
    }
    #endregion

    private void Update()
    {
        _updateTimer += Time.deltaTime;
        if (_updateTimer >= _updateTime)
        {
            _updateTimer = 0f;
            UpdateGrid();
        }
    }

    private void UpdateGrid()
    {
        List<TileChange> tileChanges = new List<TileChange>();
        for (int x = 0; x < _bounds.size.x; x++)
        {
            for (int y = 0; y < _bounds.size.y; y++)
            {
                if (_tileTypeGrid[x, y] == TileType.Empty) { continue; }
                if (_tileTypeGrid[x, y] == TileType.PlantSource) 
                { 
                    SourceSpread(x, y, tileChanges);
                }
                if (_tileTypeGrid[x, y] == TileType.PlantConnection) 
                {
                    ConnectionSpread(x, y, tileChanges);
                }
            }             
        }
        foreach (TileChange change in tileChanges)
        {
            change.ApplyChange(this);
        }
    }

    /*private void Spread(int x, int y)
    {
        if (_tileTypeGrid[x, y] == TileType.PlantSource)
        {
            
        }
    }*/

    private void SourceSpread(int x, int y, List<TileChange> tileChanges)
    {
        if (Random.value > 0.5f) return;
        Directions availableDirections = new Directions();

        if (x > 0 && (_tileTypeGrid[x - 1, y] == TileType.Sand || _tileTypeGrid[x - 1, y] == TileType.PlantConnection))
        {
            availableDirections.DirectionsList.Add(Vector2Int.left);
        }
        if (x < _bounds.size.x - 1 && (_tileTypeGrid[x + 1, y] == TileType.Sand || _tileTypeGrid[x + 1, y] == TileType.PlantConnection))
        {
            availableDirections.DirectionsList.Add(Vector2Int.right);
        }
        if (y > 0 && (_tileTypeGrid[x, y - 1] == TileType.Sand || _tileTypeGrid[x, y - 1] == TileType.PlantConnection))
        {
            availableDirections.DirectionsList.Add(Vector2Int.down);
        }
        if (y < _bounds.size.y - 1 && (_tileTypeGrid[x, y + 1] == TileType.Sand || _tileTypeGrid[x, y + 1] == TileType.PlantConnection))
        {
            availableDirections.DirectionsList.Add(Vector2Int.up);
        }
        if (availableDirections.Count() == 0) return;
        Vector2Int direction = availableDirections.GetRandomDirection();
        tileChanges.Add(new TileChange
        {
            x = x + direction.x,
            y = y + direction.y,
            tile = _connectionTile,
            type = TileType.PlantConnection,
            spreadDir = direction
        });
    }

    private void ConnectionSpread(int x, int y, List<TileChange> tileChanges)
    {
        if (Random.value > 0.5f) return;
        Directions availableDirections = new Directions();

        if (x > 0 && (_tileTypeGrid[x - 1, y] == TileType.Sand || _tileTypeGrid[x - 1, y] == TileType.PlantConnection))
        {
            availableDirections.DirectionsList.Add(Vector2Int.left);
        }
        if (x < _bounds.size.x - 1 && (_tileTypeGrid[x + 1, y] == TileType.Sand || _tileTypeGrid[x + 1, y] == TileType.PlantConnection) )
        {
            availableDirections.DirectionsList.Add(Vector2Int.right);
        }
        if (y > 0 && (_tileTypeGrid[x, y - 1] == TileType.Sand || _tileTypeGrid[x, y - 1] == TileType.PlantConnection))
        {
            availableDirections.DirectionsList.Add(Vector2Int.down);
        }
        if (y < _bounds.size.y - 1 && (_tileTypeGrid[x, y + 1] == TileType.Sand || _tileTypeGrid[x, y + 1] == TileType.PlantConnection))
        {
            availableDirections.DirectionsList.Add(Vector2Int.up);
        }


        Directions validDirections = _spreadDirections[x, y];
        availableDirections.Filter(validDirections);

        if (availableDirections.Count() == 0) return;
        Vector2Int direction = availableDirections.GetRandomDirection();
        ChangeTile(x + direction.x, y + direction.y, _connectionTile, TileType.PlantConnection, direction);
    }

    private void ChangeTile(int x, int y, TileBase tile, TileType type, Vector2Int spreadDir)
    {
        //Debug.Log($"Changing tile at ({x}, {y}) to {type}");
        _solidTilemap.SetTile(new Vector3Int(x + _bounds.position.x, y + _bounds.position.y, 0), tile);
        _tileTypeGrid[x, y] = type;
        _spreadDirections[x, y].DirectionsList.Add(spreadDir);
    }

    public class TileChange
    {
        public int x;
        public int y;
        public TileBase tile;
        public TileType type;
        public Vector2Int spreadDir;

        public void ApplyChange(LevelGrid grid)
        {
            grid.ChangeTile(x, y, tile, type, spreadDir);
        }
    }
}



public enum TileType
{
    Empty,
    Sand,
    Building,
    Resource0,
    Resource1,
    Resource2,
    Resource3,
    PlantSource,
    PlantConnection,
}

public class Directions
{
    public List<Vector2Int> DirectionsList = new List<Vector2Int>();

    public int Count()
    {  
        return DirectionsList.Count; 
    }

    public Vector2Int GetRandomDirection()
    {
        if (DirectionsList.Count == 0) return Vector2Int.zero;
        return DirectionsList[Random.Range(0, DirectionsList.Count)];
    }

    public void Filter(Directions other)
    {
        List<Vector2Int> newList = new List<Vector2Int>();
        foreach (Vector2Int dir in DirectionsList)
        {
            if (other.DirectionsList.Contains(dir))
            {
                newList.Add(dir);
            }
        }
        DirectionsList = newList;
    }   
}