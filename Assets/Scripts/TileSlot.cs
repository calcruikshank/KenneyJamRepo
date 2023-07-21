using System;
using System.Collections.Generic;
using UnityEngine;

public class TileSlot
{
    public TileSlot neighborTileSlots;
    public Vector3Int id;
    List<TileSlot> neighborTiles = new List<TileSlot>();

    public Tile tileInSlot;
    internal void InitializeTileSlot(Vector3Int v3)
    {
        id = v3;
        if (RandomWorldGenerator.singleton.tileSlots.ContainsKey(new Vector3Int(id.x + 1, id.y, id.z)))
        {
            neighborTiles.Add(RandomWorldGenerator.singleton.tileSlots[new Vector3Int(id.x + 1, id.y, id.z)]);
        }
        if (RandomWorldGenerator.singleton.tileSlots.ContainsKey(new Vector3Int(id.x, id.y, id.z + 1)))
        {
            neighborTiles.Add(RandomWorldGenerator.singleton.tileSlots[new Vector3Int(id.x, id.y, id.z + 1)]);
        }
        if (RandomWorldGenerator.singleton.tileSlots.ContainsKey(new Vector3Int(id.x, id.y, id.z - 1)))
        {
            neighborTiles.Add(RandomWorldGenerator.singleton.tileSlots[new Vector3Int(id.x, id.y, id.z - 1)]);
        }
        if (RandomWorldGenerator.singleton.tileSlots.ContainsKey(new Vector3Int(id.x - 1, id.y, id.z)))
        {
            neighborTiles.Add(RandomWorldGenerator.singleton.tileSlots[new Vector3Int(id.x - 1, id.y, id.z)]);
        }
        if (RandomWorldGenerator.singleton.tileSlots.ContainsKey(new Vector3Int(id.x, id.y + 1, id.z)))
        {
            neighborTiles.Add(RandomWorldGenerator.singleton.tileSlots[new Vector3Int(id.x, id.y + 1, id.z)]);
        }
        if (RandomWorldGenerator.singleton.tileSlots.ContainsKey(new Vector3Int(id.x, id.y - 1, id.z)))
        {
            neighborTiles.Add(RandomWorldGenerator.singleton.tileSlots[new Vector3Int(id.x, id.y - 1, id.z)]);
        }
    }

    internal Tile GetRandomViableType()
    {
        return allViableOptions[UnityEngine.Random.Range(0, allViableOptions.Count)];
    }

    public List<Tile> currentViableTilesFromSouth = new List<Tile>();
    public List<Tile> currentViableTilesFromNorth = new List<Tile>();
    public List<Tile> currentViableTilesFromWest = new List<Tile>();
    public List<Tile> currentViableTilesFromEast = new List<Tile>();
    public List<Tile> allViableOptions = new List<Tile>();

    Tile tileSouth;
    Tile tileNorth;
    Tile tileWest;
    Tile tileEast;
    public void SetViableTilesFromEachDirection()
    {
        allViableOptions = new List<Tile>();
        currentViableTilesFromSouth = new List<Tile>();
        currentViableTilesFromNorth = new List<Tile>();
        currentViableTilesFromWest = new List<Tile>();
        currentViableTilesFromEast = new List<Tile>();

        foreach (TileSlot tileSlot in neighborTiles)
        {
            if (tileSlot.tileInSlot != null)
            {
                if (tileSlot.id == new Vector3Int(id.x - 1, id.y, id.z))
                {
                    currentViableTilesFromEast = tileSlot.tileInSlot.viableTilesEast;
                    tileEast = tileSlot.tileInSlot;
                }
                if (tileSlot.id == new Vector3Int(id.x, id.y, id.z - 1))
                {
                    currentViableTilesFromNorth = tileSlot.tileInSlot.viableTilesNorth;
                    tileNorth = tileSlot.tileInSlot;
                }
                if (tileSlot.id == new Vector3Int(id.x + 1, id.y, id.z))
                {
                    currentViableTilesFromWest = tileSlot.tileInSlot.viableTileWest;
                    tileWest = tileSlot.tileInSlot;
                }
                if (tileSlot.id == new Vector3Int(id.x, id.y, id.z + 1))
                {
                    currentViableTilesFromSouth = tileSlot.tileInSlot.viableTilesSouth;
                    tileSouth = tileSlot.tileInSlot;
                }
            }
            else
            {
                if (tileSlot.id == new Vector3Int(id.x - 1, id.y, id.z))
                {
                    tileEast = null;
                }
                if (tileSlot.id == new Vector3Int(id.x, id.y, id.z - 1))
                {
                    tileNorth = null;
                }
                if (tileSlot.id == new Vector3Int(id.x + 1, id.y, id.z))
                {
                    tileWest = null;
                }
                if (tileSlot.id == new Vector3Int(id.x, id.y, id.z + 1))
                {
                    tileSouth = null;
                }
            }

            //TODO Up nd down


            foreach (Tile t in currentViableTilesFromEast)
            {
                if (!allViableOptions.Contains(t))
                {
                    allViableOptions.Add(t);
                }
            }
            foreach (Tile t in currentViableTilesFromWest)
            {
                if (!allViableOptions.Contains(t))
                {
                    allViableOptions.Add(t);
                }
            }
            foreach (Tile t in currentViableTilesFromNorth)
            {
                if (!allViableOptions.Contains(t))
                {
                    allViableOptions.Add(t);
                }
            }
            foreach (Tile t in currentViableTilesFromSouth)
            {
                if (!allViableOptions.Contains(t))
                {
                    allViableOptions.Add(t);
                }
            }

            /*foreach (Tile t in currentViableTilesFromEast)
            {
                if (tileWest != null && !currentViableTilesFromWest.Contains(t))
                {
                    allViableOptions.Remove(t);
                }
                if (tileSouth != null && !currentViableTilesFromSouth.Contains(t))
                {
                    allViableOptions.Remove(t);
                }
                if (tileNorth != null && !currentViableTilesFromNorth.Contains(t))
                {
                    allViableOptions.Remove(t);
                }
                if (tileEast != null && !currentViableTilesFromEast.Contains(t))
                {
                    allViableOptions.Remove(t);
                }
            }
            foreach (Tile t in currentViableTilesFromWest)
            {
                if (tileWest != null && !currentViableTilesFromWest.Contains(t))
                {
                    allViableOptions.Remove(t);
                }
                if (tileSouth != null && !currentViableTilesFromSouth.Contains(t))
                {
                    allViableOptions.Remove(t);
                }
                if (tileNorth != null && !currentViableTilesFromNorth.Contains(t))
                {
                    allViableOptions.Remove(t);
                }
                if (tileEast != null && !currentViableTilesFromEast.Contains(t))
                {
                    allViableOptions.Remove(t);
                }
            }
            foreach (Tile t in currentViableTilesFromNorth)
            {
                if (tileWest != null && !currentViableTilesFromWest.Contains(t))
                {
                    allViableOptions.Remove(t);
                }
                if (tileSouth != null && !currentViableTilesFromSouth.Contains(t))
                {
                    allViableOptions.Remove(t);
                }
                if (tileNorth != null && !currentViableTilesFromNorth.Contains(t))
                {
                    allViableOptions.Remove(t);
                }
                if (tileEast != null && !currentViableTilesFromEast.Contains(t))
                {
                    allViableOptions.Remove(t);
                }
            }
            foreach (Tile t in currentViableTilesFromSouth)
            {
                if (tileWest != null && !currentViableTilesFromWest.Contains(t))
                {
                    allViableOptions.Remove(t);
                }
                if (tileSouth != null && !currentViableTilesFromSouth.Contains(t))
                {
                    allViableOptions.Remove(t);
                }
                if (tileNorth != null && !currentViableTilesFromNorth.Contains(t))
                {
                    allViableOptions.Remove(t);
                }
                if (tileEast != null && !currentViableTilesFromEast.Contains(t))
                {
                    allViableOptions.Remove(t);
                }
            }
            */

        }
    }
}