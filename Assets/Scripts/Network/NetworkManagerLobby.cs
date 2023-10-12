using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

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

        if (numPlayers > 1)
            MatchManager.Instance.StartMatch();
    }
    public void SetNetWorkAdress(string adress) => networkAddress = adress;    
}
