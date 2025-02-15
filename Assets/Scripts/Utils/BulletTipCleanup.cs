using System;
using Unity.VisualScripting;
using UnityEngine;

public class BulletTipCleanup : MonoBehaviour
{
    public float timeToLive;
    private float _currTimeToLive;
    private bool _hasHitSomething; 
    

    public void Update()
    {
        if(_currTimeToLive >= timeToLive)
            Destroy(this.gameObject);
        _currTimeToLive += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            this.gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Shell"))
        {
            this.gameObject.layer = LayerMask.NameToLayer("Default");
            this.gameObject.tag = "Untagged"; 
        }
    }
    
    
}
