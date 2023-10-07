using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputName : MonoBehaviour
{
    [SerializeField] Button submitButton;
    [SerializeField] TMP_InputField playerNameDisplay;
    string playerName;
    const string playerNameKey = "PlayerName";
    
    private void Awake()
    {
        if (PlayerPrefs.HasKey(playerNameKey))
        {
            playerNameDisplay.text = PlayerPrefs.GetString(playerNameKey);
        }
        else
        {
            submitButton.interactable = false;
        }
    }
    public void SetButtonActive(string value)
    {
        playerName = value;
        submitButton.interactable = !string.IsNullOrEmpty(value);
    }
    public void SubmitName()
    {
        PlayerPrefs.SetString(playerNameKey, playerName);
    }
}
