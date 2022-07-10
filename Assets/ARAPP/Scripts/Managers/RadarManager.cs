using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RadarManager : MonoBehaviour
{
    public static RadarManager instance;
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

    [SerializeField] private GameObject RadarCamera;
    [SerializeField] private GameObject SnakeHeadTracker;
    [SerializeField] private Material[] materials = new Material[2];
    [SerializeField] private float minDistance = 1.0f;

    [HideInInspector]
    [SerializeField] private GameObject SnakeHead;
    private float distance;
    [HideInInspector]
    public bool needDetroyHelper;

    private bool shouldTrackSnake;
    private GameObject helper;

    [Serializable]
    public class Helper
    {
        public GameObject helper;
        public GameObject target;
        public bool isVisible;
        public bool isApple;
    }

    private List<Helper> helpers = new List<Helper>();

    void Start()
    {
        GameManager.instance.OnSnakeSpawen += StartRadar;
    }

    // Update is called once per frame
    void Update()
    {
        if (!shouldTrackSnake)
            return;
        FollowCamera();
        HelperFollow();
    }

    private void StartRadar() 
    {
       
        SnakeHead = SnakeControllerManager.instance.snakeBody[0];
        FollowCamera();
        RadarCamera.gameObject.SetActive(true);
        shouldTrackSnake = true;
    }

    private void FollowCamera() 
    {
        RadarCamera.transform.localPosition = new Vector3(SnakeHead.transform.position.x, RadarCamera.transform.position.y, SnakeHead.transform.position.z);
        RadarCamera.transform.localRotation = Quaternion.Euler(RadarCamera.transform.rotation.eulerAngles.x, SnakeHead.transform.rotation.eulerAngles.y,
            RadarCamera.transform.rotation.eulerAngles.z);
        SnakeHeadTracker.transform.position = SnakeHead.transform.position;
        helper = SnakeHeadTracker.transform.GetChild(0).gameObject;

    }


    private void HelperFollow() 
    {
        if (helpers.Count == 0)
        {
            return;
        }

        foreach (var helper in helpers)
        {
            if (helper.isVisible)
            {
                if (needDetroyHelper)
                    return;
                distance = Vector3.Distance(SnakeHeadTracker.transform.position, helper.target.transform.position);

                if (distance > minDistance)
                {
                    var pos = Calculate(SnakeHeadTracker.transform.position, helper.target.transform.position, minDistance / distance);
                    helper.helper.transform.position = pos;
                    helper.helper.layer = LayerMask.NameToLayer("Radar");
                    helper.target.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Invisible");
                }
                else
                {
                    helper.target.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Radar");
                    helper.helper.layer = LayerMask.NameToLayer("Invisible");
                }
            }

        }
        
    }


    public void GenerateHelper(GameObject target) 
    {
        if (helpers.Count == 0)
        {
            Debug.Log("New Helper For Radar");
            var temp = new Helper();
            temp.target = target;

            temp.isApple = target.CompareTag("Apple");
            var mat = temp.isApple ? materials[0] : materials[1];
            
            temp.helper = Instantiate(helper, SnakeHeadTracker.transform);
            temp.helper.GetComponent<MeshRenderer>().sharedMaterial = mat;
            temp.isVisible = true;
            helpers.Add(temp);
        }
        else
        {
            if (CheckNull())
            {
                for (int i = 0; i < helpers.Count; i++)
                {
                    if (helpers[i].target == null)
                    {
                        helpers[i].target = target;
                        helpers[i].isVisible = true;
                        helpers[i].isApple = target.CompareTag("Apple");
                        var mat = helpers[i].isApple ? materials[0] : materials[1];
                        helpers[i].helper.GetComponent<MeshRenderer>().sharedMaterial = mat;

                    }
                }
            }
            else
            {
                Debug.Log("NewHelper");
                var temp = new Helper();
                temp.target = target;
                temp.isApple = target.CompareTag("Apple") ? true : false;
                var mat = temp.isApple ? materials[0] : materials[1];

                temp.helper = Instantiate(helper, SnakeHeadTracker.transform);
                temp.helper.GetComponent<MeshRenderer>().sharedMaterial = mat;
                temp.isVisible = true;
                helpers.Add(temp);
            }
        }
  
    }

    private bool CheckNull() 
    {
        int count = helpers.Count;
        for (int i = 0; i < count; i++)
        {
            if (helpers[i].target == null)
            {
                return true;
            }
        }

        return false;
    }
    public void DeleteHelper(GameObject target)
    {
        int count = helpers.Count;
        for (int i = 0; i < count; i++)
        {
            if (helpers[i].target == target)
            {
                helpers[i].target = null;
                helpers[i].isVisible = false;
            }
        }

        Destroy(target.gameObject);
        needDetroyHelper = false;
    }

    private Vector3 Calculate(Vector3 pOne, Vector3 pTwo, float t)
    {
       
        var mx = pTwo.x - pOne.x;
        var my = pTwo.y - pOne.y;
        var mz = pTwo.z - pOne.z;

        var x = pOne.x + mx * t;
        var y = pOne.y + my * t;
        var z = pOne.z + mz * t;

        return new Vector3(x, y, z);
    }  
}
