using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public Sound[] sounds;

    void Awake()
    {
        foreach (Sound s in sounds)
        {
            SoundToAudioSource(s);
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogError(name + "사운드가 없습니다");
            return;
        }
        
        s.source.Play();
    }
    
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogError(name + "사운드가 없습니다");
            return;
        }
        
        s.source.Stop();
    }

    public void PlayBgm()
    {
        Sound bgm2D = Array.Find(sounds, sound => sound.name == "2DMusic");
        Sound bgm3D = Array.Find(sounds, sound => sound.name == "3DMusic");
        
        bgm2D.source.Play();
        bgm3D.source.volume = 0f;
        bgm3D.source.Play();
    }
    
    public void ChangeTo2DMusic()
    {
        StartCoroutine(FadeIn("2DMusic"));
        StartCoroutine(FadeOut("3DMusic"));
    }

    public void ChangeTo3DMusic()
    {
        StartCoroutine(FadeIn("3DMusic"));
        StartCoroutine(FadeOut("2DMusic"));
    }

    public void StopStageBgm()
    {
        Stop("2DMusic");
        Stop("3DMusic");
    }

    IEnumerator FadeIn(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogError(name + "사운드가 없습니다");
            yield break;
        }
        
        s.source.volume = 0f;
        s.source.mute = false;
        
        while (s.source.volume < s.volume)
        {
            s.source.volume += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator FadeOut(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s == null)
        {
            Debug.LogError(name + "사운드가 없습니다");
            yield break;
        }
        
        s.source.volume = 0.75f;
        
        while (s.source.volume <= 0)
        {
            s.source.volume -= Time.deltaTime;
            Debug.Log("fadeout : " + s.source.volume);
            yield return null;
        }

        s.source.mute = true;
    }

    public void SoundToAudioSource(Sound s)
    {
        s.source = gameObject.AddComponent<AudioSource>();
        s.source.clip = s.clip;

        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.loop = s.loop;
    }

    public Sound GetSound(string name)
    {
        return Array.Find(sounds, sound => sound.name == name);
    }
}