using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GrassController : MonoBehaviour
{
    [SerializeField] private GameObject principal;
    [SerializeField] private List<GameObject> childs = new List<GameObject>();

    private void Start()
    {
/*
        principal = transform.GetChild(0).gameObject;

        int count = transform.childCount;
        for (int i = 1; i < count; i++)
        {
            childs.Add(transform.GetChild(i).gameObject);
        }*/
    }
    public void GrowGrass() 
    {
        StartCoroutine(GenerateGrass());
    }

    IEnumerator GenerateGrass() 
    {
        principal.transform.DOScale(Vector3.one, 1.2f);
        yield return new WaitForSeconds(0.3f);
        foreach (var child in childs)
        {
            child.transform.DOScale(Vector3.one, 0.8f);
        }
    }

}
