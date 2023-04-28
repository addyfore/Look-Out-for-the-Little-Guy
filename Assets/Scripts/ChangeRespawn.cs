using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRespawn : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform respawn;
    [SerializeField] private Transform checkpoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Respawn"))
        {
            respawn.position = new Vector3(player.position.x, checkpoint.position.y, respawn.position.z);
        }
    }
}
