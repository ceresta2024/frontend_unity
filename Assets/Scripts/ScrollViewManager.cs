using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewManager : MonoBehaviour
{
    public GameObject buttonTemplate;
    private Transform templateParent;

    private void OnEnable()
    {
        LoadJobList();
    }
    // Method to handle item click event
    public void OnItemClicked(int index)
    {
        // Handle the selected item's index
        Debug.Log("Selected item index: " + index);

        if (UIController.Instance.shopPanel.activeInHierarchy)
        {
            Debug.Log("Shop func:");

        }
        else if(UIController.Instance.inventoryPanel.activeInHierarchy)
        {
            Debug.Log("Inventory func:");
        }

    }

    public void LoadJobList()
    {
        templateParent = buttonTemplate.transform.parent;
        foreach (Transform obj in buttonTemplate.transform.parent)
        {
            if (obj.gameObject.activeInHierarchy)
            {
                Destroy(obj.gameObject);
            }
            Debug.Log(":Des");
            //  Destroy(obj.gameObject);
        }
        if (buttonTemplate != null)
        {
            templateParent = buttonTemplate.transform.parent;

            Debug.Log("::");

            foreach (var item in LoginManager.Instance.allJobItems)
            {
               GameObject obj= Instantiate(buttonTemplate, buttonTemplate.transform.parent);
                obj.GetComponent<JobTemplate>().jobItem= item;
                obj.SetActive(true);

            }

        }
    }



}
