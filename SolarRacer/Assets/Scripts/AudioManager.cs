using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    void Awake()
    {

        DontDestroyOnLoad(gameObject);

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }

    }

    public void Play(string name)
    {
        Sound toPlay = Array.Find(sounds, sound => sound.name == name);
        if (toPlay == null)
        {
            Debug.LogWarning("Sound: " + name + " not found");
            return;
        }
        toPlay.source.Play();
    }

    public void StopEverything()
    {
        foreach (Sound s in sounds)
        {
            if (s.name != "Victory")
                s.source.Stop();
        }
    }

    public void SetPitch(string name, float value)
    {
        Sound toPlay = Array.Find(sounds, sound => sound.name == name);
        if (toPlay == null)
        {
            Debug.LogWarning("Sound: " + name + " to change pitch not found");
            return;
        }
        toPlay.source.pitch = value;
    }
}
