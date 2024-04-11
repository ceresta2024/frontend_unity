using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobLocal : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        GetJobs();
    }
    public void GetJobs()
    {
        LoginManager.Instance.GetJobs();
    }
    public void SetJob()
    {
        LoginManager.Instance.SetJob(UIController.Instance.selectedJobID);
    }

}
