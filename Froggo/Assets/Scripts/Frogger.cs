using System;
using System.Collections;
using UnityEngine;

public class Frogger : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public Sprite idleSprite;
    public Sprite leapSprite;
    public Sprite deathSprite;

    private Vector2 spawnPosition;

    private float farthestRow;

    AudioSource audioSource;

    private GameManager gameManager;

    public Vector3 show;
    public Vector3 move;

    

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spawnPosition = transform.position;
    }

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gameManager =FindObjectOfType<GameManager>();
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            Move(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 180f);
            Move(Vector2.down);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            Move(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            transform.rotation = Quaternion.Euler(0f, 0f, -90f);
            Move(Vector2.right);
        }

    }

    private void Move(Vector2 direction)
    {
        //transform.position += direction;
        Vector2 destination = (Vector2)transform.position + direction;
        audioSource.Play();
        Collider2D barrier = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Barrier"));
        Collider2D platform = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Platform"));
        Collider2D obstacle = Physics2D.OverlapBox(destination, Vector2.zero, 0f, LayerMask.GetMask("Obstacle"));


        if (barrier != null)
        {
            return;
        }

        if(platform != null)
        {
            transform.SetParent(platform.transform);
        }
        else
        {
            transform.SetParent(null);
        }

        if(obstacle != null && platform == null)
        {
            transform.position = destination;
            Death();
        }
        else
        {
            if(destination.y > farthestRow)
            {
                farthestRow = destination.y;
                gameManager.AdvancedRow();
            }

            StartCoroutine(Leap(destination));
        }

    }

    private IEnumerator Leap(Vector2 destination)
    {
        Vector2 startPosition = transform.position;
        float elapsed = 0f;
        float duration = 0.125f;

        spriteRenderer.sprite = leapSprite;

        while(elapsed < duration)
        {
            float t = elapsed / duration;
            transform.position = Vector2.Lerp(startPosition, destination, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = destination;
        spriteRenderer.sprite = idleSprite;
    }

    public void Death()
    {
        StopAllCoroutines();
        transform.rotation = Quaternion.identity;
        spriteRenderer.sprite = deathSprite;
        enabled = false;

        gameManager.Died();
    }

    public void Respawn()
    {
        StopAllCoroutines();
        transform.rotation = Quaternion.identity;
        transform.position = spawnPosition;
        farthestRow = spawnPosition.y;
        spriteRenderer.sprite = idleSprite;
        gameObject.SetActive(true);
        enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(enabled && other.gameObject.layer == LayerMask.NameToLayer("Obstacle") && transform.parent == null)
        {
            Death();
        }
    }
}
