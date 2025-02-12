using System;
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

    // launch forces for player giblets 
    private const float MinLaunchForce = 3f;
    private const float MaxLaunchForce = 5f;
    private const float LaunchAngleRange = 200f;
    private const float RotationForce = 10f; 
    
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

    public void OnPlayerTakeDamage(int damageAmount)
    {
        lives -= damageAmount;
        if (lives == 0)
        {
            OnPlayerDeathExlpode();
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

    private Vector2 GetRandomDirection()
    {
        float randomAngle = Random.Range(-LaunchAngleRange, LaunchAngleRange);
        return new Vector2(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad));
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if(playerController) // TODO: fix this constant updating of this shite
            OnPlayerTakeDamage(0); 
    }

}
