using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("#BGM")]
    public AudioClip[] bgmClips;
    public float bgmVolume;
    public int bgmClipcount;
    AudioSource[] bgmPlayers;
    int bgmIndex;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    int channelIndex;

    public enum Bgm {Wait =0, Battle}
    public enum Sfx {Chopping, Jump, Mining, Run, Shovel, Ui, Walk}

    void Awake()
    {
        instance = this;
        Init();
    }

    void Init()
    {
        //// 배경음 초기화
        GameObject bgmobject = new GameObject("BgmPlayer");
        bgmobject.transform.parent = transform;
        bgmPlayers = new AudioSource[bgmClipcount];

        for (int index = 0; index < bgmPlayers.Length; index++)
        {
            bgmPlayers[index] = bgmobject.AddComponent<AudioSource>();
            bgmPlayers[index].playOnAwake = false;
            bgmPlayers[index].loop = true;
            bgmPlayers[index].volume = bgmVolume;
        }

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

    //배경음악 호출
    public void PlayerBgm(Bgm bgm)
    {
        if (bgm == 0)
        {
            bgmPlayers[0].Play();
        }
        else
        {
            bgmPlayers[1].Play();
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
