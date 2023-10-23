using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Sound[] sounds;
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

        } else {
            Destroy(gameObject);
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    public void PlaySound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            // Debug.Log("Sound: " + name + " not found!");
            return;
        } else {
            s.source.Play();
        }
    }

    public void StopSound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            // Debug.Log("Sound: " + name + " not found!");
            return;
        } else {
            s.source.Stop();
        }
    }

    public bool IsPlaying(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            // Debug.Log("Sound: " + name + " not found!");
            return false;
        } else {
            return s.source.isPlaying;
        }
    }



    // public void FadeOutAndStop(string name, float fadeTime)
    // {
    //     Sound s = Array.Find(sounds, sound => sound.name == name);
    //     if (s == null)
    //     {
    //         Debug.Log("Sound: " + name + " not found!");
    //         return;
    //     } 
    //     else 
    //     {
    //         StartCoroutine(FadeOutCoroutine(s, fadeTime));
    //     }
    // }

    // private IEnumerator FadeOutCoroutine(Sound sound, float fadeTime)
    // {
    //     float startVolume = sound.source.volume;

    //     while (sound.source.volume > 0)
    //     {
    //         sound.source.volume -= startVolume * Time.deltaTime / fadeTime;
    //         yield return null;
    //     }

    //     sound.source.Stop();
    //     sound.source.volume = startVolume; // Reset the volume to the original value
    // }
}
