using System;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class MoveWithEscalator : MonoBehaviour
{
    private Rigidbody2D _escalatorRigidbody2D;
    private Vector2 _previousPosition;
    public float frequencyRefreshSecs = 1f;
    private float _currentTimerTime;
    public float additionalForce = 2f; 
    
    
    public float frequencyRefreshInner = 0.1f;
    private float _currentInnerTimeTimer; 
    
    
    private void Start()
    {
        _escalatorRigidbody2D = GetComponent<Rigidbody2D>();
        _previousPosition = transform.position; 
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if (collision.gameObject.name == $"MainConnectionBone")
        {
            CharacterController2D playerController = collision.gameObject.GetComponent<CharacterController2D>();
            if (playerController)
            {
                Vector2 platformMovement = (Vector2)transform.position - _previousPosition;
                playerController.AddOnVelocity(platformMovement * additionalForce);

            }
        }
        
    }

    private void LateUpdate()
    {

        if (_currentTimerTime > frequencyRefreshSecs)
        {
            _previousPosition = transform.position;
            _currentTimerTime = 0; 
        }
        else
        {
            _currentTimerTime += Time.deltaTime; 
        }

    }

    
}
