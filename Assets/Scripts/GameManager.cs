using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
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
                Instantiate(gameObject, vector3, Quaternion.identity);
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
    }
}