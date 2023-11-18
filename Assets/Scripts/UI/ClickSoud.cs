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
            button.onClick.AddListener(() => Play());
        }
        foreach (var inputField in FindObjectsOfType<TMP_InputField>())
        {
            inputField.onSelect.AddListener((s) => Play());
            inputField.onSubmit.AddListener((s) => Play());
        }
    }

    void Play()
    {
        print("Play sound");
        SoundManager.Instance.PlaySound(SoundType.UIClick);
    }
}
