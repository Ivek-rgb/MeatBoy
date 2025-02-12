using System;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LivesUIController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public HeartPulse heartPulseController;
    public UnityEngine.UI.Image timesLivesSpriteHandler;
    public UnityEngine.UI.Image numberLivesCounter;

    private GameManager _gameManager;
    private Dictionary<string, Sprite> _numberSpriteDictionary = new Dictionary<string, Sprite>(); 
    
    void Start()
    {

        _gameManager = GameManager.Instance;
        
        _gameManager?.onLivesChanged?.AddListener(UpdateLivesUI);
        Sprite[] loadedSprites = Resources.LoadAll<Sprite>($"Sprites/UI/ui_sprites");
        foreach (Sprite sprite in loadedSprites)
        {
            _numberSpriteDictionary.Add(sprite.name, sprite);
        }

    }

    void UpdateLivesUI(int newLives)
    {
        if (newLives <= 1) HandleRecoloringOfSpriteText(Color.red);
        else HandleRecoloringOfSpriteText(Color.white);
        SetHeartPulseByLives(newLives);
        HandleLivesChangeText(newLives);
    }

    private void HandleRecoloringOfSpriteText(Color newColor)
    {
        if (numberLivesCounter == null || timesLivesSpriteHandler == null)
            return; 

        numberLivesCounter.color = newColor;
        timesLivesSpriteHandler.color = newColor;    
    }

    private void SetHeartPulseByLives(int newLives)
    {

        if (heartPulseController == null)
            return; 
            
        // this can be replaced by more optimal function 
        float clamped = Mathf.Clamp(newLives, 0, 3);
        heartPulseController.pulseSpeed = clamped * Mathf.Exp(-clamped + 1) * 15f; 
        
    }

    private void HandleLivesChangeText(int newLives)
    {
        if (numberLivesCounter == null)
            return;
        numberLivesCounter.sprite = _numberSpriteDictionary[$"{(newLives > 0 ? "sprite_" + newLives : "times")}"]; 
    }
    
    
}
