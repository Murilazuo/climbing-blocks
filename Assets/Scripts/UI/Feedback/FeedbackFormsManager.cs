using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FeedbackFormsManager : MonoBehaviour
{
    [SerializeField] TMP_InputField feedback1;
    string URL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSc3UCvxRAGc48kA8aQctauAomQC2yiPIm032zW7hkRK7CiK4Q/formResponse";
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Send()
    {
        if (feedback1.text == "")
            return;

        StartCoroutine(Post(feedback1.text));
    }

    IEnumerator Post(string s1)
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.883711715", s1);

        UnityWebRequest www = UnityWebRequest.Post(URL, form);

        yield return www.SendWebRequest();

    }
}
