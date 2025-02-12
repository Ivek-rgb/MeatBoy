using UnityEngine;

public class HeartPulse : MonoBehaviour
{

    public float pulseSpeed = 10f;  
    public float pulseAmount = 0.1f; 

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale; 
    }

    void Update()
    {
        float scaleFactor = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = originalScale * scaleFactor;
    }

}
