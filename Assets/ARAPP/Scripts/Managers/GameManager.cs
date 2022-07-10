using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{

    public event Action OnMinimumMeshesFound;
    public event Action OnEggLaid;
    public event Action OnSnakeBorn;
    public event Action OnSnakeSpawen;
    public event Action OnappleEaten;
    public event Action OnGameOver;

    public GameObject SnakeManager;

    public static GameManager instance;


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

    public void MinimumMeshesFound() 
    {
        Debug.Log("Minimum Meshes Found");
        OnMinimumMeshesFound?.Invoke();
    }

    public void EggLaid() 
    {
        Debug.Log("Egg Laif");
        OnEggLaid?.Invoke();
    }

    public void SnakeBorn() 
    {
        Debug.Log("Snake Born");
        OnSnakeBorn?.Invoke();
    }

    public void AppleEaten() 
    {
        Debug.Log("Apple Eate");
        OnappleEaten?.Invoke();
    }

    public void SnakeSpawen() 
    {
        Debug.Log("On Sanke Spawen");
        OnSnakeSpawen?.Invoke();
    }

    public void GameOver() 
    {
        Debug.Log("Game Over");
        OnGameOver?.Invoke();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    public Vector3 Convert2UnityCoordinateSystem(Vector3 v) 
    {
        return new Vector3(v.x, -v.y, v.z);
    }
}
