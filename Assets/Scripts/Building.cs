using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] public String type;
    [SerializeField] public int upkeep;
    [SerializeField] public int buildCost;
    [SerializeField] public int plankCost;
    [SerializeField] public Tile tileReference;
    [SerializeField] public int efficiencyValue;
    [SerializeField] public int resourceInterval;
    [SerializeField] public int output;
    [SerializeField] public List<Tile> tiles;
    [SerializeField] public int minNeighbours;
    [SerializeField] public int maxNeighbours;
    [SerializeField] public Resources inputResource;
    [SerializeField] public Resources outputResource;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
