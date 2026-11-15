using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBackground : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    private Vector3 startPosition;
    private float repeatWidth;
    private PlayerController playerControllerScript;

    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        startPosition = transform.position;
        repeatWidth = GetComponent<BoxCollider>().bounds.size.z/2;
    }

    void Update()
    {
        if (playerControllerScript.isGameOver == false)
        {
            transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);

            if (transform.position.z < startPosition.z-repeatWidth)
            {
                transform.position = startPosition;
            }
        }
    }

}