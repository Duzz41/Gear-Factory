using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public Sound[] musicSounds, sfxSounds, typeSounds;
    public AudioSource musicSource, sfxSource, carSource;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        PlayMusic("Main Menu");
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.soundName == name);

        if (s == null)
        {
            Debug.LogError("Sound: " + name + " not found!");
            return;
        }
        else
        {
            musicSource.clip = s.soundClip;
            musicSource.Play();
        }
    }
    public void PlayCar(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.soundName == name);

        if (s == null)
        {
            Debug.LogError("Sound: " + name + " not found!");
            return;
        }
        else
        {
            if (s.loop)
            {
                carSource.clip = s.soundClip;
                carSource.loop = true;
                carSource.Play();
            }
            else
            {
                carSource.PlayOneShot(s.soundClip);
            }
        }

    }
    public void PlaySfx(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.soundName == name);

        if (s == null)
        {
            Debug.LogError("Sound: " + name + " not found!");
            return;
        }
        else
        {
            if (s.loop)
            {
                sfxSource.clip = s.soundClip;
                sfxSource.loop = true;
                sfxSource.Play();
            }
            else
            {
                sfxSource.PlayOneShot(s.soundClip);
            }
        }
    }

    public void StopSfx()
    {
        sfxSource.clip = null;
        sfxSource.Stop();
        sfxSource.loop = false; // Reset loop to false
    }
    public void StopCar()
    {
        carSource.clip = null;
        carSource.Stop();
        carSource.loop = false; // Reset loop to false
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        sfxSource.volume = volume;
        carSource.volume = volume;
    }

    public void SetSfxVolume(float volume)
    {
        sfxSource.volume = volume;
    }

    public void SetCarVolume(float volume)
    {
        carSource.volume = volume;
    }



}
