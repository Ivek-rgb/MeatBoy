using UnityEngine;
using UnityEngine.UI;

public class VolumeSliderController : MonoBehaviour
{
    public enum AudioChannel { Music, Sfx }
    
    [SerializeField] private AudioChannel channel; 
    private Slider _slider;
    private LevelManager _levelManager;
    private AudioManager _audioManager; 
    
    private void Start()
    {
        _slider = GetComponent<Slider>();
        _levelManager = LevelManager.Instance;

        _audioManager = _levelManager.GetCurrentAudioManager();
        if (_audioManager == null) return; 

        _slider.value = channel == AudioChannel.Music ? _audioManager.GetMusicVolume() : _audioManager.GetSfxVolume();
    }

    public void SetVolume()
    {
        float value = _slider.value;
        AudioManager currAudioManager = FindFirstObjectByType<AudioManager>();
        
        if (currAudioManager == null) return;
        
        if (channel == AudioChannel.Music)
            currAudioManager.SetMusicVolume(value);
        else
            currAudioManager.SetSfxVolume(value);
    }
    
}
