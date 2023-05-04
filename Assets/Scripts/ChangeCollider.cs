using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCollider : MonoBehaviour
{
    [SerializeField] public BoxCollider2D box;
    private float origin;

    private void Start()
    {
        box = GetComponent<BoxCollider2D>();
        origin = box.offset.y;
    }

    private void Push()
    {
        box.offset = new Vector2(box.offset.x, origin - 3f);
    }

    private void Pull()
    {
        box.offset = new Vector2(box.offset.x, origin);
    }
}
