using System;
using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RecombinatorScript : MonoBehaviour
{

    public int instanceID; // just for debug for now --Bench
    private GameManager _gameManager;

    [Header("Door sliding animation params")]
    public float doorSlidingTime = 1f; 
    
    [Header("Animated parts")]
    [Tooltip("Mostly not configurable since prefabs will be used to drag checkpoints on level")]
    public SpriteRenderer door;
    public Transform openDoorPosition;
    public Light2D statusLight; 
    private Vector3 _originalDoorPosition; 
    
    
    
    private void Awake()
    {
        instanceID = gameObject.GetInstanceID(); 
    }

    void Start()
    {
        _gameManager = GameManager.Instance;
        if (door != null)
            _originalDoorPosition = door.transform.position; 
        
        TurnOffLight();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (string.Compare(other.tag, "Player", StringComparison.Ordinal) == 0)
        {
            _gameManager.SetCheckpoint(this);
        }
    }


    private void SetSpriteRendererSortLayer(SpriteRenderer sr, string sortLayerName, int position)
    {
        if (!sr) return;
        sr.sortingLayerName = sortLayerName;
        sr.sortingOrder = position; 
    }

    public IEnumerator IntroThePlayerRespawn(CharacterController2D spawnedCharacter)
    {

        float elapsedTime = 0;
        SetSpriteRendererSortLayer(door, "Player", 5);

        while (elapsedTime < doorSlidingTime)
        {
            door.transform.position = Vector3.Slerp(door.transform.position, openDoorPosition.position, elapsedTime / doorSlidingTime);
            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        door.transform.position = openDoorPosition.position; 
        
        SetSpriteRendererSortLayer(door, "ForeGround", 5);

        Rigidbody2D characterRigidBody = spawnedCharacter.transform.GetComponent<Rigidbody2D>();
        characterRigidBody.linearVelocity = new Vector2(4f, 0f);
        spawnedCharacter.disablePlayerInteractivity = false;

        elapsedTime = 0; 
        
        while (elapsedTime < (doorSlidingTime / 2))
        {
            door.transform.position = Vector3.Slerp(door.transform.position, _originalDoorPosition, elapsedTime / doorSlidingTime);
            elapsedTime += Time.deltaTime;
            yield return null; 
        }

    }

    public void TurnOnLight()
    {
        statusLight.intensity = 1.2f; 
    }

    public void TurnOffLight()
    {
        statusLight.intensity = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
