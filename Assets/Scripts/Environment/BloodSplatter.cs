using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CircularSaw : MonoBehaviour
{

    private GameManager _gameManager;
    [Header("Decal spawn cooldown")] 
    public float coolDownInSecs = 0.5f;
    private float _timerCooldown = 0f;
    [Header("Blood decal settings")] 
    public float maxRotationAngle = 360f;
    public float maxDecalScale = 1.2f;
    public float minDecalScale = 0.8f; 

    void Start()
    {
        _gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {

        if (_timerCooldown > 0)
        {
            _timerCooldown -= Time.deltaTime; 
        }

    }
    
    public void SpawnSprite(Vector3 position)
    {
        GameObject spriteInstance = new GameObject("BloodSprite");

        SpriteRenderer spriteRenderer = spriteInstance.AddComponent<SpriteRenderer>();

        spriteRenderer.sprite = _gameManager.OnPlayerSplatterBlood(spriteRenderer.gameObject);
        float randomScaleFactor = Random.Range(minDecalScale, maxDecalScale); 
        spriteRenderer.transform.localScale = transform.localScale * randomScaleFactor;

        float randomRotation = Random.Range(0f, maxRotationAngle);
        spriteRenderer.transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);

        Color darkenColor = spriteRenderer.color * Random.Range(0.6f, 0.9f); // darken by 10-20%, just stay as static // it makes the blood more cool --Bench 
        spriteRenderer.color = darkenColor;

        spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

        spriteRenderer.transform.localPosition = Vector3.zero;
        spriteRenderer.sortingLayerName = "ForeGround";
        spriteRenderer.sortingOrder = 5;
        spriteRenderer.transform.parent = transform;
        spriteInstance.transform.position = position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && CanSpawnDecal())
        {
            _timerCooldown = coolDownInSecs; 
            SpawnSprite(other.ClosestPoint(transform.position));
        }
    }

    private bool CanSpawnDecal()
    {
        return _timerCooldown <= 0;
    }
    
}
