using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingPanel : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] TMP_Text tmpText;

    public static LoadingPanel Instance;
    private void Awake()
    {
        if (Instance)
            Destroy(gameObject);
        else
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
    }
    public void Open(string message)
    {
        tmpText.text = message;
        panel.SetActive(true);
    }
    public void Close()
    {
        if(panel)
            panel.SetActive(false);
    }

}
