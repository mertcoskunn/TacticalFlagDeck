using System; 
using UnityEngine;

[Serializable]
public class CreateCharMessage : NetworkMessage
{
    
    public int cardIndex; 
    public int posX;
    public int posY; 
    
    public CreateCharMessage(string token, string matchID, int currentTeam, int cardIndex, int posX, int posY)
    {
        this.token = token;
        this.type = "CHAR_CREATE"; 
        this.matchID = matchID;
        this.currentTeam = currentTeam;
        this.cardIndex = cardIndex; 
        this.posX = posX; 
        this.posY = posY; 
    }
}
