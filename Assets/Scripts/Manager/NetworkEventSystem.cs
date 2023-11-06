using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkEventSystem
{
    public const byte START_MATCH_EVENT = 1;
    public const byte PIECE_WIN_GAME_EVENT = 2;
    public const byte PLATFORM_WIN_GAME_EVENT = 3;
    public const byte START_COUNTER_EVENT = 4;
    public const byte SELECT_TEAM_EVENT = 4;
    public const byte PIECE_STOP_EVENT = 10;
    public const byte PIECE_DESTROY_EVENT = 11;

    public static void CallEvent(byte id, object[] data)
    {
        PhotonNetwork.RaiseEvent(id, data, RaiseEventOptions.Default, SendOptions.SendUnreliable);
    }
    public static void CallEvent(byte id)
    {
        PhotonNetwork.RaiseEvent(id, new object[] { }, RaiseEventOptions.Default, SendOptions.SendUnreliable);
    }

}
