using System;
using System.Collections;
using UnityEngine;

public class DoorWormhole : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private GameManager _gameManager; 
    
    [Header("Door sliding animation params")]
    public float doorSlidingTime = 1f; 
    
    [Header("Animated parts")]
    [Tooltip("Mostly not configurable since prefabs will be used to drag checkpoints on level")]
    public SpriteRenderer doorL;
    public Transform openDoorPositionL; 
    private Vector3 _originalDoorPositionL; 
    
    public SpriteRenderer doorR;
    public Transform openDoorPositionR; 
    private Vector3 _originalDoorPositionR;

    // proceed to another wormhole 
    public DoorWormhole anotherWormhole;

    // will be used on determining fate when up is presed
    public bool isExit = false;

    // damn what a nice (thread)Coroutine locking mechanism, I wonder what could go wrong using this??? --Bench
    private bool _coroutineLock = false;
    private readonly bool[] _coroutineActivated = {false, false};

    public float teleportationSicknessDuration = 3f;
    private float _lastTeleport ;
    
    void Start()
    {
        
        _gameManager = GameManager.Instance;
        
        if (doorL != null)
            _originalDoorPositionL = doorL.transform.position; 
        
        if (doorR != null)
            _originalDoorPositionR = doorR.transform.position;

        _lastTeleport = teleportationSicknessDuration; 

    }
    
    // Update is called once per frame
    void Update()
    {
        _lastTeleport += Time.deltaTime; 
    }

    private IEnumerator OpenDoors()
    {

        if (_coroutineLock) yield break;
        _coroutineLock = true;
        
        float elapsedTime = 0;

        while (elapsedTime < doorSlidingTime)
        {
            doorL.transform.position = Vector3.Slerp(doorL.transform.position, openDoorPositionL.position, elapsedTime / doorSlidingTime);
            doorR.transform.position = Vector3.Slerp(doorR.transform.position, openDoorPositionR.position, elapsedTime / doorSlidingTime);
            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        doorL.transform.position = openDoorPositionL.position; 
        doorR.transform.position = openDoorPositionR.position;

        _coroutineLock = false;

        _coroutineActivated[1] = false; 

    }

    private IEnumerator CloseDoors()
    {
        if (_coroutineLock) yield break;
        
        _coroutineLock = true; 
        
        float elapsedTime = 0; 
        
        while (elapsedTime < (doorSlidingTime / 2))
        {
            doorL.transform.position = Vector3.Slerp(doorL.transform.position, _originalDoorPositionL, elapsedTime / doorSlidingTime);
            doorR.transform.position = Vector3.Slerp(doorR.transform.position, _originalDoorPositionR, elapsedTime / doorSlidingTime);
            elapsedTime += Time.deltaTime;
            yield return null; 
            
        }
        
        doorL.transform.position = _originalDoorPositionL; 
        doorR.transform.position =_originalDoorPositionR;

        _coroutineLock = false; 
        
        _coroutineActivated[0] = false; 

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") &&  !_coroutineActivated[0] )
        {
            StartCoroutine(OpenDoors());
            _coroutineActivated[0] = true; 
        }
    }
    
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_coroutineActivated[1] )
        {
            StartCoroutine(CloseDoors());
            _coroutineActivated[0] = false;
        }
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        // open doors
        if (other.CompareTag("Player"))
        {
            
            if (Input.GetAxis("Vertical") > 0 && anotherWormhole && _lastTeleport >= teleportationSicknessDuration)
            {
                StartCoroutine(_gameManager.TeleportCharacter(anotherWormhole.transform.position));
                _lastTeleport = 0; 
            }else if (Input.GetAxis("Horizontal") > 0 && isExit)
            {
                // here be exit script 
            }
            
        }


    }
}
