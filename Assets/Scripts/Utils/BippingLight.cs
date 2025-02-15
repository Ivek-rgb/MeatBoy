using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BippingLight : MonoBehaviour
{
    public Light2D light2D;   
    public float minIntensity = 0.5f;
    public float maxIntensity = 2.0f;
    public float speed = 2.0f; 

    private float time;

    void Start()
    {
        if (light2D == null)
            light2D = GetComponent<Light2D>(); 
    }

    void Update()
    {
        time += Time.deltaTime * speed;
        light2D.intensity = Mathf.Lerp(minIntensity, maxIntensity, (Mathf.Sin(time) + 1) / 2);
    }
    
}
