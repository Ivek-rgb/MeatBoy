using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    [SerializeField] 
    private GameObject _loaderCanvas;

    private GameManager _gameManager; 
    
    public static LevelManager Instance { get; private set; }
    
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
    
    void Start()
    {
        _loaderCanvas.SetActive(false);
    }

    private void UsurpGameManager()
    {
        
    }

    void Update()
    {
        
    }
    
    public async void LoadScene(string sceneName)
    {
        var scene = SceneManager.LoadSceneAsync($"Scenes/{sceneName}");
        if (scene != null)
        {
            scene.allowSceneActivation = false;
            _loaderCanvas.SetActive(true);


            scene.allowSceneActivation = true;
            await Task.Delay(2000);
            _loaderCanvas.SetActive(false);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }


}
