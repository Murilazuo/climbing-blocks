using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
public class ConnectToServer : MonoBehaviourPunCallbacks
{
    static ConnectToServer instance;
    private void Awake()
    {
        if (instance)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);    
    }
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();       
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene(1);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene(0);
        PhotonNetwork.ConnectUsingSettings();
    }
}
