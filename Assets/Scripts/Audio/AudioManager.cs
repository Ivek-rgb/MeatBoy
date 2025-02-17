using UnityEngine;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Audio Source")] 
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Used clips")] 
    public AudioClip backgroundMusic; 
    public AudioClip jump;
    public AudioClip takeDamage;
    public AudioClip death; 
    
    void Start()
    {
        musicSource.clip = backgroundMusic; 
        musicSource.Play();
    }

    public void PlaySfx(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void TakeDamage()
    {
        PlaySfx(takeDamage);
    }

    public void Jump()
    {
        PlaySfx(jump);
    }

    public void Death()
    {
        PlaySfx(death);
    }

    public void Land()
    {
        Death();
    }

    public void SetMusicVolume(float value)
    {
        Debug.Log(value);
        musicSource.volume = value;
    }

    public void SetSfxVolume(float value)
    {
        Debug.Log(value);
        sfxSource.volume = value; 
    }
    
    public float GetMusicVolume()
    {
        return musicSource.volume; 
    }

    public float GetSfxVolume()
    {
        return sfxSource.volume; 
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ContinueMusic()
    {
        musicSource.Play();
    }

}
