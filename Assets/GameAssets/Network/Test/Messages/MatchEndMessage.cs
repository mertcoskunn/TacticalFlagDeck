using UnityEngine;

public class MatchEndMessage : NetworkMessage
{
    public MatchEndMessage(string token, string matchID, int currentTeam)
    {
        this.token = token;
        this.type = "MATCH_END"; 
        this.matchID = matchID;
        this.currentTeam = currentTeam; 
    }
}
