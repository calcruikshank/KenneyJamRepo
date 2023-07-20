using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int width;
    private int height;
    private float cellSize;

    private int[,] gridArray;

    private GameObject[,] boardObjectArray;
    public Grid(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridArray = new int[width, height];
        boardObjectArray = new GameObject[width, height];

        for (int x = -width; x < gridArray.GetLength(0); x++)
        {
            for (int z = -height; z < gridArray.GetLength(1); z++)
            {
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x,z + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1,z), Color.white, 100f);
                RandomWorldGenerator.singleton.CreateATileAtXYZ(GetWorldPositionMiddle(x, z), x, 0, z);   //We can add a y later but its 0 for now 
            }
        }

        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
    }


    private Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x, 0, z) * cellSize;
    }
    public Vector3 GetWorldPositionMiddle(int x, int z)
    {
        return new Vector3(x, 0, z) * cellSize + new Vector3(cellSize, 0, cellSize) * .5f;
    }

    public Vector2Int GetXZ(Vector3 worldPosition)
    {
        int xv = Mathf.FloorToInt(worldPosition.x / cellSize);
        int zv = Mathf.FloorToInt(worldPosition.z / cellSize);
        return new Vector2Int(xv,zv);
    }

    public void SetValue(int x, int z, int value)
    {
        if (x >= 0 && z >= 0 && x < width && z < height)
        {
            gridArray[x, z] = value;
        }
    }



}
