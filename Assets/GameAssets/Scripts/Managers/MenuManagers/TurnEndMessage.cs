using UnityEngine;

public class TurnEndMessage : NetworkMessage
{
    
    public TurnEndMessage(string token, string matchID, int currentTeam)
    {
        this.token = token;
        this.type = "TURN_END"; 
        this.matchID = matchID;
        this.currentTeam = currentTeam;
        
    }
}
