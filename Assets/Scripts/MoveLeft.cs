using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MoveLeft : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    private PlayerController playerControllerScript;
    [SerializeField] private float leftBoundary = 5f;


    void Start()
    {
        // Βρίσκουμε το αντικείμενο με το όνομα "Player" και παίρνουμε το script του
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        if (playerControllerScript.isGameOver == false)
        {
            transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);    
        }

        if (gameObject.CompareTag("Obstacle") && transform.position.z < leftBoundary)
        {
            Destroy(gameObject);
        }
    }
}
