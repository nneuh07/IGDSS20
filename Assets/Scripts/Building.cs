using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : MonoBehaviour
{
    public enum BuildingTypes
    {
        House,
        Fishery,
        Lumberjack,
        Sawmill,
        SheepFarm,
        FrameworkKnitters,
        PotatoFarm,
        SchnappsDistillery
    };

    [SerializeField] public Building.BuildingTypes type; //Type of Building
    [SerializeField] public float upkeep; // Cost per minute
    [SerializeField] public float buildCost; //Money for building
    [SerializeField] public float plankCost; // Placement costs
    [SerializeField] public Tile tileReference; // Reference to tile
    [SerializeField] public float efficiencyValue = 1f; //Efficiency based on surrounding tiles 
    [SerializeField] public float resourceInterval = 0f; //If operation on 100% efficiency
    [SerializeField] public float output; //Number of output per cycle
    [SerializeField] public List<Tile.TileTypes> canBeBuiltOn; //Which tiles it can be built on
    [SerializeField] public Tile.TileTypes efficiencyScalesWith;
    [SerializeField] public float minNeighbours; //Minimum neighbours
    [SerializeField] public float maxNeighbours; //Max neighbour 

    #region Manager References
    protected JobManager _jobManager; //Reference to the JobManager


    #endregion

    #region Workers

    public List<Worker> _workers; //List of all workers associated with this building, either for work or living

    #endregion

    #region Jobs

    public List<Job> _jobs; // List of all available Jobs. Is populated in Start()

    #endregion


    #region Methods

    public void WorkerAssignedToBuilding(Worker w)
    {
        _workers.Add(w);
    }

    public void WorkerRemovedFromBuilding(Worker w)
    {
        _workers.Remove(w);
    }

    #endregion
}