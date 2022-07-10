using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class GamePlayManager : MonoBehaviour
{
    public event Action OnPowerUp;
    public static GamePlayManager instance;
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
    [Header("Game Play States")]
    public int AppleCount = 0;
    public int PowerUpAppleCount;
    public int PowerUpDestroyCount;
    public int PowerUpGrassCount;
    public int Points;
    public int LifeCount = 2;

    [HideInInspector]
    public bool IsPowerUpAppleActivated;
    [HideInInspector]
    public bool IsPowerUpDestroyActivated;
    [HideInInspector]
    public bool IsPowerUpGrassActivated;
    [HideInInspector]
    public bool ShouldSpawnApples;
    public int deadTime = 4500;

    public const int APPLE_VALUE = 20;
    public const int POWER_UP_APPLE = 10;
    public const int POWER_UP_DESTROY = 10;
    public const int POWER_UP_GRASS = 15;

    public int[] PowerUpIn = { 1, 4, 10, 15, 20, 22, 30 }; 
    [HideInInspector]
    public List<GameObject> ActiveApples = new List<GameObject>();
    public void PowerUP() 
    {
        OnPowerUp?.Invoke();
    }

    public void PointCounts() 
    {
        Points = AppleCount * APPLE_VALUE + PowerUpAppleCount * POWER_UP_APPLE + PowerUpDestroyCount*POWER_UP_DESTROY + PowerUpGrassCount*POWER_UP_GRASS;
    }

}
