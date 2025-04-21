using UnityEngine;

public class AttackMessage : NetworkMessage
{
    public int initPosX;
    public int initPosY;
    public int targetPosX;
    public int targetPosY; 
    
    public AttackMessage(string token, string matchID, int currentTeam, int initPosX, int initPosY, int targetPosX, int targetPosY)
    {
        this.token = token;
        this.type = "ATTACK"; 
        this.matchID = matchID;
        this.currentTeam = currentTeam;
        this.initPosX = initPosX; 
        this.initPosY = initPosY;
        this.targetPosX = targetPosX; 
        this.targetPosY = targetPosY; 
    }
}
