using System;
using Unity.VisualScripting;
using UnityEngine;

public class LinearMovingScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    [Header("Moving points")]
    public Transform startPoint;
    public Transform endPoint;
    [Header("Object that moves")]
    public Transform platform; 
    [Header("Travel settings vars")] 
    public float maxDistanceDelta;

    
    private int _direction = 1;
    private Rigidbody2D _platformRb;

    private void Start()
    {
        _platformRb = platform.GetComponent<Rigidbody2D>();
    }

    Vector2 TravelTarget()
    {
        if (!startPoint || !endPoint) return transform.position; // do not move if both params have not been set  
        return _direction < 0 ? startPoint.position : endPoint.position; 
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 target = TravelTarget();
        platform.position = Vector2.MoveTowards(platform.position, target, maxDistanceDelta); 
        if ((target - (Vector2)platform.position).magnitude <= 0.1f)
            _direction *= -1;

        
    }

    private void OnDrawGizmosSelected()
    {
        if (!startPoint || !endPoint) return; 
        Gizmos.color = Color.blue; 
        Gizmos.DrawLine(startPoint.position, endPoint.position);
    }
    
 
  
    
}
