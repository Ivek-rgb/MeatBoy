using UnityEngine;

public class ShellCasingCleaner : MonoBehaviour
{
    public float timeToLive = 5f;
    private float _currTimeToLive;
    
    public void Update()
    {
        if(_currTimeToLive >= timeToLive)
            Destroy(this.gameObject);
        _currTimeToLive += Time.deltaTime; 
    }
    
}
