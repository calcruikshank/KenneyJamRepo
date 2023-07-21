using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWorldGenerator : MonoBehaviour
{
    public Grid grid;

    public static RandomWorldGenerator singleton;

    [SerializeField] public List<Tile> tiles = new List<Tile>();
    public Dictionary<Vector3Int, TileSlot> tileSlots = new Dictionary<Vector3Int, TileSlot>();
    public Dictionary<Vector3Int, Tile> addedTiles = new Dictionary<Vector3Int, Tile>();

    private void Awake()
    {
        if (singleton != null)
        {
            Destroy(this);
        }
        singleton = this;
    }
    private void Start()
    {
        grid = new Grid(24, 1, 24, 1);
        InitializeNeighbors();

        InitializeStartingTile();

        SpawnTiles();
    }

    private void SpawnTiles()
    {

        for (int i = 0; i < 3000; i++)
        {
            TileSlot ts = GetTileSlotWithLowestNumberOfOptions();

            if (ts != null)
            {
                CreateATileAtXYZ(grid.GetWorldPositionMiddle(ts.id.x, ts.id.y, ts.id.z), ts.id.x, ts.id.y, ts.id.z, ts.GetRandomViableType());
            }
        }
       
    }

    private TileSlot GetTileSlotWithLowestNumberOfOptions()
    {
        int currentLowest = int.MaxValue;
        TileSlot currentLowestTile = new TileSlot();
        foreach (KeyValuePair<Vector3Int, TileSlot> tileSlot in tileSlots)
        {
            if (tileSlot.Value.tileInSlot == null)
            {
                tileSlot.Value.SetViableTilesFromEachDirection();
            }
        }

        foreach (KeyValuePair<Vector3Int, TileSlot> tileSlot in tileSlots)
        {
            if (tileSlot.Value.tileInSlot == null)
            {
                if (tileSlot.Value.allViableOptions.Count < currentLowest && tileSlot.Value.allViableOptions.Count > 0)
                {
                    if (!addedTiles.ContainsKey(tileSlot.Key))
                    {
                        currentLowestTile = tileSlot.Value;
                        currentLowest = tileSlot.Value.allViableOptions.Count;
                    }
                }
            }
        }
        if (addedTiles.ContainsKey(currentLowestTile.id))
        {
            Debug.LogError("xra");
            return null;
        }
        return currentLowestTile;
    }

    private void InitializeStartingTile()
    {
        CreateATileAtXYZ(grid.GetWorldPositionMiddle(0, 0, 0), 0, 0, 0, tiles[1]);
    }

    private void InitializeNeighbors()
    {
        foreach (KeyValuePair<Vector3Int, TileSlot> tile in tileSlots)
        {
            tile.Value.InitializeTileSlot(tile.Key);
        }
    }

    internal void CreateATileSlot(Vector3 vector3, int x, int y, int z)
    {
        TileSlot undecidedTile = new TileSlot();
        //tileToAdd.Initialize(vector3, x, z);
        tileSlots.Add(new Vector3Int(x, y, z), undecidedTile);
    }

    public void CreateATileAtXYZ(Vector3 worldSpace, int x, int y, int z, Tile tileType)
    {
        GameObject instantiatedBoardSpace = Instantiate(tileType, worldSpace, Quaternion.identity).gameObject;
        Tile tileToAdd = instantiatedBoardSpace.GetComponent<Tile>();

        tileSlots[new Vector3Int(x, y, z)].tileInSlot = tileToAdd;

        addedTiles.Add(new Vector3Int(x, y, z), tileToAdd);
    }


}
