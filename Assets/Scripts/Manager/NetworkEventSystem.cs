using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkEventSystem
{
    public const int START_MATCH_EVENT = 1;
    public const int PIECE_WIN_GAME_EVENT = 2;
    public const int PLATFORM_WIN_GAME_EVENT = 3;
    public const int PIECE_STOP_EVENT = 10;

    public static void CallEvent(byte id, object[] data)
    {
        PhotonNetwork.RaiseEvent(id, data, RaiseEventOptions.Default, SendOptions.SendUnreliable);
    }
    public static void CallEvent(byte id)
    {
        PhotonNetwork.RaiseEvent(id, new object[] { }, RaiseEventOptions.Default, SendOptions.SendUnreliable);
    }

}
