using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

public class GameManager : MonoBehaviour
{
    #region Map generation

    private Tile[,] _tileMap; //2D array of all spawned tiles

    #endregion

    #region Buildings

    public GameObject[] _buildingPrefabs; //References to the building prefabs

    public JobManager _jobManager;

    public int
        _selectedBuildingPrefabIndex =
            0; //The current index used for choosing a prefab to spawn from the _buildingPrefabs list

    #endregion


    #region Resources

    float tick = 0f;

    private Dictionary<ResourceTypes, float>
        _resourcesInWarehouse =
            new Dictionary<ResourceTypes, float>(); //Holds a number of stored resources for every ResourceType

    //A representation of _resourcesInWarehouse, broken into individual floats. Only for display in inspector, will be removed and replaced with UI later
    [SerializeField] private float _ResourcesInWarehouse_Fish;
    [SerializeField] private float _ResourcesInWarehouse_Wood;
    [SerializeField] private float _ResourcesInWarehouse_Planks;
    [SerializeField] private float _ResourcesInWarehouse_Wool;
    [SerializeField] private float _ResourcesInWarehouse_Clothes;
    [SerializeField] private float _ResourcesInWarehouse_Potato;
    [SerializeField] private float _ResourcesInWarehouse_Schnapps;

    [SerializeField] private float income = 100f;
    [SerializeField] private float account;

    #endregion

    #region Enumerations

    public enum ResourceTypes
    {
        None,
        Fish,
        Wood,
        Planks,
        Wool,
        Clothes,
        Potato,
        Schnapps
    }; //Enumeration of all available resource types. Can be addressed from other scripts by calling GameManager.ResourceTypes

    #endregion

    #region MonoBehaviour

    // Start is called before the first frame update
    void Start()
    {
        PopulateResourceDictionary();
        account = income;
        var prefabs = GetPreFabs();
        var texture2D = Resources.Load<Texture2D>("Heightmap_16");
        if (texture2D == null)
            throw new ArgumentNullException(nameof(texture2D));
        _tileMap = new Tile[texture2D.width, texture2D.height];

        for (var x = 0; x < texture2D.width; x++)
        {
            for (var z = 0; z < texture2D.height; z++)
            {
                Vector3 vector3;
                if (x % 2 == 0)
                {
                    var evenstartingVector = new Vector3(0, 0, 0);
                    vector3 = new Vector3((float) 8.67 * x, 0, 10 * z);
                    vector3 = evenstartingVector + vector3;
                }
                else
                {
                    var unevenstartingVector = new Vector3((float) 8.67, 0, 5);
                    vector3 = new Vector3((float) 8.67 * (x - 1), 0, 10 * z);
                    vector3 = unevenstartingVector + vector3;
                }

                GameObject gameObject;
                var currentPixel = texture2D.GetPixel(x, z);

                if (Math.Abs(currentPixel.maxColorComponent) < 0.001)
                {
                    gameObject = prefabs.Single(p => p.name == "WaterTile");
                }
                else if (Math.Abs(currentPixel.maxColorComponent) < 0.2)
                {
                    gameObject = prefabs.Single(p => p.name == "SandTile");
                }
                else if (Math.Abs(currentPixel.maxColorComponent) < 0.4)
                {
                    gameObject = prefabs.Single(p => p.name == "GrassTile");
                }
                else if (Math.Abs(currentPixel.maxColorComponent) < 0.6)
                {
                    gameObject = prefabs.Single(p => p.name == "ForestTile");
                }
                else if (Math.Abs(currentPixel.maxColorComponent) < 0.8)
                {
                    gameObject = prefabs.Single(p => p.name == "StoneTile");
                }
                else
                {
                    gameObject = prefabs.Single(p => p.name == "MountainTile");
                }

                vector3.y = currentPixel.maxColorComponent * 40;
                var newGameObject = Instantiate(gameObject, vector3, Quaternion.identity);
                var tile = newGameObject.GetComponent<Tile>();
                tile._coordinateWidth = x;
                tile._coordinateHeight = z;
                _tileMap[x, z] = tile;
            }
        }

        for (var x = 0; x < _tileMap.GetLength(0); x++)
        {
            for (var z = 0; z < _tileMap.GetLength(1); z++)
            {
                var tile = _tileMap[x, z];
                tile._neighborTiles = FindNeighborsOfTile(tile);
            }
        }

        StartEconomy();
    }

    private static IEnumerable<GameObject> GetPreFabs()
    {
        var prefabArray = new[] {"WaterTile", "ForestTile", "GrassTile", "MountainTile", "SandTile", "StoneTile"};
        var returnList = prefabArray.Select(prefab => Resources.Load(prefab, typeof(GameObject)) as GameObject)
            .ToList();
        if (returnList.Any(element => element == null))
        {
            throw new ArgumentException();
        }

        return returnList;
    }

    // Update is called once per frame
    void Update()
    {
        HandleKeyboardInput();
        StartEconomy();

        UpdateInspectorNumbersForResources();
    }

    private void StartEconomy()
    {
        tick += Time.deltaTime;
        if (!(tick >= 1f)) return;
        tick %= 1f;
        RunMoneyCycle();
    }

    private void RunMoneyCycle()
    {
        account += income / 60;
        account += _jobManager.getNumberOfWorkers() * 50 / 60;
        RunBuildingCycle();
    }

    private void RunBuildingCycle()
    {
        foreach (var tile in _tileMap)
        {
            if (tile.Building == null) continue;
            var upkeep = tile.Building.upkeep;
            if (!(account >= upkeep)) continue;
            account -= upkeep / 60;
            if (tile.Building.type != Building.BuildingTypes.House)
            {
                RunProductionCycle(tile.Building as ProductionBuilding);
            }
        }
    }

    private void RunProductionCycle(ProductionBuilding productionBuilding)
    {
        if (productionBuilding.efficiencyScalesWith != Tile.TileTypes.Empty)
        {
            var count = productionBuilding.tileReference._neighborTiles.Count(x =>
                x._type == productionBuilding.efficiencyScalesWith &&
                x.Building == null);
            if (count < productionBuilding.minNeighbours)
            {
                productionBuilding.efficiencyValue = 0f;
            }
            else if (count >= productionBuilding.maxNeighbours)
            {
                productionBuilding.efficiencyValue = 1f;
            }
            else
            {
                productionBuilding.efficiencyValue = count / productionBuilding.maxNeighbours;
            }
        }

        productionBuilding.efficiencyValue *= productionBuilding.AverageWorkerHappiness();
        productionBuilding.efficiencyValue *= productionBuilding._workers.Count / (float)productionBuilding.NumberJobs;

        var productionEfficiency = productionBuilding.resourceInterval / productionBuilding.efficiencyValue;

        if (productionBuilding.inputResources != null)
        {
            if (productionBuilding.inputResources.All(HasResourceInWarehoues))
            {
                foreach (var res in productionBuilding.inputResources)
                    _resourcesInWarehouse[res] -= 1;
            }
            else
            {
                return;
            }
        }

        _resourcesInWarehouse[productionBuilding.outputResource] += productionBuilding.output / productionEfficiency;
    }

    #endregion

    #region Methods

    //Makes the resource dictionary usable by populating the values and keys
    void PopulateResourceDictionary()
    {
        _resourcesInWarehouse.Add(ResourceTypes.None, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Fish, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Wood, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Planks, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Wool, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Clothes, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Potato, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Schnapps, 0);
    }

    //Sets the index for the currently selected building prefab by checking key presses on the numbers 1 to 0
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _selectedBuildingPrefabIndex = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _selectedBuildingPrefabIndex = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _selectedBuildingPrefabIndex = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _selectedBuildingPrefabIndex = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _selectedBuildingPrefabIndex = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            _selectedBuildingPrefabIndex = 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            _selectedBuildingPrefabIndex = 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            _selectedBuildingPrefabIndex = 7;
        }
        //Cheat Code
        else if (Input.GetKeyDown(KeyCode.A))
        {
            account = 10000f;
            _resourcesInWarehouse[ResourceTypes.Fish] = 1000f;
            _resourcesInWarehouse[ResourceTypes.Wood] = 1000f;
            _resourcesInWarehouse[ResourceTypes.Planks] = 1000f;
            _resourcesInWarehouse[ResourceTypes.Wool] = 1000f;
            _resourcesInWarehouse[ResourceTypes.Clothes] = 1000f;
            _resourcesInWarehouse[ResourceTypes.Potato] = 1000f;
            _resourcesInWarehouse[ResourceTypes.Schnapps] = 1000f;
        }
    }

    //Updates the visual representation of the resource dictionary in the inspector. Only for debugging
    void UpdateInspectorNumbersForResources()
    {
        _ResourcesInWarehouse_Fish = _resourcesInWarehouse[ResourceTypes.Fish];
        _ResourcesInWarehouse_Wood = _resourcesInWarehouse[ResourceTypes.Wood];
        _ResourcesInWarehouse_Planks = _resourcesInWarehouse[ResourceTypes.Planks];
        _ResourcesInWarehouse_Wool = _resourcesInWarehouse[ResourceTypes.Wool];
        _ResourcesInWarehouse_Clothes = _resourcesInWarehouse[ResourceTypes.Clothes];
        _ResourcesInWarehouse_Potato = _resourcesInWarehouse[ResourceTypes.Potato];
        _ResourcesInWarehouse_Schnapps = _resourcesInWarehouse[ResourceTypes.Schnapps];
    }

    //Checks if there is at least one material for the queried resource type in the warehouse
    public bool HasResourceInWarehoues(ResourceTypes resource)
    {
        return _resourcesInWarehouse[resource] >= 1;
    }

    public bool HasEnoughResourcesInWarehoues(ResourceTypes resource, float plankCost)
    {
        return _resourcesInWarehouse[resource] >= plankCost;
    }


    //Is called by MouseManager when a tile was clicked
    //Forwards the tile to the method for spawning buildings
    public void TileClicked(Tile tile)
    {
        PlaceBuildingOnTile(tile);
    }

    //Checks if the currently selected building type can be placed on the given tile and then instantiates an instance of the prefab
    private void PlaceBuildingOnTile(Tile t)
    {
        //if there is building prefab for the number input4
        if (_selectedBuildingPrefabIndex < _buildingPrefabs.Length)
        {
            var preFab = _buildingPrefabs[_selectedBuildingPrefabIndex].GetComponent<Building>();
            if (t != null && t.Building == null && preFab.canBeBuiltOn.Contains(t._type) &&
                HasEnoughResourcesInWarehoues(ResourceTypes.Planks, preFab.plankCost) && account >= preFab.buildCost)
            {
                var newBuildingGameObject =
                    Instantiate(_buildingPrefabs[_selectedBuildingPrefabIndex], t.gameObject.transform);

                var b = newBuildingGameObject.GetComponent<Building>();
                t.Building = b;
                b.tileReference = t;
                b._jobManager = _jobManager;
                b._GameManager = this;

                account -= b.buildCost;
                _resourcesInWarehouse[ResourceTypes.Planks] -= b.plankCost;
            }
        }
    }

    //Returns a list of all neighbors of a given tile --> Aber wie starte ich das??? 
    private List<Tile> FindNeighborsOfTile(Tile t)
    {
        var neighborList = new List<Tile>();

        if (t._coordinateWidth + 1 < _tileMap.GetLength(0))
            neighborList.Add(_tileMap[t._coordinateHeight, t._coordinateWidth + 1]);

        if (t._coordinateHeight - 1 >= 0)
            neighborList.Add(_tileMap[t._coordinateHeight - 1, t._coordinateWidth]);

        if (t._coordinateWidth - 1 >= 0)
            neighborList.Add(_tileMap[t._coordinateHeight, t._coordinateWidth - 1]);

        if (t._coordinateWidth - 1 >= 0 && t._coordinateHeight - 1 >= 0)
            neighborList.Add(_tileMap[t._coordinateHeight - 1, t._coordinateWidth - 1]);

        if (t._coordinateHeight + 1 < _tileMap.GetLength(1))
            neighborList.Add(_tileMap[t._coordinateHeight + 1, t._coordinateWidth]);


        if (t._coordinateWidth + 1 < _tileMap.GetLength(0) && t._coordinateHeight + 1 < _tileMap.GetLength(1))
            neighborList.Add(_tileMap[t._coordinateHeight + 1, t._coordinateWidth + 1]);

        return neighborList;
    }

    #endregion

    public bool WorkerConsumed()
    {
        if (!(_resourcesInWarehouse[ResourceTypes.Clothes] > 0.1) ||
            !(_resourcesInWarehouse[ResourceTypes.Fish] > 0.1) ||
            !(_resourcesInWarehouse[ResourceTypes.Schnapps] > 0.1)) return false;
        _ResourcesInWarehouse_Clothes -= 0.1f;
        _ResourcesInWarehouse_Fish -= 0.1f;
        _ResourcesInWarehouse_Schnapps -= 0.1f;
        return true;
    }
}