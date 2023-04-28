using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRespawn : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject[] checkpoints;

    [HideInInspector] public float respawnX;
    [HideInInspector] public float respawnY;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Respawn"))
        {
            respawnX = checkpoints[int.Parse(collision.gameObject.name)].transform.position.x;
            respawnY = checkpoints[int.Parse(collision.gameObject.name)].transform.position.y;

        }
    }
}
