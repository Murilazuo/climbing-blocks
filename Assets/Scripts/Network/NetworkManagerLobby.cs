using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System.Net;
using System.Linq;
using Mirror.Discovery;

public class NetworkManagerLobby : NetworkManager
{
    public static NetworkManagerLobby Instance;
    public int playerId;
    [SerializeField] NetworkDiscovery networkDiscovery;
    
    public override void Awake()
    {
        if(Instance)
        {
            DestroyImmediate(gameObject);
            return;
        }

        networkDiscovery.BroadcastAddress = Dns.GetHostEntry(Dns.GetHostName())
        .AddressList
        .First(f => f.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        .ToString();

        base.Awake();
        Instance = this;
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        playerId = numPlayers;

        if (numPlayers == 2)
            LeanTween.delayedCall(.2f, () => MatchManager.Instance.StartMatch());
    }
    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void SetNetWorkAdress(string adress) => networkAddress = adress;    
}
