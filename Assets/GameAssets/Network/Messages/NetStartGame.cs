using Unity.Networking.Transport; 
using UnityEngine;

public class NetStartGame : NetMessage
{
    public int AssignedTeam{set; get;}


    public NetStartGame()
    {
        Code = OpCode.START_GAME; 

    }   

    public NetStartGame(Unity.Collections.DataStreamReader reader)
    {
        Code = OpCode.START_GAME;
        Deserialize(reader);
    } 

    public override void Serialize(ref Unity.Collections.DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(AssignedTeam);
    }


    public override void Deserialize(Unity.Collections.DataStreamReader reader)
    {
        AssignedTeam = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_START_GAME?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_START_GAME?.Invoke(this, cnn);
    }



}
