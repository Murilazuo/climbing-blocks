using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectToServerButton : MonoBehaviour
{
    public void Connect()
    {
        ConnectionManager.instance.ConnectToServer();
    }
}
