using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType { Jump, Punch, UIClick}
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

    }
    [SerializeField] List<SoundData> soundData;
    [SerializeField] PhotonView view;
    [SerializeField] AudioSource source;

    public static SoundManager Instance;
    private void Awake()
    {
        Instance = this;
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

        if (view && !soundData[soundIndex].clientOnly)
            view.RPC(nameof(Play), RpcTarget.All, soundIndex);
        else
            Play(soundIndex);
    }

    [PunRPC]
    void Play(int clipId)
    {
        source.volume = soundData[clipId].volume;
        source.PlayOneShot(soundData[clipId].clip);
    }
}
