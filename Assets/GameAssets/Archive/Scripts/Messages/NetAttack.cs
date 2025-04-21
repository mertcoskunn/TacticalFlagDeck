using Unity.Networking.Transport; 
using UnityEngine;

public class NetAttack : NetMessage
{
    public int currentTeam{set; get;} 
    public Vector2 initPoisiton{set; get;}
    public Vector2 targetPoisiton{set; get;}

    public NetAttack()
    {
        Code = OpCode.ATTACK; 

    }   

    public NetAttack(Unity.Collections.DataStreamReader reader)
    {
        Code = OpCode.ATTACK;
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
        NetUtility.C_ATTACK?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_ATTACK?.Invoke(this, cnn);
    }



}
