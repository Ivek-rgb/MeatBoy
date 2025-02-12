using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class LivesUIController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public HeartPulse heartPulseController;
    public Image timesLivesSpriteHandler;
    public Image numberLivesCounter;

    private GameManager _gameManager = GameManager.Instance; 
    
    void Start()
    {
        _gameManager?.onLivesChanged?.AddListener(UpdateLivesUI);        
    }

    void UpdateLivesUI(int newLives)
    {
        if (newLives <= 1)
        {
            numberLivesCounter.color = Color.red;
            timesLivesSpriteHandler.color = Color.red; 
        }
        else
        {
            numberLivesCounter.color = Color.white;
            timesLivesSpriteHandler.color = Color.white; 
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
