using System;
using System.Collections.Generic;
using UnityEngine;


public class HousingBuilding : Building
{
    public GameObject _characterPrefab;
    public int maximalResidents = 10;
    private float tick = 0f;

    public void Start()
    {
        var newCharacterGameObject =
            Instantiate(_characterPrefab, transform);
        var worker = newCharacterGameObject.GetComponent<Worker>();
        _workers.Add(worker);
        worker._jobManager = _jobManager;
        worker._gameManager = _GameManager;
        worker._age = 14f;
        worker._happiness = 1f;
        _jobManager._unoccupiedWorkers.Add(worker);
        newCharacterGameObject =
            Instantiate(_characterPrefab, transform);
        worker = newCharacterGameObject.GetComponent<Worker>();
        _workers.Add(worker);
        worker._age = 14f;
        worker._happiness = 1f;
        worker._jobManager = _jobManager;
        worker._gameManager = _GameManager;
        _jobManager._unoccupiedWorkers.Add(worker);
    }

    public void Update()
    {
        tick += Time.deltaTime;
        var currentEfficiency = 15f / AverageWorkerHappiness();
        if (!(tick >= currentEfficiency)) return;
        tick %= currentEfficiency;
        var newCharacterGameObject =
            Instantiate(_characterPrefab, transform);
        var worker = newCharacterGameObject.GetComponent<Worker>();
        worker._jobManager = _jobManager;
        worker._gameManager = _GameManager;
        _workers.Add(worker);
    }
}