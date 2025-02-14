using System;
using Unity.VisualScripting;
using UnityEngine;

public class MoveWithEscalator : MonoBehaviour
{
    private Rigidbody2D _escalatorRigidbody2D;
    private Vector2 _previousPosition;
    
    private void Start()
    {
        _escalatorRigidbody2D = GetComponent<Rigidbody2D>();
        _previousPosition = transform.position; 
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == $"MainConnectionBone")
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 platformMovement = (Vector2)transform.position - _previousPosition;
                rb.linearVelocity += platformMovement; 
            }
        }
        
    }

    private void LateUpdate()
    {
        _previousPosition = transform.position; 
    }

    
}
