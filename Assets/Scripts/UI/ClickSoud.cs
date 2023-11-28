using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClickSoud : MonoBehaviour
{

    private void Awake()
    {
        foreach(var button in FindObjectsOfType<Button>())
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
        print("Play sound");
        SoundManager.Instance.PlaySound(SoundType.UIClick);
    }
}
