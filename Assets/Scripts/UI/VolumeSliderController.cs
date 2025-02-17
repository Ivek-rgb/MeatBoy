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
        
        if (_levelManager.storedSoundValues != null)
        {
            _slider.value = channel == AudioChannel.Music
                ? _levelManager.storedSoundValues[0]
                : _levelManager.storedSoundValues[1]; 
            SetVolume();
        }else _slider.value = channel == AudioChannel.Music ? _audioManager.GetMusicVolume() : _audioManager.GetSfxVolume();
    }

    public void SetVolume()
    {
        float value = _slider.value;
        // quick patch to also let the event method on value change use it  
        LevelManager levelManager = LevelManager.Instance; 
        AudioManager currAudioManager = FindFirstObjectByType<AudioManager>();
        
        
        if (currAudioManager == null) return;

        if (channel == AudioChannel.Music)
        {
            if(levelManager) levelManager.storedSoundValues[0] = value; 
            currAudioManager.SetMusicVolume(value);    
        }
        else
        {
            if(levelManager) levelManager.storedSoundValues[1] = value;
            currAudioManager.SetSfxVolume(value);
            
        }

    }
    
}
