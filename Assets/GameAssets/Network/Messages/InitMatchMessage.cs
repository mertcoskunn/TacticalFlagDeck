using System; 
using UnityEngine;

[Serializable]
public class InitMatchMessage : NetworkMessage
{
    public InitMatchMessage(string token, string matchID)
    {
        this.token = token;
        this.type = "INIT_MATCH"; 
        this.matchID = matchID;
        this.currentTeam = -1; 
    }
}
