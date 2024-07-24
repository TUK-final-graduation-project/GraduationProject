using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("#WaitBGM")]
    public AudioClip WbgmClip;
    public float WbgmVolume;
    AudioSource WbgmPlayer;

    [Header("#BattleBGM")]
    public AudioClip BbgmClip;
    public float BbgmVolume;
    AudioSource BbgmPlayer;

    [Header("#BossBGM")]
    public AudioClip SbgmClip;
    public float SbgmVolume;
    AudioSource SbgmPlayer;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;

    public enum Sfx {Chopping, Jump, Mining, Run, Shovel, Ui, Walk, Wind, Bomb, Magic, Throw, Build, Lab, Level, Item, Fail}

    void Awake()
    {
        instance = this;
        Init();
    }

    void Init()
    {
        // 대기 배경음 초기화
        GameObject Wbgmobject = new GameObject("WBgmPlayer");
        Wbgmobject.transform.parent = transform;
        WbgmPlayer = Wbgmobject.AddComponent<AudioSource>();
        WbgmPlayer.playOnAwake = false;
        WbgmPlayer.loop = true;
        WbgmPlayer.volume = WbgmVolume;
        WbgmPlayer.clip = WbgmClip;

        // 전투 배경음 초기화
        GameObject Bbgmobject = new GameObject("BBgmPlayer");
        Bbgmobject.transform.parent = transform;
        BbgmPlayer = Bbgmobject.AddComponent<AudioSource>();
        BbgmPlayer.playOnAwake = false;
        BbgmPlayer.loop = true;
        BbgmPlayer.volume = BbgmVolume;
        BbgmPlayer.clip = BbgmClip;

        // 보스 배경음 초기화
        GameObject Sbgmobject = new GameObject("SBgmPlayer");
        Sbgmobject.transform.parent = transform;
        SbgmPlayer = Sbgmobject.AddComponent<AudioSource>();
        SbgmPlayer.playOnAwake = false;
        SbgmPlayer.loop = true;
        SbgmPlayer.volume = SbgmVolume;
        SbgmPlayer.clip = SbgmClip;

        // 효과음 초기화
        GameObject sfxobject = new GameObject("SfxPlayer");
        sfxobject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];
  
        for(int index =0; index < sfxPlayers.Length;index++)
        {
            sfxPlayers[index] = sfxobject.AddComponent<AudioSource>();
            sfxPlayers[index].playOnAwake = false;
            sfxPlayers[index].volume = sfxVolume;
        }
    }

    // 대기 배경음악 호출
    public void WPlayBgm(bool isplay)
    {
        if(isplay)
        {
            WbgmPlayer.Play();
        }
        else
        {
            WbgmPlayer.Stop();
        }
    }

    //전투 배경음악 호출
    public void BPlayBgm(bool isplay)
    {
        if (isplay)
        {
            BbgmPlayer.Play();
        }
        else
        {
            BbgmPlayer.Stop();
        }
    }

    //보스 배경음악 호출
    public void SPlayBgm(bool isplay)
    {
        if (isplay)
        {
            SbgmPlayer.Play();
        }
        else
        {
            SbgmPlayer.Stop();
        }
    }

    //효과음 호출
    public void PlaySfx(Sfx sfx)
    {
        for(int index =0; index<sfxPlayers.Length;index++)
        {
            int loopIndex = (index + channelIndex) % sfxPlayers.Length;

            if(sfxPlayers[loopIndex].isPlaying)
            {
                continue;
            }

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }
}
