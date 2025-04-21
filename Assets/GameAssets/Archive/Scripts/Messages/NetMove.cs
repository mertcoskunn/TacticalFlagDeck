using Unity.Networking.Transport; 
using UnityEngine;

public class NetMove : NetMessage
{
    public int currentTeam{set; get;} 
    public Vector2 initPoisiton{set; get;}
    public Vector2 targetPoisiton{set; get;}

    public NetMove()
    {
        Code = OpCode.MAKE_MOVE; 

    }   

    public NetMove(Unity.Collections.DataStreamReader reader)
    {
        Code = OpCode.MAKE_MOVE;
        Deserialize(reader);
    } 

    public override void Serialize(ref Unity.Collections.DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(currentTeam);
        writer.WriteInt((int)initPoisiton.x);
        writer.WriteInt((int)initPoisiton.y);
        writer.WriteInt((int)targetPoisiton.x);
        writer.WriteInt((int)targetPoisiton.y); 
    }


    public override void Deserialize(Unity.Collections.DataStreamReader reader)
    {
        currentTeam = reader.ReadInt(); 
        int initPosx = reader.ReadInt();
        int initPosy = reader.ReadInt();
        int targetPosx = reader.ReadInt();
        int targetPosy = reader.ReadInt();
        initPoisiton = new Vector2((float)initPosx, (float)initPosy);
        targetPoisiton = new Vector2((float)targetPosx, (float)targetPosy);
           
    }


    public override void ReceivedOnClient()
    {
        NetUtility.C_MAKE_MOVE?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_MAKE_MOVE?.Invoke(this, cnn);
    }



}
