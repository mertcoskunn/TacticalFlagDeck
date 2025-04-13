using Unity.Networking.Transport; 
using UnityEngine;

public class NetMatchEnd : NetMessage
{
    public int currentTeam{set; get;}


    public NetMatchEnd()
    {
        Code = OpCode.MATCH_END; 

    }   

    public NetMatchEnd(Unity.Collections.DataStreamReader reader)
    {
        Code = OpCode.MATCH_END;
        Deserialize(reader);
    } 

    public override void Serialize(ref Unity.Collections.DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(currentTeam);
    }


    public override void Deserialize(Unity.Collections.DataStreamReader reader)
    {
        currentTeam = reader.ReadInt();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_MATCH_END?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_MATCH_END?.Invoke(this, cnn);
    }
    
}
