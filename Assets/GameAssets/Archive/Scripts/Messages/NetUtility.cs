using System;
using Unity.Networking.Transport; 
using UnityEngine; 

public enum OpCode
{
    KEEP_ALIVE = 1,
    WELCOME = 2, 
    START_GAME = 3,
    CHAR_CREATED = 4, 
    MAKE_MOVE = 5,
    ATTACK = 6,
    TURN_END = 7,
    MATCH_END = 8
}


public static class NetUtility 
{


    public static void OnData(Unity.Collections.DataStreamReader stream, NetworkConnection cnn, Server server = null)
    {
        NetMessage msg = null; 
        var opCode = (OpCode)stream.ReadByte();
        switch(opCode)
        {
            case OpCode.KEEP_ALIVE: msg = new NetKeepAlive(stream);break; 
            case OpCode.WELCOME: msg = new NetWelcome(stream);break;
            case OpCode.START_GAME: msg = new NetStartGame(stream);break;
            case OpCode.CHAR_CREATED: msg = new NetCharCreated(stream);break;  
            case OpCode.MAKE_MOVE: msg = new NetMove(stream);break;
            case OpCode.ATTACK: msg = new NetAttack(stream);break;
            case OpCode.TURN_END: msg = new NetTurnEnd(stream);break; 
            case OpCode.MATCH_END: msg = new NetMatchEnd(stream);break;  
        
            default:
                Debug.Log("Message received had no OpCOde");
                break; 
        }

        if(server != null)
        {
            msg.ReceivedOnServer(cnn);   
        }
        else
        {
            msg.ReceivedOnClient();
        }
    }
    public static Action<NetMessage> C_KEEP_ALIVE;
    public static Action<NetMessage> C_WELCOME;
    public static Action<NetMessage> C_START_GAME;
    public static Action<NetMessage> C_CHAR_CREATED; 
    public static Action<NetMessage> C_MAKE_MOVE;
    public static Action<NetMessage> C_ATTACK;
    public static Action<NetMessage> C_TURN_END;
    public static Action<NetMessage> C_MATCH_END;
    public static Action<NetMessage> C_REMATCH;
    public static Action<NetMessage, NetworkConnection> S_KEEP_ALIVE;
    public static Action<NetMessage, NetworkConnection> S_WELCOME;
    public static Action<NetMessage, NetworkConnection> S_CHAR_CREATED; 
    public static Action<NetMessage, NetworkConnection> S_START_GAME;
    public static Action<NetMessage, NetworkConnection> S_MAKE_MOVE;
    public static Action<NetMessage, NetworkConnection> S_ATTACK;
    public static Action<NetMessage, NetworkConnection> S_TURN_END;
    public static Action<NetMessage, NetworkConnection> S_MATCH_END;
    public static Action<NetMessage, NetworkConnection> S_REMATCH;


    
}
