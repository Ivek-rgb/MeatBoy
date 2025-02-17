using TMPro;
using UnityEngine;

public class RollCredits : MonoBehaviour
{
    public float scrollSpeed = 50f; 
    private RectTransform _textTransform;
    private string _textToDisplay = "";
    public TextMeshProUGUI textMeshProUGUI;
    private LevelManager _levelManager;
    private bool _isOver; 
    
    void Start()
    {

        int numOfDeaths = 0; 
        _levelManager = LevelManager.Instance;
        
        if(_levelManager)
            numOfDeaths = _levelManager.GetNumberOfTotalDeaths();

        _textToDisplay = $@"Thank you for playing!
Times died: {numOfDeaths}

Team - TvzByte

Sprites/Images:
Ivan Kuštović
Ivan Benčić

Level design: 
Hrvoje Renato Šokčić

Programming:
Ivan Benčić

Animation/Emitters:
Hrvoje Renato Šokčić

Ingame SFX:
Hrvoje Renato Šokčić

Story: 
ChatGPT

Special thanks: 
Leo Znika
Matija Lukec
Mirko Vrlec



The end? 
";
        
        _textTransform = GetComponent<RectTransform>();
        if (textMeshProUGUI)
            textMeshProUGUI.text = _textToDisplay; 

    }

    void Update()
    {
        _textTransform.anchoredPosition += Vector2.up * (scrollSpeed * Time.deltaTime);

        if (Input.anyKeyDown && !_isOver)
        {
            _levelManager.LoadScene("StartScreen");
        }

    }
    
}
