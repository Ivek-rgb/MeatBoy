using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking.PlayerConnection;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
 
    public CharacterController2D playerController;
    public Camera mainLevelCamera;
    public int lives; 
    public UnityEvent<int> onLivesChanged;
    
    private Dictionary<string, List<GameObject>> _gibletStorage = new Dictionary<string, List<GameObject>>();
    private Sprite[] _storedBloodSprites; 
    private RecombinatorScript _currentCheckpoint;
    private HashSet<int> _collectionOfCheckpoints = new HashSet<int>(); 
    private GameObject _prefabbedPlayer; // use: only for respawning player 
    
    // launch forces for player giblets 
    private const float MinLaunchForce = 3f;
    private const float MaxLaunchForce = 5f;
    private const float LaunchAngleRange = 200f;
    private const float RotationForce = 10f;

    private List<GameObject> _bloodDecalContainer = new List<GameObject>();
    [Header("DecalSettings")]
    public int maxBloodDecalAmount = 100; 

    private bool _checkMade = true; 
    
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return; 
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public Sprite OnPlayerSplatterBlood(GameObject bloodSplatterDecal)
    {

        if (_bloodDecalContainer.Count == maxBloodDecalAmount)
        {
            int toRemove = Mathf.Clamp(maxBloodDecalAmount / 10, 1, maxBloodDecalAmount / 10);

            for (int i = 0; i < toRemove; i++)
                Destroy(_bloodDecalContainer[toRemove]);

            _bloodDecalContainer.RemoveRange(0, Mathf.Clamp(toRemove, 1, toRemove));
        }
        
        _bloodDecalContainer.Add(bloodSplatterDecal);
        return _storedBloodSprites[Random.Range(0, _storedBloodSprites.Length)];
    }


    public void OnPlayerTakeDamage(int damageAmount)
    {

        lives -= damageAmount;
        if (lives == 0)
        {
            OnPlayerDeathExlpode();
            OnPlayerTakeDamage(-1);
            StartCoroutine(ResurrectPlayerProcedure());
        }
        
        onLivesChanged?.Invoke(lives);

    }

    public void OnPlayerDeathExlpode()
    {
        LaunchAllGiblets();
        playerController.DestroyWholePlayer();
    }

    void Start()
    {
        GameObject[] gibletPrefabs = Resources.LoadAll<GameObject>("Prefabs/OnDeathGibs");
        foreach (var prefab in gibletPrefabs)
        {

            string nameOfPocket = prefab.name.Split("-")[0]; 
            
            if (!_gibletStorage.ContainsKey(nameOfPocket))
            {
                _gibletStorage.Add(nameOfPocket, new List<GameObject>());
            }
            
            _gibletStorage[nameOfPocket].Add(prefab);
            
        }

        _prefabbedPlayer = Resources.Load<GameObject>("Prefabs/Player/Player");
        _storedBloodSprites = Resources.LoadAll<Sprite>("Sprites/Blood/blood-sprites"); 
    }
    
    public void LaunchAllGiblets()
    {
        foreach (var category in _gibletStorage)
        {
            
            GameObject randomGiblet = category.Value[Random.Range(0, category.Value.Count)];
            GameObject giblet = Instantiate(randomGiblet, playerController.transform.position, Quaternion.identity);

            giblet.AddComponent<CleanupGibs>();
            CleanupGibs scriptForThisGiblet = giblet.GetComponent<CleanupGibs>();
            scriptForThisGiblet.designatedCamera = mainLevelCamera; 
            
            Rigidbody2D rb = giblet.GetComponent<Rigidbody2D>(); // eh kinda expensive but ok --Bench

            float launchForce = Random.Range(MinLaunchForce, MaxLaunchForce); 
            
            if (rb)
            {
                Vector2 randomDirection = GetRandomDirection();
                rb.AddForce(randomDirection * launchForce, ForceMode2D.Impulse);

                float randomTorqueForRotation = Random.Range(-RotationForce, RotationForce);
                rb.AddTorque(randomTorqueForRotation, ForceMode2D.Impulse);
                
                
            }
        }
    }

    public void SetCheckpoint(RecombinatorScript newCheckpointCandidate)
    {
        int instanceID = newCheckpointCandidate.instanceID; 
        if (!_collectionOfCheckpoints.Contains(instanceID))
        {
            _currentCheckpoint = newCheckpointCandidate;
            _collectionOfCheckpoints.Add(instanceID); 
        }

    }

    private Vector2 GetRandomDirection()
    {
        float randomAngle = Random.Range(-LaunchAngleRange, LaunchAngleRange);
        return new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad));
    }

    public IEnumerator ResurrectPlayerProcedure()
    {
        
        yield return new WaitForSeconds(0.5f);
        GameObject instantiatedPlayer = Instantiate(_prefabbedPlayer, _currentCheckpoint.transform.position, Quaternion.identity);
        playerController = instantiatedPlayer.GetComponentInChildren<CharacterController2D>();
        if (playerController)
        {
            playerController.disablePlayerInteractivity = true; 
            StartCoroutine(_currentCheckpoint.IntroThePlayerRespawn(playerController));
            mainLevelCamera.GetComponent<CameraFollowBehaviour>().target = playerController.transform;
            yield return new WaitForSeconds(1f); 
        }
        
    }

    void FixedUpdate()
    {
        // Damn it with time constraints, this patch is inefficient as hell, just for a health update by events --Bench
        if (playerController && _checkMade)
        {
            OnPlayerTakeDamage(0);
            _checkMade = false; 
        }

    }

}
