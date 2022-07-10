using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using UnityEngine;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;
using Niantic.ARDK.Extensions.Meshing;

public class ObjectProviderManager : MonoBehaviour
{
    public static ObjectProviderManager instance;
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

    [Header("Niantic Mesh Manager")]
    [SerializeField] private ARMeshManager meshManager;
    [SerializeField] private int minimumMeshBlocks = 30;
    [SerializeField] private int miniumTimeMesh = 5000;

    [Header("Object Generation")]
    public GameObject[] containers = new GameObject[2];
    [Header("Prefabs")]
    [SerializeField] private GameObject apple;
    [SerializeField] private GameObject grass;
    [SerializeField] private List<GameObject> powersUp = new List<GameObject>();
    [SerializeField] private List<GameObject> enemyBlocks = new List<GameObject>();

    [Header("Object Generation Parameters")]
    [SerializeField] private float groundTolerance = 0.05f;
    [SerializeField] private float centerOffset = 0.2f;
    [SerializeField] private float appleOffset = 0.015f;
    [SerializeField] private float minDisEnemies = 0.4f;

    [HideInInspector]
    public GameObject SnakeManagerGO;
    [HideInInspector]
    public List<GameObject> currentEnemies = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> FloorMeshes = new List<GameObject>();
    private Stopwatch meshTimer = new Stopwatch();
    [HideInInspector]
    public int grasQuantity = 0;
    [HideInInspector]
    public int grassPowerCount = 0;

    private float lowerLimit;
    private float upperLimit;

    private bool isTimerComplete;
    private bool isMinimulBlocks;

    private GameObject meshRoot;
    private List<GameObject> meshBloks = new List<GameObject>();
    private Dictionary<GameObject, Vector3> BlockCenterDic = new Dictionary<GameObject, Vector3>();


    private void Start()
    {
        meshRoot = meshManager.MeshRoot;

        meshManager.MeshObjectsUpdated += MeshObjectUpdate;

        GameManager.instance.OnSnakeSpawen += ActivateFloorFinder;
        GameManager.instance.OnSnakeSpawen += FindFloor;
        GameManager.instance.OnappleEaten += FindFloor;
        GameManager.instance.OnappleEaten += GrowApples;
        GameManager.instance.OnappleEaten += SpawnEnemy;
    }

    private void MeshObjectUpdate(MeshObjectsUpdatedArgs args)
    {

        if (meshRoot.transform.childCount >= minimumMeshBlocks && !isMinimulBlocks)
        {
            meshTimer.Start();
            isMinimulBlocks = true;
        }

        if (meshTimer.ElapsedMilliseconds >= miniumTimeMesh && !isTimerComplete)
        {
            isTimerComplete = true;
            GameManager.instance.MinimumMeshesFound();
            meshTimer.Stop();
        }

        foreach (Transform child in meshRoot.transform)
        {
            SaveBlocks(child.gameObject);
        }
    }

    private void SaveBlocks(GameObject child)
    {

        var mesh = child.GetComponent<MeshCollider>();
        if (!(bool)mesh)
            return;

        var meshCenter = mesh.sharedMesh.bounds.center;
        meshCenter = GameManager.instance.Convert2UnityCoordinateSystem(meshCenter);

        if (!meshBloks.Contains(child))
        {
            meshBloks.Add(child);
            BlockCenterDic.Add(child, meshCenter);
        }
        else
        {
            BlockCenterDic[child] = meshCenter;
        }
    }

    private void FindFloor()
    {
        lowerLimit = SnakeManagerGO.transform.position.y - centerOffset;
        upperLimit = SnakeManagerGO.transform.position.y + centerOffset;

        foreach (var centerPos in BlockCenterDic.Values)
        {
            var key = BlockCenterDic.FirstOrDefault(x => x.Value == centerPos).Key;

            if (lowerLimit <= centerPos.y && centerPos.y <= upperLimit)
            {

                if (!FloorMeshes.Contains(key))
                {
                    FloorMeshes.Add(key);
                }
            }

            else
            {
                if (FloorMeshes.Contains(key))
                {
                    FloorMeshes.Remove(key);
                }
            }
        }
    }
    private void ActivateFloorFinder()
    {
        StartCoroutine(CallFirstApples());
    }
    IEnumerator CallFirstApples()
    {
        yield return new WaitForSeconds(2);
        FirstApples(global::SnakeManager.instance.currentMesh);
    }
    
    public void GrowGrass() 
    {
        GameObject powerUpGrass = GenerateItem(0, grass);

        if (powerUpGrass != null)
        {
            powerUpGrass.name = grass.name;
            var startGrass = powerUpGrass.transform.GetChild(0).GetComponent<GrassController>();
            startGrass.GrowGrass();
            var explosion = powerUpGrass.transform.GetChild(1).gameObject;
            powerUpGrass.transform.parent = null;
            explosion.transform.localScale = Vector3.one;
            explosion.SetActive(true);
            powerUpGrass.transform.rotation = Quaternion.Euler(grass.transform.rotation.x, Random.Range(0, 360), grass.transform.rotation.z);
            powerUpGrass.transform.parent = containers[1].transform;
            StartCoroutine(SnakeManager.instance.DestroyExplosion(explosion));

            if (grassPowerCount < grasQuantity)
            {
                Invoke("GrowGrass", 0.5f);
                grassPowerCount++;
            }
            else
            {
                GamePlayManager.instance.IsPowerUpGrassActivated = false;
            }
 
            
        }
        else
        {
            Invoke("GrowGrass", 1);
        }
    }
    

    public void GrowApples()
    {
        if (!GamePlayManager.instance.ShouldSpawnApples)
            return;
        float offset = appleOffset + apple.transform.localScale.y / 2;

        var newApple = GenerateItem(offset, apple);

        if (newApple != null)
        {

            if (currentEnemies.Count == 0)
            {
                GamePlayManager.instance.ActiveApples.Add(newApple);
                RadarManager.instance.GenerateHelper(newApple);
                
            }
            else
            {
                foreach (var enemie in currentEnemies)
                {
                    var dis = Vector3.Distance(enemie.transform.position, newApple.transform.position);
                    if (dis < minDisEnemies)
                    {
                        var pos = enemie.transform.position;
                        apple.transform.position = new Vector3(pos.x * Random.Range(0.3f, 1), pos.y, pos.z * Random.Range(0.3f, 1));
                    }
                }

                GamePlayManager.instance.ActiveApples.Add(newApple);
                RadarManager.instance.GenerateHelper(newApple);
            }

            Debug.Log("New Apple");
        }
        else
        {
            Invoke("GrowApples", 1.0f);
        }
    }

    public void SpawnPowerUp() 
    {
        GameObject powerUpPrefab = powersUp[Random.Range(0, powersUp.Count)];
        GameObject powerUpGO = GenerateItem(0, powerUpPrefab);

        if (powerUpGO != null)
        {
            powerUpGO.name = powerUpPrefab.name;

            if (currentEnemies.Count == 0)
            {
                RadarManager.instance.GenerateHelper(powerUpGO);
            }
            else
            {
                foreach (var enemie in currentEnemies)
                {
                    var dis = Vector3.Distance(enemie.transform.position, powerUpGO.transform.position);
                    if (dis < minDisEnemies)
                    {
                        var pos = enemie.transform.position;
                        apple.transform.position = new Vector3(pos.x * Random.Range(0.3f, 1), pos.y, pos.z * Random.Range(0.3f, 1));
                    }
                }

                Debug.Log("New Power Up");
                RadarManager.instance.GenerateHelper(powerUpGO);
            }
        }
        else
        {
            Invoke("SpawnPowerUp", 1);
        }

    }
    public void SpawnEnemy() 
    {
        GameObject enemyPrefab = enemyBlocks[Random.Range(0, enemyBlocks.Count)];
        GameObject enemyBlock = GenerateItem(0, enemyPrefab);

        if (enemyBlock !=  null)
        {

            if (currentEnemies.Count == 0)
            {
                enemyBlock.name = enemyPrefab.name + currentEnemies.Count;
                currentEnemies.Add(enemyBlock);
                enemyBlock.transform.parent = containers[0].gameObject.transform;
            }
            else
            {
                foreach (var enemie in currentEnemies)
                {
                    var dis = Vector3.Distance(enemie.transform.position, enemyBlock.transform.position);
                    if (dis<minDisEnemies)
                    {
                        var pos = enemie.transform.position;
                        enemyBlock.transform.position = new Vector3(pos.x * Random.Range(0.3f, 1), pos.y, pos.z * Random.Range(0.3f, 1));
                    }
                }

                enemyBlock.transform.parent = containers[0].gameObject.transform;
                enemyBlock.name = enemyPrefab.name + currentEnemies.Count;
                currentEnemies.Add(enemyBlock);
                Debug.Log("New Block Enemy");
            }

        }
        else
        {
            Invoke("SpawnEnemy", 1.0f);
        }
    }


    public void FirstApples(Mesh mesh)
    {
        int vertex = Random.Range(0, mesh.vertexCount);

        Vector3 normal = mesh.normals[vertex];
        bool floorPart = Mathf.Abs(normal.y) >= 1.0f - groundTolerance;
        if (floorPart)
        {
            Vector3 position = mesh.vertices[vertex];
            position = GameManager.instance.Convert2UnityCoordinateSystem(position);
            position = new Vector3(position.x, position.y + appleOffset + apple.transform.localScale.y / 2, position.z);
            var newApple = Instantiate(apple, position, Quaternion.identity);

            RadarManager.instance.GenerateHelper(newApple);
            GamePlayManager.instance.ActiveApples.Add(newApple);
            Debug.Log("New apple");
        }
        else
        {
            FirstApples(mesh);
            Debug.Log("No Valit Vertex, Recalculating...");
        }
    }
    private GameObject GenerateItem(float offsetFloor, GameObject spawnObject)
    {
        var randomPosition = Random.Range(0, FloorMeshes.Count);
        GameObject floorBlock = FloorMeshes[randomPosition];

        if (floorBlock != null)
        {
            var mesh = floorBlock.GetComponent<MeshFilter>().sharedMesh;
            int vertex = Random.Range(0, mesh.vertexCount);

            Vector3 normal = mesh.normals[vertex];
            bool floorPart = Mathf.Abs(normal.y) >= 1.0f - groundTolerance;

            if (floorPart)
            {
                Vector3 position = mesh.vertices[vertex];
                position = GameManager.instance.Convert2UnityCoordinateSystem(position);
                position = new Vector3(position.x, position.y + offsetFloor, position.z);
                var newApple = Instantiate(spawnObject, position, Quaternion.identity);

                return newApple;
            }
            else
            {
                Debug.Log("No Valit Vertex, Recalculating...");
                return null;

            }
        }
        else
        {
            return null;
        }
    }
}
