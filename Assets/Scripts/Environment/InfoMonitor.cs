using TMPro;
using UnityEngine;

public class InfoMonitor : MonoBehaviour
{
    
    public GameObject infoPanel; 
    public TextMeshProUGUI infoText; 
    [Header("Text settings")]
    [TextArea(5, 30)]
    public string displayText = "PLACEHOLDER TEXT"; 
    [Header("Fade duration")]
    public float fadeDuration = 1f; 

    private bool isPlayerInRange = false;
    private CanvasGroup canvasGroup;

    void Start()
    {
        infoPanel.SetActive(false); 
        canvasGroup = infoPanel.GetComponent<CanvasGroup>(); 
        if (canvasGroup == null)
        {
            canvasGroup = infoPanel.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0; 
    }

    void Update()
    {
        if (isPlayerInRange && canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / fadeDuration; 
        }
        else if (!isPlayerInRange && canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime / fadeDuration; 
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            infoPanel.SetActive(true); 
            infoText.text = displayText; 
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
