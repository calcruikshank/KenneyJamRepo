using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomWorldGenerator : MonoBehaviour
{
    public Grid grid;

    public static RandomWorldGenerator singleton;

    [SerializeField] List<Tile> tiles = new List<Tile>();
    Dictionary<Vector3Int, Tile> addedTiles = new Dictionary<Vector3Int, Tile>();
    


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
        grid = new Grid(24, 24, 1);

    }

    public void CreateATileAtXYZ(Vector3 worldSpace, int x, int y, int z)
    {
        GameObject instantiatedBoardSpace = Instantiate(tiles[0], worldSpace, Quaternion.identity).gameObject;
        Tile tileToAdd = instantiatedBoardSpace.GetComponent<Tile>();
        //tileToAdd.Initialize(vector3, x, z);
        addedTiles.Add(new Vector3Int(x, y, z), tileToAdd);
    }

}
