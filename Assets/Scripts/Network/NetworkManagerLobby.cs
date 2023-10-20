using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class NetworkManagerLobby : NetworkManager
{
    public static NetworkManagerLobby Instance;
    public int playerId;

    
    public override void Awake()
    {
        base.Awake();
        Instance = this;
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        playerId = numPlayers;

        if (numPlayers == 1)
            LeanTween.delayedCall(.2f, () => MatchManager.Instance.StartMatch());
    }
    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void SetNetWorkAdress(string adress) => networkAddress = adress;    
}
