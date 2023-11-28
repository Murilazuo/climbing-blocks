using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    [Header("Texts")]
    [SerializeField] TMP_InputField createRoomInput;
    [SerializeField] TMP_InputField joinRoomInput;
    [SerializeField] TMP_Text warnigTMP;

    [Header("Panel")]
    [SerializeField] RectTransform panel;
    [SerializeField] GameObject createContent;
    [SerializeField] GameObject lobbyContent;
    [SerializeField] CanvasGroup buttonsCanvasGroup;

    [Header("Panel Anim")]
    [SerializeField] float closedPositionY;
    [SerializeField] float openPositionY, timeToMove;
    [SerializeField] LeanTweenType ease;

    List<RoomInfo> roomList = new List<RoomInfo>();

    static CreateAndJoinRooms Instance;
    private void Awake()
    {
        Instance = this;
        LoadingPanel.Instance.Close();
    }

    public void CreateRoom()
    {
        LoadingPanel.Instance.Open("Creating Room...");


        string roomName = createRoomInput.text;

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 11;

        if (roomName == "")
        {
            LoadingPanel.Instance.Close();
            ShowWarning("Room name invalid");
        }
        else
            PhotonNetwork.CreateRoom(roomName, roomOptions, null);
    }
    public void JoinRoom()
    {
        LoadingPanel.Instance.Open("Joining Room...");

        string roomName = joinRoomInput.text;
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
        if (returnCode == 32765)
            ShowWarning("Room is full");

        LoadingPanel.Instance.Close();
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        if (returnCode == 32766)
            ShowWarning("Room already exist");

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
    }
    public void OpenPanelJoin()
    {
        OpenPanel();
        createContent.SetActive(false);
        lobbyContent.SetActive(true);
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

}
