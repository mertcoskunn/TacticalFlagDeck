using System; 
using UnityEngine;

[Serializable]
public class HelloMessage : NetworkMessage
{
    public HelloMessage(string token, string matchID)
    {
        this.token = token;
        this.type = "HELLO"; 
        this.matchID = matchID;
        this.currentTeam = -1; 
    }
        
   
}
