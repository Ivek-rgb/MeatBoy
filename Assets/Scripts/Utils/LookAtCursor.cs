using Unity.VisualScripting;
using UnityEngine;

public class LookAtCursor : MonoBehaviour
{
    // TODO: create a script that has offsets and other modular stuff you can track and move player with 
    [Header("Degree offset vars")] 
    public float finalOffsetDegree = 0;
    public Transform pivotPoint;

    [Header("Rotation speed vars")] 
    public float rotationSpeed = 50f; 

    private void Start()
    {
        if (pivotPoint == null)
            pivotPoint = this.transform; 
    }


    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = pivotPoint.position.z;

        Vector3 direction = mousePosition - pivotPoint.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        angle += finalOffsetDegree;
        
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    
    private void OnDrawGizmosSelected()
    {
        if (pivotPoint == null)
            pivotPoint = this.transform; 

        Gizmos.color = Color.blue; 
        Gizmos.DrawWireSphere(pivotPoint.transform.position, 0.1f);
    }

    
}
