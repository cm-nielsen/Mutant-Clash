using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBehavior : MonoBehaviour
{
    public UnitStats stats;

    public float baseMoveSpeed;
    bool movingLeft;

    public Sprite idleSprite;
    public Sprite pushAnticipationSprite;
    public Sprite pushSprite;
    public Sprite defeatedSprite;

    SpriteRenderer spriteRenderer;

    int health;

    public void Init(bool goLeft, Color colour)
    {
        health = stats.health;
        movingLeft = goLeft;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = idleSprite;
        spriteRenderer.flipX = !movingLeft;
        spriteRenderer.color = colour;
    }

    void Update()
    {
        transform.position += Time.deltaTime * baseMoveSpeed * stats.speed *
            (movingLeft ? Vector3.left : Vector3.right);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        health -= 1;
        if(health <= 0)
        {
            // get pushed back, play sit animation, disable collision and start death coroutine
            Destroy(gameObject);
        }
    }
}
