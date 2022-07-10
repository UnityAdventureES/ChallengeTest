using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeControllerManager : MonoBehaviour
{
    public static SnakeControllerManager instance;
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
    [Header("Control Parameters")]
    [SerializeField] private float speed = 2;
    [SerializeField] private float turnSpeed = 1;
    [SerializeField] float disteanceBetween = 0.05f;
    [Header("Prefabs")]
    [SerializeField] private List<GameObject> bodyParts = new List<GameObject>();

    [HideInInspector]
    public List<GameObject> snakeBody = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> saveOldParts = new List<GameObject>();

    private bool turnLeft;
    private bool turnRight;
    private bool shouldStop;

    float contUp = 0;
    void Start()
    {
        CreateBodyParts();
        SnakeManager.instance.OnDownSnake += ActivateReset;

        foreach (var item in bodyParts)
        {
            saveOldParts.Add(item);
        }
    }


    private void FixedUpdate()
    {
        ManageSnakeBody();
        if (SnakeManager.instance.OnEarth)
        {
            SnakeMovement();
        }

    }

    private void ManageSnakeBody() 
    {
        if (bodyParts.Count > 0)
        {
            CreateBodyParts();
        }

        for (int i = 0; i < snakeBody.Count; i++)
        {
            if (snakeBody[i] == null)
            {
                snakeBody.RemoveAt(i);
                i = i - 1;
            }
        }

        if (snakeBody.Count == 0)
        {
            Destroy(this);
        }
    }

   private void SnakeMovement() 
   {
        snakeBody[0].transform.position += snakeBody[0].transform.forward * speed * Time.deltaTime;
        if (turnRight)
        {
            snakeBody[0].transform.rotation = Quaternion.Euler(0, snakeBody[0].transform.rotation.eulerAngles.y, 0);
            snakeBody[0].transform.Rotate(new Vector3(0, 1, 0) * turnSpeed * Time.deltaTime);
        }
        else if (turnLeft)
        {
            snakeBody[0].transform.rotation = Quaternion.Euler(0, snakeBody[0].transform.rotation.eulerAngles.y, 0);
            snakeBody[0].transform.Rotate(new Vector3(0, -1, 0) * turnSpeed * Time.deltaTime);
        }

        if (snakeBody.Count > 1)
        {
 
            for (int i = 1; i < snakeBody.Count; i++)
            {
                SnakeBodyPartManager snakeManager = snakeBody[i -1].GetComponent<SnakeBodyPartManager>();
                snakeBody[i].transform.position = snakeManager.snakes[0].position;
                snakeBody[i].transform.rotation = snakeManager.snakes[0].rotation;
                snakeManager.snakes.RemoveAt(0);

                if (shouldStop)
                {
                    Debug.Log("Stop Snake to Reset");
                    ResetSnake();
                    i = snakeBody.Count;
                }
            }

 
        }
   }

    private void CreateBodyParts() 
    {
        if (snakeBody.Count ==0)
        {
            GameObject temp1 = Instantiate(bodyParts[0], transform.position, transform.rotation, transform);
            if (!temp1.GetComponent<SnakeBodyPartManager>())
            {
                temp1.AddComponent<SnakeBodyPartManager>();
            }
            if (!temp1.GetComponent<Rigidbody>())
            {
               var headCol = temp1.AddComponent<Rigidbody>();
            }
            snakeBody.Add(temp1);
            ObjectProviderManager.instance.SnakeManagerGO = temp1;
            GameManager.instance.SnakeSpawen();
            bodyParts.RemoveAt(0);
            
        }

        SnakeBodyPartManager bodySnake = snakeBody[snakeBody.Count - 1].GetComponent<SnakeBodyPartManager>();
        
        if (contUp == 0)
        {
            bodySnake.ClearSnake();
        }

        contUp += Time.deltaTime;
        if (contUp >= disteanceBetween)
        {
            GameObject temp = Instantiate(bodyParts[0], transform.position, transform.rotation, transform);
            if (!temp.GetComponent<SnakeBodyPartManager>())
            {
                temp.AddComponent<SnakeBodyPartManager>();
            }
            if (!temp.GetComponent<Rigidbody>())
            {
                temp.AddComponent<Rigidbody>();
            }
            snakeBody.Add(temp);
            if (snakeBody.Count > SnakeManager.instance.visibleBody)
            {
                temp.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                temp.gameObject.GetComponent<Rigidbody>().isKinematic = true;

            }

            bodyParts.RemoveAt(0);
            temp.GetComponent<SnakeBodyPartManager>().ClearSnake();
            contUp = 0;
        }
    }


    private void ActivateReset() 
    {
        shouldStop = true;
        Debug.Log("Activate position reset for snake");
    }
    private void ResetSnake() 
    {


        int count = this.transform.childCount;
        for (int i = 1; i < count; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        snakeBody = new List<GameObject>();

        for (int i = 1; i < saveOldParts.Count - 1; i++)
        {
            bodyParts.Add(saveOldParts[i]);
        }

        snakeBody.Add(this.transform.GetChild(0).gameObject);

        CreateBodyParts();
        shouldStop = false;
    }

    public void TurnRight(bool shouldTurn)
    {
        turnRight = shouldTurn;
    }
    public void TurnLeft(bool shouldTurn)
    {
        turnLeft = shouldTurn;
    }

}
