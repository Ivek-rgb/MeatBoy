using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class AntiGravitationalFields : MonoBehaviour
{
    
    [Header("Reach")]
    [Tooltip("Modifies the reach of the collider for force application")]
    public Vector2 colliderSize; 
    public BoxCollider2D boxCollider;
    private Vector2 _initOffset = Vector2.zero; 
    
    [Header("Force settings")] 
    public float forceMultiplier;
    public Vector2 forceDirection;

    private void OnTriggerStay2D(Collider2D other)
    {
        
        Rigidbody2D[] containedRigidbodies = other.GetComponents<Rigidbody2D>();
        foreach (var rb in containedRigidbodies)
        {
            rb.AddForce(forceDirection * forceMultiplier, ForceMode2D.Impulse);
        }
    }

}
