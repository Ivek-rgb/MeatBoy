using UnityEngine;

/*
 * NOTE TO ANYONE READING: this is a patch script just to remove gravity affected body parts --Bench
 * It's quite a bad one, but it will suffice for this project 
 */
public class CleanupGibs : MonoBehaviour
{

    public Camera designatedCamera;
    [Header("Additional offset")]
    public float additionalHeightOffset = -2f; 
    private float _camHeight; 
    
    void Start()
    {
        if (!designatedCamera) return;
        _camHeight = designatedCamera.orthographicSize * 2; 
    }

    // Update is called once per frame
    void Update()
    {
        if (!designatedCamera) return; 
        
        if ((designatedCamera.transform.position.y - _camHeight / 2) + additionalHeightOffset > transform.position.y)
        {
            Destroy(this.gameObject);
        }
        
    }
    
    
}
