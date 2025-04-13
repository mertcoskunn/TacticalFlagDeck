using Unity.Networking.Transport; 
using UnityEngine;

public class NetCharCreated : NetMessage
{
    public int currentTeam{set; get;} 
    public int cardIndex{set; get;}
    public Vector2 gridPosition{set; get;}

    public NetCharCreated()
    {
        Code = OpCode.CHAR_CREATED; 

    }   

    public NetCharCreated(Unity.Collections.DataStreamReader reader)
    {
        Code = OpCode.CHAR_CREATED;
        Deserialize(reader);
    } 

    public override void Serialize(ref Unity.Collections.DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(currentTeam);
        writer.WriteInt(cardIndex);
        writer.WriteInt((int)gridPosition.x);
        writer.WriteInt((int)gridPosition.y); 
    }


    public override void Deserialize(Unity.Collections.DataStreamReader reader)
    {
        currentTeam = reader.ReadInt(); 
        cardIndex = reader.ReadInt();
        int posx = reader.ReadInt();
        int posy = reader.ReadInt();
        gridPosition = new Vector2((float)posx, (float)posy);
        //gridPosition.x = reader.ReadInt();
        //gridPosition.y = reader.ReadInt();     
    }


    public override void ReceivedOnClient()
    {
        NetUtility.C_CHAR_CREATED?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection cnn)
    {
        NetUtility.S_CHAR_CREATED?.Invoke(this, cnn);
    }



}
