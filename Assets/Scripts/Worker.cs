using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Worker : MonoBehaviour
{
    #region Manager References

    public JobManager _jobManager; //Reference to the JobManager
    public GameManager _gameManager; //Reference to the GameManager

    #endregion

    private float tick = 0f;
    public float _age = 0f; // The age of this worker
    public float _happiness = 1f; // The happiness of this worker

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Age();
    }


    private void Age()
    {
        tick += Time.deltaTime;
        if (!(tick >= 15f)) return;
        tick %= 15f;
        _age++;
        Consume();
        HappinessJob();


        if (_age > 14)
        {
            BecomeOfAge();
        }

        if (_age > 64)
        {
            Retire();
        }

        if (_age > 100)
        {
            Die();
        }
    }

    private void Consume()
    {
        if (_gameManager.WorkerConsumed())
        {
            _happiness += 0.1f;
            if (_happiness > 1f)
            {
                _happiness = 1f;
            }
        }
        else
        {
            _happiness -= 0.1f;
            if (_happiness < 0f)
            {
                _happiness = 0f;
            }
        }
    }

    private void HappinessJob()
    {
        if (_jobManager._assignedJobs.Any(job => job._worker == this))
        {
            _happiness += 0.2f;
        }
    }


    public void BecomeOfAge()
    {
        _jobManager.RegisterWorker(this);
    }

    private void Retire()
    {
        _jobManager.RemoveWorker(this);
    }

    private void Die()
    {
        Destroy(this.gameObject, 1f);
    }
}