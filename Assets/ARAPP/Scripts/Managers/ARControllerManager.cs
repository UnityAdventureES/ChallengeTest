using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Niantic.ARDK.Templates;

public class ARControllerManager : MonoBehaviour
{

    public static ARControllerManager instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public GameObject ARController;
    private MeshColliderController meshColliderController;
    void Start()
    {
        meshColliderController = ARController.GetComponent<MeshColliderController>();
        GameManager.instance.OnMinimumMeshesFound += TurnOnInteractions;

        GameManager.instance.OnEggLaid += TurnOffInteraction;
    }

    public void TurnOffInteraction()
    {
        meshColliderController.enabled = false;
    }

    public void TurnOnInteractions()
    {
    
        meshColliderController.enabled = true;
    }

    public void PowerUp() 
    {

        ARController.transform.GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(false);
        ARController.transform.GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(true);
        ARController.transform.GetChild(0).GetChild(1).GetChild(2).gameObject.SetActive(true);
        ARController.transform.GetChild(0).GetChild(1).GetChild(3).gameObject.SetActive(true);

    }

}
