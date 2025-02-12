using System;
using Player;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
 
    public CharacterController2D playerController;
    public Camera mainLevelCamera;
    public int lives; 
    public UnityEvent<int> onLivesChanged;

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

    // TODO: generates giblets of gore to explode upon death wait and then switch back on last genetic recombinator  
    public void OnPlayerDeathExlpode()
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        OnPlayerTakeDamage(0); 
    }

}
