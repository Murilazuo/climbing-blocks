using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum SoundType { Jump, Punch, UIClick, Bubbles, Win, Lose }
public class SoundManager : MonoBehaviour
{
    [System.Serializable]
    public struct SoundData
    {
        [HideInInspector]public string name;
        public SoundType sound;
        public AudioClip clip;
        public bool clientOnly;
        [Range(0,1)]public float volume;
        public bool loop;
        public AudioSource source;

        public void Init()
        {
            source.volume = volume;
            source.clip = clip;
        }
        public void Play()
        {
            source.Play();
        }
        public void Stop()
        {
            source.Stop();
        }

    }
    [SerializeField] List<SoundData> soundData;
    [SerializeField] PhotonView view;
    [SerializeField] AudioSource source;

    public static SoundManager Instance;
    private void Awake()
    {
        Instance = this;
        foreach (var sound in soundData)
        {
            if(sound.loop)
                sound.Init();
        }
    }

    private void OnValidate()
    {
        for (int i = 0; i < soundData.Count; i++)
        {
            SoundData sound = soundData[i];
            sound.name = sound.sound.ToString();
            soundData[i] = sound;
        }
    }

    public void PlaySound(SoundType sound)
    {
        int soundIndex = soundData.IndexOf(soundData.Find(x => x.sound == sound));

        if (soundData[soundIndex].clip == null)
            return;

        if (view && !soundData[soundIndex].clientOnly)
            view.RPC(nameof(Play), RpcTarget.All, soundIndex);
        else
            Play(soundIndex);
    }

    [PunRPC]
    void Play(int clipId)
    {
        SoundData sound = soundData[clipId];
        if (sound.loop)
        {
            if(sound.source.isPlaying)
                sound.Play();
        }
        else
        {
            source.volume = sound.volume;
            source.PlayOneShot(sound.clip);
        }
    }

    public void Stop(SoundType sound)
    {
        int soundIndex = soundData.IndexOf(soundData.Find(x => x.sound == sound));

        soundData[soundIndex].Stop();
    }
}
