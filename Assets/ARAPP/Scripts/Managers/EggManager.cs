using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class EggManager : MonoBehaviour
{
    public GameObject Snake;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void CallSnakeGeneration() 
    {
        StartCoroutine(SnakeGeneration());
    }

    IEnumerator SnakeGeneration() 
    {

        yield return new WaitForSeconds(0.8f);
        this.transform.DOShakeRotation(0.8f, 60, 10, 40, true);
        yield return new WaitForSeconds(1.2f);
        Snake.transform.position = this.transform.position;
        if (this.GetComponent<MeshCollider>())
        {
            this.GetComponent<MeshCollider>().convex = true;
            this.GetComponent<MeshCollider>().isTrigger = true;
        }
        else if(this.GetComponent<SphereCollider>())
        {
            this.GetComponent<SphereCollider>().enabled = false;
            this.GetComponent<SphereCollider>().isTrigger= false;
        }

       
        Snake.SetActive(true);
        GameManager.instance.SnakeBorn();
        this.transform.DOScale(Vector3.zero, 1.0f);

        
    }
}
