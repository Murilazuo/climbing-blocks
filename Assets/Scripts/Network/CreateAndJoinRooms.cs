using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using ExitGames.Client.Photon;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    [Header("Texts")]
    [SerializeField] TMP_InputField createRoomInput;
    [SerializeField] TMP_InputField joinRoomInput;
    [SerializeField] TMP_Text warnigTMP;
    [SerializeField] Toggle isPublic;

    [Header("Panel")]
    [SerializeField] RectTransform panel;
    [SerializeField] GameObject createContent;
    [SerializeField] GameObject lobbyContent;
    [SerializeField] CanvasGroup buttonsCanvasGroup;
    [SerializeField] GameObject roomListPanel;
    [SerializeField] SetPreferredHeight setPreferredHeight;

    [Header("Panel Anim")]
    [SerializeField] float closedPositionY;
    [SerializeField] float openPositionY, timeToMove;
    [SerializeField] LeanTweenType ease;

    [Header("Room List")]
    [SerializeField] GameObject roomListItem;
    [SerializeField] Transform roomListContent;

    List<RoomListItem> roomItemList = new List<RoomListItem>() ;
    private TypedLobby sqlLobby = new TypedLobby("customSqlLobby", LobbyType.SqlLobby);

    public const string GAME_TYPE_KEY = "C0";

    public static CreateAndJoinRooms Instance;
    private void Awake()
    {
        Instance = this;
        LoadingPanel.Instance.Close();
        
    }

    public void CreateRoom()
    {
        LoadingPanel.Instance.Open("Creating Room...");


string roomName = createRoomInput.text;

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 11,
            IsVisible = isPublic.isOn,
            CustomRoomProperties = new Hashtable { { GAME_TYPE_KEY, "Normal" }  },
            CustomRoomPropertiesForLobby = new string[] { GAME_TYPE_KEY }
        };

        if (roomName == "")
        {
            LoadingPanel.Instance.Close();
            ShowWarning("Room name invalid");
        }
        else
            PhotonNetwork.CreateRoom(roomName, roomOptions, sqlLobby);
    }
    public void JoinRoom()
    {
        JoinRoom(joinRoomInput.text);
    }
    public void JoinRoom(string roomName)
    {
        LoadingPanel.Instance.Open("Joining Room...");

        if (roomName == "")
        {
            ShowWarning("Room name invalid");
            LoadingPanel.Instance.Close();
        }
        else
            PhotonNetwork.JoinRoom(roomName);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (returnCode == 32758)
            ShowWarning("Room doesn't exist");
        else if (returnCode == 32765)
            ShowWarning("Room is full");
        else
            ShowWarning(message);

        LoadingPanel.Instance.Close();
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        if (returnCode == 32766)
            ShowWarning("Room already exist");
        else
            ShowWarning(message);

        LoadingPanel.Instance.Close();
    }
    void ShowWarning(string warningText)
    {
        LeanTween.cancel(warnigTMP.gameObject);
        warnigTMP.text = warningText;
        warnigTMP.gameObject.SetActive(true);

        LeanTween.delayedCall(warnigTMP.gameObject,5,() =>
        {
            if (warnigTMP.gameObject.activeSelf)
                warnigTMP.gameObject.SetActive(false);
        });
    }
    public override void OnDisable()
    {
        base.OnDisable();

        LeanTween.cancel(warnigTMP.gameObject);
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Main");
    }
    private void OnDestroy()
    {
        LoadingPanel.Instance.Close();
    }

    public void OpenPanelCreate()
    {
        OpenPanel();
        createContent.SetActive(true);
        lobbyContent.SetActive(false);
        roomListPanel.SetActive(false);
    }
    public void OpenPanelJoin()
    {
        OpenPanel();
        createContent.SetActive(false);
        lobbyContent.SetActive(true);
        roomListPanel.SetActive(false);
    }
    public void ClosedPanel()
    {
        buttonsCanvasGroup.interactable = true;
        LeanTween.value(panel.gameObject,MoveRectTransformY,  openPositionY, closedPositionY, timeToMove).setEase(ease);

    }
    void OpenPanel()
    {
        warnigTMP.gameObject.SetActive(false);
        buttonsCanvasGroup.interactable = false;
        LeanTween.value(panel.gameObject,MoveRectTransformY, closedPositionY, openPositionY,timeToMove).setEase(ease);
    }
    
    void MoveRectTransformY(float pos)
    {
        Vector2 rect = Instance.panel.anchoredPosition;
        Instance.panel.anchoredPosition = new Vector2(rect.x, pos);
    }

    public void OpenRoomList()
    {
        OpenPanel();
        createContent.SetActive(false);
        lobbyContent.SetActive(false);
        roomListPanel.SetActive(true);
        PhotonNetwork.GetCustomRoomList(sqlLobby, "C0 = 'Normal'");
    }
    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo roomInfo in roomList)
        {
            if (roomInfo.RemovedFromList)
            {
                int index = roomItemList.FindIndex(x => x.name == roomInfo.Name);
                Destroy(roomItemList[index].gameObject);
            }
            else if (roomItemList.Exists(x => x.name == roomInfo.Name))
            {
                roomItemList.Find(x => x.name == roomInfo.Name).UpdateContent(roomInfo);
            }
            else
            {
                if (roomInfo.IsVisible)
                {
                    RoomListItem item = Instantiate(roomListItem, roomListContent).GetComponent<RoomListItem>();
                    item.UpdateContent(roomInfo);
                    roomItemList.Add(item);
                }
            }
        }

        setPreferredHeight.UpdateHeight();
    }

    public override void OnJoinedLobby()
    {
        roomItemList.Clear();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateCachedRoomList(roomList);
    }

    public override void OnLeftLobby()
    {
        roomItemList.Clear();
    }
    public override void OnErrorInfo(ErrorInfo errorInfo)
    {
        Debug.Log(errorInfo.Info);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        roomItemList.Clear();
    }
}
