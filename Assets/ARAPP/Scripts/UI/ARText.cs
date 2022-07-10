using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ARText : MonoBehaviour
{
    private GameObject SnakeHead;
    private bool shouldTrackSnake;
    [SerializeField] private GameObject ARCamera;
    [SerializeField] private float distanceSnake = 0.15f;
    [SerializeField] public List<string> Words = new List<string>();

    public static ARText instance;
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

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.OnSnakeSpawen += FollowSanke;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!shouldTrackSnake)
            return;
        FollowSanke();

    }

    private void FollowSanke()
    {
        SnakeHead = SnakeControllerManager.instance.snakeBody[0];
        transform.localPosition = new Vector3(SnakeHead.transform.position.x, SnakeHead.transform.position.y + distanceSnake, SnakeHead.transform.position.z);
        transform.LookAt(ARCamera.transform);
        shouldTrackSnake = true;

    }

    public void ShowSentence(bool isPowerUp)
    {
        //transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(true);

        if (isPowerUp)
        {
            transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Words[Random.Range(0, Words.Count - 1)];
        }
        else
        {
            transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Words[Words.Count-1];
        }

        StartCoroutine(TurnOffSentence());
    }

    private IEnumerator TurnOffSentence() 
    {
        yield return new WaitForSeconds(1.5f);
        //transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(false);
    }
}
