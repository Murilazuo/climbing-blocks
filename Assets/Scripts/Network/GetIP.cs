using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;

public class GetIP : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    private void Start()
    {
        text.text = GetLocalIPv4();
    }
    public string GetLocalIPv4()
    {
        return Dns.GetHostEntry(Dns.GetHostName())
        .AddressList.First(
        f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        .ToString();
    }
}
