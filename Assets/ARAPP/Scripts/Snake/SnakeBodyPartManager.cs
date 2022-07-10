using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class SnakeBodyPartManager : MonoBehaviour
{
    [Serializable]
    public class SnakePart 
    {
        public Vector3 position;
        public Quaternion rotation;

        public SnakePart(Vector3 pos, Quaternion rot) 
        {
            position = pos;
            rotation = rot;
        }
    }

    public List<SnakePart> snakes = new List<SnakePart>();

    private void FixedUpdate()
    {
        UpdateSnake();
    }

    public void UpdateSnake() 
    {
        snakes.Add(new SnakePart(transform.position, transform.rotation));
    }

    public void ClearSnake() 
    {
        snakes.Clear();
        snakes.Add(new SnakePart(transform.position, transform.rotation));
    }
}
