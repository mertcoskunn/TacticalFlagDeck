using Unity.Networking.Transport; 
using UnityEngine;

public class NetWelcome : NetMessage
{
    public int AssignedTeam{set; get;}


    public NetWelcome()
    {
        Code = OpCode.WELCOME; 

    }   

    public NetWelcome(Unity.Collections.DataStreamReader reader)
    {
        Code = OpCode.WELCOME;
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
        NetUtility.C_WELCOME?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_WELCOME?.Invoke(this, cnn);
    }



}
