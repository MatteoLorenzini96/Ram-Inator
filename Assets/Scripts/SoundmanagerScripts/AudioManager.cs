using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    private float initialSFXPitch; // Memorizza il pitch iniziale degli SFX


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        initialSFXPitch = sfxSource.pitch;

    }

    private void Start()
    {
        PlayMusic("Theme");
    }


    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            //Debug.Log("Music Not Found");
        }

        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            //Debug.Log("Sound Not Found");
        }
        else
        {
            // Calcola un pitch casuale tra -0.3 e +0.3
            float randomPitch = 1 + UnityEngine.Random.Range(-0.3f, +0.3f);

            // Moltiplica il pitch casuale per il volume corrente
            sfxSource.pitch = randomPitch * sfxSource.volume;

            // Riproduce il suono
            sfxSource.PlayOneShot(s.clip);

            sfxSource.pitch = initialSFXPitch;
        }
    }


    public void ToggleMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Pause();  // Ferma la musica quando viene mutata
        }
        else
        {
            musicSource.Play();
        }
        //Debug.Log("Musica Mutata");
    }

    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
        if (sfxSource.mute)
        {
            sfxSource.Stop();  // Ferma gli effetti sonori quando vengono mutati
        }
        //Debug.Log("SFX Mutati");
    }

    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
