using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class NetworkManagerLobby : NetworkManager
{
    [Scene, SerializeField] string menuScene;

    [Header("Room")]
    [SerializeField] NetworkRoomPlayerLobby roomPlayerLobbyPrefab;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    List<GameObject> SpawnPrefab { get => Resources.LoadAll<GameObject>("SpanwnablePrefabs").ToList(); }
    bool IsInMenuScene { get => SceneManager.GetActiveScene().name != menuScene; }
    public override void OnStartServer()
    {
        spawnPrefabs = SpawnPrefab;
    }
    public override void OnStartClient()
    {
        foreach(var prefab in SpawnPrefab)
        {
            NetworkClient.RegisterPrefab(prefab);
        }
    }
    public override void OnClientConnect()
    {
        base.OnClientConnect();
        OnClientConnected?.Invoke();
    }
    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if(numPlayers > maxConnections || IsInMenuScene)
        {
            conn.Disconnect();
            return;
        }
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (IsInMenuScene)
        {
            NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerLobbyPrefab);

            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }
}
