using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour
{

    private Animator anim;
    private Rigidbody2D rb;

    [SerializeField] private AudioSource deathSoundEffect;
    [SerializeField] private Transform player;
    [SerializeField] private ChangeRespawn changeRespawn;

    [SerializeField] public int numLives = 3;
    public bool isAlive = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            Die();
        }
    }

    private void Die()
    {
        if (numLives < 0)
        {
            isAlive = false;
        }
        deathSoundEffect.Play();
        rb.bodyType = RigidbodyType2D.Static;
        anim.SetTrigger("death");
    }

    private void ResetCharacter()
    {
        if (numLives > 0)
        {
            player.position = new Vector3(changeRespawn.respawnX, changeRespawn.respawnY, player.position.z);
            anim.ResetTrigger("death");
            anim.SetTrigger("respawn");
            rb.bodyType = RigidbodyType2D.Dynamic;
            numLives--;
        }
        else
        {
            RestartLevel();
        }
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
