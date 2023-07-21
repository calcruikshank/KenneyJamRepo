using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] public List<Tile> viableTilesNorth;
    [SerializeField] public List<Tile> viableTilesEast;
    [SerializeField] public List<Tile> viableTileWest;
    [SerializeField] public List<Tile> viableTilesSouth;
    [SerializeField] public List<Tile> viableTilesDown;
    [SerializeField] public List<Tile> viableTilesUp;


    
}
