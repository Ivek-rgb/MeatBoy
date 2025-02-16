using TMPro;
using UnityEngine;

public class FetchLevelName : MonoBehaviour
{
    public TextMeshProUGUI textHandler;
    private GameManager _gameManager;
    
    void Start()
    {
        _gameManager = GameManager.Instance;
        textHandler.text = _gameManager.levelName; 
    }

}
