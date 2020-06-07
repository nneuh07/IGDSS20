using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public enum BuildingTypes { Fishery, Lumberjack, Sawmill, SheepFarm, FrameworkKnitters, PotatoFarm, SchnappsDistillery };
    
    [SerializeField] public BuildingTypes type; //Type of Building
    [SerializeField] public float upkeep; // Cost per minute
    [SerializeField] public float buildCost; //Money for building
    [SerializeField] public float plankCost; // Placement costs
    [SerializeField] public Tile tileReference; // Reference to tile
    [SerializeField] public float efficiencyValue = 1f; //Efficiency based on surrounding tiles 
    [SerializeField] public float resourceInterval = 0f; //If operation on 100% efficiency
    [SerializeField] public float output; //Number of output per cycle
    [SerializeField] public List<Tile.TileTypes> tiles; //Which tiles it can be built on
    [SerializeField] public float minNeighbours; //Minimum neighbours
    [SerializeField] public float maxNeighbours; //Max neighbour 
    [SerializeField] public List<GameManager.ResourceTypes> inputResources = new List<GameManager.ResourceTypes>(); // Choice for input resources
    [SerializeField] public GameManager.ResourceTypes outputResource;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
