using System;
using System.Collections.Generic;
using UnityEngine;


public class HousingBuilding : Building
{
    public GameObject _characterPrefab;
    private float tick = 0f;

    public void Start()
    {
        _jobManager = GetComponent<JobManager>();
        var newCharacterGameObject =
            Instantiate(_characterPrefab, transform);
        var worker = newCharacterGameObject.GetComponent<Worker>();
        _workers.Add(worker);
        _jobManager._unoccupiedWorkers.Add(worker);
        newCharacterGameObject =
            Instantiate(_characterPrefab, transform);
        worker = newCharacterGameObject.GetComponent<Worker>();
        _workers.Add(worker);
        _jobManager._unoccupiedWorkers.Add(worker);
    }

    public void Update()
    {
        tick += Time.deltaTime;
        if (!(tick >= 15f)) return;
        tick %= 15f;
        //TODO: Spawn Persons 
    }
}