using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCollector : MonoBehaviour
{
    [HideInInspector] public int cards = 0;

    [SerializeField] private Text cardsText;
    [SerializeField] private Text livesText;
    [SerializeField] private PlayerLife playerLife;
    [SerializeField] private AudioSource collectionSoundEffect;

    private void Update()
    {
        livesText.text = "Lives: " + playerLife.numLives;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Card"))
        {
            collectionSoundEffect.Play();
            Destroy(collision.gameObject);
            cards++;
            playerLife.numLives++;
            cardsText.text = "Cards:" + cards + "/5";
        }
    }
}
