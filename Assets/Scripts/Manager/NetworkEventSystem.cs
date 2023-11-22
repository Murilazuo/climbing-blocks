using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkEventSystem
{
    public const byte START_MATCH_EVENT = 1;
    public const byte PIECE_WIN_GAME_EVENT = 2;
    
    
    public const byte PIECE_COLIDE_WITH_PLATFORM_EVENT = 3;
    public const byte PLATFORM_REACH_TOP_EVENT = 4;
    public const byte PIECE_REACH_TOP_EVENT = 5;
    public const byte PLATFORM_DROWNED_EVENT = 6;
    
    public const byte PIECE_STOP_EVENT = 10;

    public const byte UPDATE_PLAYERS_SELECT_EVENT = 20;

    public const byte START_COUNTER_EVENT = 99;
    public const byte SELECT_TEAM_EVENT = 98;

    public static bool IsEndGame(byte value)
    {
        return PieceWin(value) || PlatfomrWin(value);
    }
    public static bool PieceWin(byte value)
    {
        return value == PIECE_COLIDE_WITH_PLATFORM_EVENT ||
               value == PLATFORM_DROWNED_EVENT;
    }
    public static bool PlatfomrWin(byte value)
    {
        return value == PLATFORM_REACH_TOP_EVENT ||
               value == PIECE_REACH_TOP_EVENT;
    }

    public static void CallEvent(byte id, object[] data)
    {
        PhotonNetwork.RaiseEvent(id, data, RaiseEventOptions.Default, SendOptions.SendUnreliable);
    }
    public static void CallEvent(byte id)
    {
        PhotonNetwork.RaiseEvent(id, new object[] { }, RaiseEventOptions.Default, SendOptions.SendUnreliable);
    }

}
