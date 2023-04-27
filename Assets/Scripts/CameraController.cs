using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;

    [SerializeField] PlayerLife playerLife;

    private void Update()
    {
        if (playerLife.isAlive)
        {
            transform.position = new Vector3(player.position.x, player.position.y + 1, transform.position.z);
        }
        
    }
}
