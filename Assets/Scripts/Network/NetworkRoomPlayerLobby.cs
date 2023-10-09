using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class NetworkRoomPlayerLobby : NetworkBehaviour 
{
    private void Start()
    {
        DontDestroyOnLoad(this);
        SceneManager.LoadScene("Main");
    }
    public override void OnStartLocalPlayer()
    {
        
    }

}
