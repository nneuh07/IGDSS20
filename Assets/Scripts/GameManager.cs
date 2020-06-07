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
    public int _selectedBuildingPrefabIndex = 0; //The current index used for choosing a prefab to spawn from the _buildingPrefabs list
    #endregion


    #region Resources
    private Dictionary<ResourceTypes, float> _resourcesInWarehouse = new Dictionary<ResourceTypes, float>(); //Holds a number of stored resources for every ResourceType

    //A representation of _resourcesInWarehouse, broken into individual floats. Only for display in inspector, will be removed and replaced with UI later
    [SerializeField]
    private float _ResourcesInWarehouse_Fish;
    [SerializeField]
    private float _ResourcesInWarehouse_Wood;
    [SerializeField]
    private float _ResourcesInWarehouse_Planks;
    [SerializeField]
    private float _ResourcesInWarehouse_Wool;
    [SerializeField]
    private float _ResourcesInWarehouse_Clothes;
    [SerializeField]
    private float _ResourcesInWarehouse_Potato;
    [SerializeField]
    private float _ResourcesInWarehouse_Schnapps;
    #endregion

    #region Enumerations
    public enum ResourceTypes { None, Fish, Wood, Planks, Wool, Clothes, Potato, Schnapps }; //Enumeration of all available resource types. Can be addressed from other scripts by calling GameManager.ResourceTypes
    #endregion

    #region MonoBehaviour
    
    // Start is called before the first frame update
    void Start()
    {
        PopulateResourceDictionary();
        var prefabs = GetPreFabs();
        var texture2D = Resources.Load<Texture2D>("Heightmap_16");
        if (texture2D == null)
            throw new ArgumentNullException(nameof(texture2D));
        for (var x = 0; x < texture2D.width; x++)
        {
            for (var z = 0; z < texture2D.height; z++)
            {
                Vector3 vector3;
                if (x % 2 == 0)
                {
                    Vector3 evenstartingVector = new Vector3(0, 0, 0);
                    vector3 = new Vector3((float) 8.67 * x, 0, 10 * z);
                    vector3 = evenstartingVector + vector3;
                }
                else
                {
                    Vector3 unevenstartingVector = new Vector3((float) 8.67, 0, 5);
                    vector3 = new Vector3((float) 8.67 * (x - 1), 0, 10 * z);
                    vector3 = unevenstartingVector + vector3;
                }

                Tile newTile = null;
                var currentPixel = texture2D.GetPixel(x, z);

                if (Math.Abs(currentPixel.maxColorComponent) < 0.001)
                {
                    newTile = new Tile
                    {
                        _gameObject = prefabs.Single(p => p.name == "WaterTile"),
                        _type = Tile.TileTypes.Water
                    };
                }
                else if (Math.Abs(currentPixel.maxColorComponent) < 0.2)
                {
                    newTile = new Tile
                    {
                        _gameObject = prefabs.Single(p => p.name == "SandTile"),
                        _type = Tile.TileTypes.Sand
                    };
                }
                else if (Math.Abs(currentPixel.maxColorComponent) < 0.4)
                {
                    newTile = new Tile
                    {
                        _gameObject = prefabs.Single(p => p.name == "GrassTile"),
                        _type = Tile.TileTypes.Grass
                    };
                }
                else if (Math.Abs(currentPixel.maxColorComponent) < 0.6)
                {
                    newTile = new Tile
                    {
                        _gameObject = prefabs.Single(p => p.name == "ForestTile"),
                        _type = Tile.TileTypes.Forest
                    };
                }
                else if (Math.Abs(currentPixel.maxColorComponent) < 0.8)
                {
                    newTile = new Tile
                    {
                        _gameObject = prefabs.Single(p => p.name == "StoneTile"),
                        _type = Tile.TileTypes.Stone
                    };
                }
                else
                {
                    newTile = new Tile
                    {
                        _gameObject = prefabs.Single(p => p.name == "MountainTile"),
                        _type = Tile.TileTypes.Mountain
                    };
                }
                
                //Debug.Log("Currently at: " + newTile._type);
                
                vector3.y = currentPixel.maxColorComponent * 40;
                Instantiate(newTile._gameObject, vector3, Quaternion.identity);
                
                 //_tileMap[x, z] = newTile; --> Error hier ?!?
                
            }
            
        }
        
        for (var x = 0; x < _tileMap.GetLength(0); x++)
        {
            for (var z = 0; z < _tileMap.GetLength(1); z++)
            {
                FindNeighborsOfTile(_tileMap[x, z]);
            }
        }

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
        UpdateInspectorNumbersForResources();
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
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            _selectedBuildingPrefabIndex = 8;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            _selectedBuildingPrefabIndex = 9;
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

    //Is called by MouseManager when a tile was clicked
    //Forwards the tile to the method for spawning buildings
    public void TileClicked(int height, int width)
    {
        Tile t = _tileMap[height, width];
        PlaceBuildingOnTile(t);
    }

    //Checks if the currently selected building type can be placed on the given tile and then instantiates an instance of the prefab
    private void PlaceBuildingOnTile(Tile t)
    {
        //if there is building prefab for the number input
        if (_selectedBuildingPrefabIndex < _buildingPrefabs.Length)
        {
            //TODO: check if building can be placed and then istantiate it

        }
    }

    //Returns a list of all neighbors of a given tile --> Aber wie starte ich das??? 
    private List<Tile> FindNeighborsOfTile(Tile t)
    {
        List<Tile> result = new List<Tile>();
        Tile[] tiles = FindObjectsOfType<Tile>();

        foreach (Tile tile in tiles)
        {
            if (tile.gameObject.GetInstanceID() != gameObject.GetInstanceID())
            {
                if (tile.col3d.bounds.Intersects(tile.gameObject.GetComponent<SphereCollider>().bounds))
                {
                    Debug.Log("[" + gameObject.name + "] found neighbour: " + tile.gameObject.name);
                    result.Add(tile);
                }
            }
        }
        return result;
    }
    #endregion

}