using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] Sounds;
    void Awake()
    {
        foreach (Sound s in Sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.loop = s.loop;
        }
    }
    public void Play(string name)
    {
        Sound s = Array.Find(Sounds, sound => sound.name == name);
        s.source.volume = UnityEngine.Random.Range(s.volume - 0.1f, s.volume + 0.1f);
        s.source.pitch = UnityEngine.Random.Range(s.pitch-0.2f,s.pitch+0.2f);
        s.source.panStereo = UnityEngine.Random.Range(0f, 0.5f);
        s.source.Play();
    }
    public void PlayInfos(string name, float pitch)
    {
        Sound s = Array.Find(Sounds, sound => sound.name == name);
        s.source.volume = UnityEngine.Random.Range(s.volume - 0.1f, s.volume + 0.1f);
        s.source.pitch = UnityEngine.Random.Range(pitch - 0.1f, pitch + 0.1f);
        s.source.panStereo = UnityEngine.Random.Range(0f, 0.5f);
        s.source.Play();
    }
    public void Stop(string name)
    {
        Sound s = Array.Find(Sounds, sound => sound.name == name);
        s.source.Stop();
    }
}
