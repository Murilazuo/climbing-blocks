using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] TMP_Text roomName, playersCount;
    [SerializeField] Button button;

    public void UpdateContent(RoomInfo info)
    {
        if (info.PlayerCount == 0)
            Destroy(gameObject);
        else
        {
            button.interactable = info.PlayerCount != info.MaxPlayers;
            roomName.text = info.Name;
            name = info.Name;
            playersCount.text = $"{info.PlayerCount}/{info.MaxPlayers}";
        }
    }
    public void JoinRoom()
    {
        CreateAndJoinRooms.Instance.JoinRoom(roomName.text);
    }
}
