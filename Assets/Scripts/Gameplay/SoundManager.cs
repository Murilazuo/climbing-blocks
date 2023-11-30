using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SoundType { Jump, Punch, UIClick, Bubbles, Win, Lose }
public class SoundManager : MonoBehaviour
{
    [System.Serializable]
    public struct SoundData
    {
        [HideInInspector]public string name;
        public SoundType sound;
        public AudioClip clip;
        [Range(0,1)]public float volume;
        public bool loop;
        public AudioSource source;
        [Range(-3, 3)] public float maxPitch;
        [Range(-3, 3)] public float minPitch;

        public void Init()
        {
            source.volume = volume;
            source.clip = clip;
            source.loop = loop;    
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
    [SerializeField] AudioSource source;

    public static SoundManager Instance;
    private void Awake()
    {
        if (Instance)
            Destroy(gameObject);
        else
        {
            DontDestroyOnLoad(gameObject);

            Instance = this;
            foreach (var sound in soundData)
            {
                if (sound.loop)
                    sound.Init();
            }
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
        print("Play sound " + sound);

        if (soundData[soundIndex].clip == null)
            return;

        Play(soundIndex);
    }
    void Play(int clipId)
    {
        SoundData sound = soundData[clipId];

        sound.source.pitch = Random.Range(sound.minPitch, sound.maxPitch);

        if (sound.loop)
        {
            if(!sound.source.isPlaying)
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

    public void OnEnable()
    {
        SceneManager.sceneLoaded += OnLoadSceane;
    }
    public void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLoadSceane;
    }

    private void OnLoadSceane(Scene arg0, LoadSceneMode arg1)
    {
        foreach (var button in FindObjectsOfType<Button>())
        {
            button.onClick.AddListener(() => PlayClickSound());
        }
        foreach (var inputField in FindObjectsOfType<TMP_InputField>())
        {
            inputField.onSelect.AddListener((s) => PlayClickSound());
            inputField.onSubmit.AddListener((s) => PlayClickSound());
        }
    }


    void PlayClickSound()
    {
        Instance.PlaySound(SoundType.UIClick);
    }
}
