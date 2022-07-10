using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class SnakeManager : MonoBehaviour
{
    public event Action OnDownSnake;

    public static SnakeManager instance;
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

    [SerializeField] private Camera aRCamera;
    [SerializeField] private float scanZoneWidth = 600;
    [SerializeField] private FindAvailableMesh.ScanLevel scanLevel = FindAvailableMesh.ScanLevel.Low;
    [SerializeField] private GameObject Helmet;
    public List<Vector2> pointTest = new List<Vector2>();

    private FindAvailableMesh findAvailableMesh;
    public int visibleBody = 1;
    public bool isDestroyActivated;

    private readonly string appleTag = "Apple";
    private readonly string powerUpTag = "PowerUp";
    private readonly string enemyTag = "Enemy";
    private readonly string powerUpApple= "GemPUApple";
    private readonly string powerUpDestroy = "GemPUDestroy";
    private readonly string powerUpGrass = "GemPUGrass";
    private readonly string aRCameraName = "ARSceneCamera";


    private bool shouldUseHelment;
    
    public Mesh currentMesh;
    public GameObject currenMeshContainer;

    public bool OnEarth = true;
    public Vector3 hitPoint;
    public bool isSnakeSpawned;

    void Start()
    {
        aRCamera = GameObject.Find(aRCameraName).GetComponent<Camera>();
        findAvailableMesh = new FindAvailableMesh(scanZoneWidth);
        findAvailableMesh.ScanLevelMesh = scanLevel;
        pointTest = findAvailableMesh.scanPoints;
        GameManager.instance.OnGameOver += StopSnake;   
    }


    void Update()
    {
        if (Mathf.Abs(this.transform.position.y) > 5 && OnEarth)
        {
            Debug.Log("The Snake Fell Down");
            StartCoroutine(ReSpawnSnake());
            OnEarth = false;
            OnDownSnake?.Invoke();
        }

        if (shouldUseHelment && !Helmet.activeSelf)
        {
            Helmet.SetActive(true);
        }
        else if(!shouldUseHelment && Helmet.activeSelf)
        {
            Helmet.SetActive(false);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(appleTag))
        {
            if (visibleBody < SnakeControllerManager.instance.snakeBody.Count)
            {
                GamePlayManager.instance.AppleCount++;
                GamePlayManager.instance.PointCounts();


                if (GamePlayManager.instance.AppleCount%3==0)
                {
                    GamePlayManager.instance.LifeCount++;
                }

                RadarManager.instance.needDetroyHelper = true;
                RadarManager.instance.DeleteHelper(other.gameObject);

                visibleBody++;

                GameObject temp = SnakeControllerManager.instance.snakeBody[visibleBody -1].gameObject;
                temp.transform.GetChild(0).gameObject.SetActive(true);
                temp.gameObject.GetComponent<Rigidbody>().isKinematic = false;

                var indexApple = GamePlayManager.instance.ActiveApples.IndexOf(other.gameObject);
                GamePlayManager.instance.ActiveApples.RemoveAt(indexApple);



                //Grass
                var grass = other.transform.GetChild(1).GetComponent<GrassController>();
                other.transform.GetChild(1).transform.parent = null;
                grass.transform.parent = ObjectProviderManager.instance.containers[1].transform;

                //Explosion
                var explosion = other.transform.GetChild(2).gameObject;
                other.transform.GetChild(2).transform.parent = null;
                explosion.transform.localScale = Vector3.one;
                explosion.SetActive(true);
                StartCoroutine(DestroyExplosion(explosion));
                


                //Grass
                grass.gameObject.transform.localScale = Vector3.one*0.5f;
                grass.gameObject.transform.rotation = Quaternion.Euler(grass.transform.rotation.x, Random.Range(0, 360), grass.transform.rotation.z);
                grass.enabled = true;
                grass.GrowGrass();


                if (GamePlayManager.instance.ActiveApples.Count == 0)
                {
                    GamePlayManager.instance.ShouldSpawnApples = true;
                }
                else
                {
                    GamePlayManager.instance.ShouldSpawnApples = false;
                }


                foreach (var item in GamePlayManager.instance.PowerUpIn)
                {

                    if (GamePlayManager.instance.AppleCount == item)
                    {
                        Debug.Log("Spanw Power Up");

                        ObjectProviderManager.instance.SpawnPowerUp();
                    }
                }



                GameManager.instance.AppleEaten();

                //Destroy(other.gameObject);
            }
        }

        else if(other.gameObject.CompareTag(powerUpTag))
        {
            Debug.Log("Go Power Up: " + other.name);
            RadarManager.instance.needDetroyHelper = true;
            RadarManager.instance.DeleteHelper(other.gameObject);

            //Destroy(other.gameObject);

            GamePlayManager.instance.PowerUpAppleCount++;
            GamePlayManager.instance.PointCounts();



            GamePlayManager.instance.IsPowerUpAppleActivated = other.name == powerUpApple;

            if (!isDestroyActivated)
            {
                GamePlayManager.instance.IsPowerUpDestroyActivated = other.name == powerUpDestroy;
                isDestroyActivated = GamePlayManager.instance.IsPowerUpDestroyActivated;
            }
           
           
            GamePlayManager.instance.IsPowerUpGrassActivated= other.name == powerUpGrass;




            if (GamePlayManager.instance.IsPowerUpAppleActivated)
            {
                ARControllerManager.instance.TurnOnInteractions();
                ARControllerManager.instance.PowerUp();
            }

            if (GamePlayManager.instance.IsPowerUpGrassActivated)
            {
                var grassQuantity= ObjectProviderManager.instance.grasQuantity = Random.Range(4, 10);
                ObjectProviderManager.instance.grassPowerCount = 0;
                ObjectProviderManager.instance.GrowGrass();
                UIManager.instance.ShowPowerUpGrass(grassQuantity);
                GamePlayManager.instance.PowerUpGrassCount++;

            }

            if (GamePlayManager.instance.IsPowerUpDestroyActivated)
            {
                shouldUseHelment = true;
                UIManager.instance.ShowPowerUpHelmet();
            }



            UIManager.instance.ShowPowerUpApple();
            ARText.instance.ShowSentence(true);

        }
    }
    public IEnumerator DestroyExplosion(GameObject Explosion) 
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(Explosion);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(enemyTag))
        {
            if (GamePlayManager.instance.IsPowerUpDestroyActivated)
            {
                int index = 0;
                foreach (var enemy in ObjectProviderManager.instance.currentEnemies)
                {
                    if (enemy.name == collision.gameObject.transform.parent.name)
                    {
                        index = ObjectProviderManager.instance.currentEnemies.IndexOf(enemy);
                    }
                }

                ObjectProviderManager.instance.currentEnemies.RemoveAt(index);
                Destroy(collision.gameObject.transform.parent.gameObject);
                GamePlayManager.instance.PowerUpDestroyCount++;
                GamePlayManager.instance.PointCounts();
                GamePlayManager.instance.IsPowerUpDestroyActivated = false;


                Helmet.SetActive(false);
                shouldUseHelment = false;
                isDestroyActivated = false;

                ARText.instance.ShowSentence(true);
            }
            else
            {
                var dead = collision.gameObject.transform.parent.GetComponent<DeathHandling>();
                dead.StartDeadTimer();
                ARText.instance.ShowSentence(false);

            }

        }
    }

    private void OnCollisionStay(Collision collision)
    {
        var meshFilter = collision.gameObject.GetComponent<MeshFilter>();
        if (meshFilter)
        {
            currentMesh = meshFilter.sharedMesh;
            currenMeshContainer = collision.gameObject;
        }
    }

    private IEnumerator ReSpawnSnake()
    {
        foreach (var point in findAvailableMesh.scanPoints)
        {
            var scanPos = new Vector2(Screen.width / 2 + point.x, Screen.height / 2 + point.y);
            GameObject meshDetected  = FindMesh(scanPos);
            yield return new WaitForSeconds(0.05f);
            if (meshDetected!= null)
            {
                var meshFound = ObjectProviderManager.instance.FloorMeshes.Contains(meshDetected);
                if (meshFound)
                {
                    StartCoroutine(RePosition(hitPoint));
                    Debug.Log("New Position");
                    yield break;
                }
            }

            else
            {
                Debug.Log("Invalid plane, recalculating");
            }
        }
        yield return null;
        StartCoroutine(ReSpawnSnake());
        Debug.Log("No Found");
    }
    GameObject FindMesh(Vector2 touchPosition)
    {
        Ray ray = aRCamera.ScreenPointToRay(touchPosition);
        if (Physics.Raycast(ray, out RaycastHit hit3Mesh, 2.5f))
        {
            hitPoint = hit3Mesh.point;
            return hit3Mesh.transform.gameObject;
        }
        else
        {
            return null;
        }
    }

    private IEnumerator RePosition(Vector3 point) 
    {

        var rb = transform.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        this.transform.position = new Vector3(point.x, point.y + 0.15f, point.z);
        Debug.Log("New Snake Position: " + this.transform.localPosition);
        yield return new WaitForSeconds(0.5f);
        transform.GetComponent<Rigidbody>().isKinematic = false;
        yield return new WaitForSeconds(0.5f);
        OnEarth = true;
    }

    private void StopSnake() 
    {
        var rb = transform.GetComponent<Rigidbody>();

        OnEarth = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        rb.mass = 0;



        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
    }


}
