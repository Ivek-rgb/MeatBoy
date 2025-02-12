using UnityEngine;

public class CameraFollowBehaviour : MonoBehaviour
{
    public Transform target; 
    
    [Header("Optional offset")]
    public Vector3 offset = new Vector3(0f, 0f, 0f);  
    [Header("Camera smoothing effect")]
    public float smoothSpeed = 5f;  

    void LateUpdate()
    {
        if (target == null)
            return;
        
        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = transform.position.z; 
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        
    }
    
}
