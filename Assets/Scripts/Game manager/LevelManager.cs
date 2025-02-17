using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    [SerializeField] private GameObject _loaderCanvas;
    [SerializeField] private GameObject _pauseMenu; 
    private GameManager _gameManager;
    private bool _isPaused;
    public int storedNumOfLives = 0;
    public string currentSceneName;
    private HashSet<string> _completedLevels = new HashSet<string>();
    private int _deathCounter;
    private AudioManager _currLevelAudioManager; 
    
    
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
        _completedLevels.Add("Level-1");
        _completedLevels.Add("LevelSelection");
        _completedLevels.Add("StartScreen");
        
    }

    void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        _loaderCanvas.SetActive(false);
        _pauseMenu.SetActive(false);
        _currLevelAudioManager =  FindFirstObjectByType<AudioManager>();

    }

    private void TogglePause(bool forced = false)
    {
        if (currentSceneName == "StartScreen" && !forced) return; 
        _isPaused = !_isPaused;
        Time.timeScale = _isPaused ? 0 : 1;
        
        if (_currLevelAudioManager)
        {
            if(_isPaused) _currLevelAudioManager.PauseMusic();
            else _currLevelAudioManager.ContinueMusic();
        }

        _pauseMenu.SetActive(_isPaused);
    }

    private async Task<GameManager> UsurpGameManager()
    {
        GameManager gm = null;
        int attempts = 5;

        while (gm == null && attempts > 0)
        {
            gm = FindFirstObjectByType<GameManager>();
            if (gm == null)
            {
                await Task.Delay(100); 
            }
            attempts--;
        }
        return gm;
    }
    
    
    private async Task<AudioManager> UsurpAudioManager()
    {
        AudioManager am = null;
        int attempts = 5;

        while (am == null && attempts > 0)
        {
            am = FindFirstObjectByType<AudioManager>();
            if (am == null)
            {
                await Task.Delay(100); 
            }
            attempts--;
        }
        return am;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public bool IsCompletedLevel(string levelName)
    {
        return _completedLevels.Contains(levelName); 
    }

    public async void LoadScene(string sceneName)
    {

        var sceneAsync = SceneManager.LoadSceneAsync($"Scenes/{sceneName}");
        
        if (sceneAsync == null) return;
        _loaderCanvas.SetActive(true);


        while (sceneAsync.progress < 0.9f)
        {
            await Task.Yield();
        }
        
        sceneAsync.allowSceneActivation = false;

        if(_isPaused) TogglePause(true);

        sceneAsync.allowSceneActivation = true;
        
        _completedLevels.Add(sceneName); 
        this.currentSceneName = sceneName;

        _gameManager = await UsurpGameManager();
        _currLevelAudioManager = await UsurpAudioManager(); 
        Debug.Log(_currLevelAudioManager);
        _loaderCanvas.SetActive(false);
        
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ResumeGame()
    {
        TogglePause();
    }

    public void MainMenuu()
    {
        LoadScene("StartScreen");
    }

    public void IncrementNumberOfDeaths()
    {
        this._deathCounter += 1; 
    }

    public void RestartScene()
    {
        LoadScene(currentSceneName);    
    }

    public int GetNumberOfTotalDeaths()
    {
        return _deathCounter; 
    }
    
    public AudioManager GetCurrentAudioManager()
    {
        return _currLevelAudioManager; 
    }


}
