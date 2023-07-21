using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int width;
    private int height;
    private int length;
    private float cellSize;

    private int[,,] gridArray;

    private GameObject[,] boardObjectArray;
    public Grid(int width, int height, int length, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.length = length;
        this.cellSize = cellSize;

        gridArray = new int[width, height, length];
        boardObjectArray = new GameObject[width, height];


        for (int x = -width + 1; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                for (int z = -length + 1; z < gridArray.GetLength(2); z++)
                {
                    //RandomWorldGenerator.singleton.CreateATileAtXYZ(GetWorldPositionMiddle(x, y, z), x, y, z);
                    RandomWorldGenerator.singleton.CreateATileSlot(GetWorldPositionMiddle(x, y, z), x, y, z);
                }
            }
        }

    }

    private Vector3 GetWorldPosition(int x, int y, int z)
    {
        return new Vector3(x, y, z) * cellSize;
    }
    public Vector3 GetWorldPositionMiddle(int x, int y, int z)
    {
        return new Vector3(x, y, z) * cellSize + new Vector3(cellSize, cellSize, cellSize) * .5f;
    }

    public Vector3Int GetXYZ(Vector3 worldPosition)
    {
        int xv = Mathf.FloorToInt(worldPosition.x / cellSize);
        int zv = Mathf.FloorToInt(worldPosition.z / cellSize);
        int yv = Mathf.FloorToInt(worldPosition.y / cellSize);
        return new Vector3Int(xv,yv,zv);
    }

    public void SetValue(int x, int y, int z, int value)
    {
        if (x >= 0 && z >= 0 && x < width && z < height)
        {
            gridArray[x,y,z] = value;
        }
    }



}
