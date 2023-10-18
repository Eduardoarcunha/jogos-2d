using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [Range(0, 1f)]
    public float volume;

    [Range(0, 1f)]
    public float pitch; // Frequency

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}
