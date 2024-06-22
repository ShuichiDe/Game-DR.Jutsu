using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour
{
    public float speed = 5f;
    private bool isStopped = false;

    private void Update()
    {
        if (!isStopped)
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
        }
    }

    public void StopMovement()
    {
        isStopped = true;
    }

    public void ResumeMovement()
    {
        isStopped = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.Question(other.GetComponent<Player>(), this);
        }
    }
}
