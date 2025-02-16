using System.Text;
using TMPro;
using UnityEngine;

public class TerminalEffect : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public TextMeshProUGUI textToModify;
    public float terminalBlinkingTime = 2f;
    private float _currTime; 
    private bool _isCoroutineGoing; 
    
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        _currTime += Time.deltaTime;
        if (_currTime >= terminalBlinkingTime)
        {
            StringBuilder textStr = new StringBuilder(textToModify.text);
            if (textStr[^1] == '_')
                textStr[^1] = ' '; 
            else
                textStr[^1] = '_';
            textToModify.isRightToLeftText = true;
            textToModify.text = textStr.ToString();
            textToModify.isRightToLeftText = false;
            _currTime = 0;
        }

    }
}
