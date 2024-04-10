using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class JobTemplate : MonoBehaviour
{
    public JobItem jobItem;
    public TMP_Text name;
    public TMP_Text description;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClickJob()
    {
        name.text= jobItem.name;
        description.text= jobItem.description;

        UIController.Instance.selectedJobID = jobItem.id;
        foreach (Transform obj in this.transform.parent)
        {
            if (obj.gameObject.activeInHierarchy)
            {
                obj.transform.localScale= Vector3.one;
            }
        }
        transform.localScale = new Vector3(1.2f,1.2f,1.2f);
    }
}
