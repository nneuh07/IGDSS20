using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JobManager : MonoBehaviour
{
    private List<Job> _availableJobs = new List<Job>();
    public List<Worker> _unoccupiedWorkers = new List<Worker>();
    public List<Job> _assignedJobs = new List<Job>();
    private float _numberWorkers = 0f;
    float tick = 0f;

    #region MonoBehaviour

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        tick += Time.deltaTime;
        if (!(tick >= 1f)) return;
        HandleUnoccupiedWorkers();
    }

    #endregion


    #region Methods

    private void HandleUnoccupiedWorkers()
    {
        if (_unoccupiedWorkers.Count <= 0) return;
        foreach (var worker in _unoccupiedWorkers)
        {
            if (!_availableJobs.Any()) return;
            var availableJob = _availableJobs.FirstOrDefault();
            availableJob?.AssignWorker(worker);
            _availableJobs.Remove(availableJob);
            _assignedJobs.Add(availableJob);
        }
    }

    public void AddJobs(List<Job> jobs)
    {
        _availableJobs.AddRange(jobs);
    }

    public void RegisterWorker(Worker w)
    {
        _numberWorkers += 1;
        _unoccupiedWorkers.Add(w);
    }


    public void RemoveWorker(Worker w)
    {
        _numberWorkers -= 1;
        _unoccupiedWorkers.Remove(w);
        _availableJobs.Add(_assignedJobs.FirstOrDefault(j => j._worker == w));
    }

    public float getNumberOfWorkers()
    {
        return _numberWorkers;
    }

    #endregion
}