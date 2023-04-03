using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Home : MonoBehaviour
{
    public GameObject happyFrog;

    private void OnEnable()
    {
        happyFrog.SetActive(true);
    }

    private void OnDisable()
    {
        happyFrog.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            enabled = true;

            FindObjectOfType<GameManager>().HomeOccupied();
        }
    }

    }
