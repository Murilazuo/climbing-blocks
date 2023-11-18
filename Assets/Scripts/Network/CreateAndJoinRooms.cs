using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        this.roomList = roomList;
        print("Room list Updated");
    }
    public void CreateRoom()
    {
        string roomName = createRoomInput.text;
        if (roomName == "")
            ShowWarning("Room name invalid");
        else if (RoomExist(roomName))
            ShowWarning("Room doesn't exist");
        else
            PhotonNetwork.CreateRoom(createRoomInput.text);
    }
    public void JoinRoom()
    {
        string roomName = createRoomInput.text;
        if (!RoomExist(roomName))
            PhotonNetwork.JoinRoom(joinRoomInput.text);
        else if (roomName == "")
            ShowWarning("Room name invalid");
        else
            ShowWarning("Room doesn't exist");
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
    public bool RoomExist(string roomName)
    {
        return roomList.Exists(x => x.Name == roomName);
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(2);
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
        buttonsCanvasGroup.interactable = false;
        LeanTween.value(panel.gameObject,MoveRectTransformY, closedPositionY, openPositionY,timeToMove).setEase(ease);
    }
    
    void MoveRectTransformY(float pos)
    {
        Vector2 rect = Instance.panel.anchoredPosition;
        Instance.panel.anchoredPosition = new Vector2(rect.x, pos);
    }

}
