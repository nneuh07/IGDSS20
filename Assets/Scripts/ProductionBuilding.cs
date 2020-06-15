using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProductionBuilding : Building
{
    [SerializeField] public List<GameManager.ResourceTypes> inputResources = new List<GameManager.ResourceTypes>(); // Choice for input resources
    [SerializeField] public GameManager.ResourceTypes outputResource;

    [SerializeField]
    public int NumberJobs;

    // Start is called before the first frame update
    void Start()
    {
        _jobManager.AddJobs(Enumerable.Range(0, NumberJobs).Select(x => new Job(this)).ToList() );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
