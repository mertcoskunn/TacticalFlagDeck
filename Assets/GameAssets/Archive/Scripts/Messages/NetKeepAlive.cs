using System.Collections;
using UnityEngine;
 
using Unity.Networking.Transport; 

public class NetKeepAlive:NetMessage
{
    public NetKeepAlive()
    {
        Code = OpCode.KEEP_ALIVE;
    }

    public NetKeepAlive(Unity.Collections.DataStreamReader reader)
    {
        Code = OpCode.KEEP_ALIVE;
        Deserialize(reader);
    }

    public override void Serialize(ref Unity.Collections.DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code); 

    }

    public override void Deserialize(Unity.Collections.DataStreamReader reader)
    {
        
    }

    public override void ReceivedOnClient(){
        NetUtility.C_KEEP_ALIVE?.Invoke(this);
    }


    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_KEEP_ALIVE?.Invoke(this, cnn);
    }


}