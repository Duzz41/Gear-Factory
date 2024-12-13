using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;


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





}
