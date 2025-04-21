using Unity.Networking.Transport; 
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    

    public DeckManager deckManager; 
    public GridManager gridManager;
    public HandManager handManager;
    public MenuManager menuManager; 
    private int currentTeam = -1;
    private int manaPerTurn = 3; 
    private int currentMana = 3;  


    private void Awake()
    {
        //RegisterToEvent(); 
    }
    
    private void RegisterToEvent()
    {
        NetUtility.C_WELCOME += OnSetCurrentTeamClient; 
        NetUtility.C_START_GAME += OnGameStartClient;
        NetUtility.C_CHAR_CREATED += OnEnemyCreatedNewCharClient;
        NetUtility.C_MAKE_MOVE += OnMakeMoveClient;
        NetUtility.C_ATTACK += OnAttackClient; 
        NetUtility.C_TURN_END += OnTurnEndClient; 
        NetUtility.C_MATCH_END += OnMatchEndClient; 
        

        NetUtility.S_CHAR_CREATED += OnEnemyCreatedNewCharServer;
        NetUtility.S_MAKE_MOVE += OnMakeMoveServer;
        NetUtility.S_ATTACK += OnAttackServer;
        NetUtility.S_TURN_END += OnTurnEndServer; 
        NetUtility.S_MATCH_END += OnMatchEndServer;   
        
        gridManager.OnAddObjectToGrid += OnObjectAddedToGrid;
        gridManager.OnMakeMove += OnCharMoved; 
        gridManager.OnAttack  += OnCharAttack;
        gridManager.OnMatchEnd += OnMatchEnd;
    }
    
    
    private void OnGameStartClient(NetMessage msg)
    {
        SetMana(manaPerTurn); 
        gridManager.Init(currentTeam);
        gridManager.SetLocked(true);
        
        if(currentTeam == 0)
        {
            deckManager.Init(5);            
            handManager.SetLockedCardsInHand(false);
            gridManager.SetLocked(false);
            menuManager.UpdateTurnText("Your Turn");
        }
        else{
            deckManager.Init(4);
            handManager.SetLockedCardsInHand(true);
            menuManager.UpdateTurnText("Enemy Turn"); 
        }
           
    }
    private void OnSetCurrentTeamClient(NetMessage msg)
    {
        NetWelcome nw = msg as NetWelcome;
        currentTeam = nw.AssignedTeam;
    }
    private void OnEnemyCreatedNewCharClient(NetMessage msg)
    {
        NetCharCreated nw = msg as NetCharCreated;
        if(nw.currentTeam != currentTeam)
        {
            gridManager.AddObjectToGrid(nw.cardIndex, nw.gridPosition, nw.currentTeam);
        }
    }
    private void OnMakeMoveClient(NetMessage msg)
     {
        NetMove nw = msg as NetMove;
        if(nw.currentTeam != currentTeam)
        {
            gridManager.MoveObject(nw.initPoisiton, nw.targetPoisiton);
        }
     }
    private void OnAttackClient(NetMessage msg)
     {
        NetAttack nw = msg as NetAttack;

        if(nw.currentTeam != currentTeam)
        {
            gridManager.Attack(nw.initPoisiton, nw.targetPoisiton);
        }
     }
    private void OnTurnEndClient(NetMessage msg)
    {
        NetTurnEnd nw = msg as NetTurnEnd;

        if(nw.currentTeam != currentTeam)
        {
            Debug.Log("Ifin içindeyimmmm");
            OnPlayerTurnStart();
        }
    }
    
    private void OnMatchEndClient(NetMessage msg)
    {
        NetMatchEnd nw = msg as NetMatchEnd;
        handManager.SetLockedCardsInHand(true);
        gridManager.SetLocked(true);

        gridManager.Reset();
        handManager.Reset();
        deckManager.Reset();
        
        if(nw.currentTeam != currentTeam)
        {
            
           menuManager.OpenResultMenu("Failed"); 
        }
        else
        {
            menuManager.OpenResultMenu("Victory");
        }
        currentTeam = -1; 
    
    }
    private void OnMakeMoveServer(NetMessage msg, NetworkConnection cnn)
     {
        NetMove nw = msg as NetMove;
        Server.Instance.Brodcast(nw);
     }
    private void OnAttackServer(NetMessage msg, NetworkConnection cnn)
     {
        NetAttack nw = msg as NetAttack;
        Server.Instance.Brodcast(nw);
     }
    private void OnEnemyCreatedNewCharServer(NetMessage msg, NetworkConnection cnn)
    {
        NetCharCreated nw = msg as NetCharCreated;
        Server.Instance.Brodcast(nw);
        
    }
    private void OnTurnEndServer(NetMessage msg, NetworkConnection cnn)
    {
        NetTurnEnd nw = msg as NetTurnEnd;
        Server.Instance.Brodcast(nw);
    }

    private void OnMatchEndServer(NetMessage msg, NetworkConnection cnn)
    {
        NetMatchEnd nw = msg as NetMatchEnd;
        Server.Instance.Brodcast(nw);
    }
    
    
    
    private void OnObjectAddedToGrid(int currentTeam, int cardIndex, Vector2 gridPoisition)
    {
        UpdateMana(-1); 
        Debug.Log("Card Index: " + cardIndex);
        Debug.Log("Card Position: "+ gridPoisition);

        NetCharCreated nw = new NetCharCreated();
        nw.currentTeam = currentTeam;  
        nw.cardIndex = cardIndex;
        nw.gridPosition = gridPoisition;
        Client.Instance.SendToServer(nw); 
    }
    private void OnCharMoved(Vector2 initPos, Vector2 targetPos)
    {
        UpdateMana(-1);
        NetMove nw = new NetMove();
        nw.currentTeam = currentTeam; 
        nw.initPoisiton = initPos;
        nw.targetPoisiton = targetPos;
        Client.Instance.SendToServer(nw);
    }
    private void OnCharAttack(Vector2 initPos, Vector2 targetPos)
    {
        UpdateMana(-1);
        NetAttack nw = new NetAttack();
        nw.currentTeam = currentTeam; 
        nw.initPoisiton = initPos;
        nw.targetPoisiton = targetPos;
        Client.Instance.SendToServer(nw);
    }
    private void OnPlayerTurnEnd(int currentTeam)
    {
        handManager.SetLockedCardsInHand(true);
        gridManager.SetLocked(true); 
        
        menuManager.UpdateTurnText("Enemy Turn");
        
        NetTurnEnd nw = new NetTurnEnd();
        nw.currentTeam = currentTeam; 
        Client.Instance.SendToServer(nw);
    }

    private void OnMatchEnd(int team)
    {
        handManager.SetLockedCardsInHand(true);
        gridManager.SetLocked(true); 

        NetMatchEnd nw = new NetMatchEnd();
        nw.currentTeam = currentTeam; 
        Client.Instance.SendToServer(nw);

    }


    public void OnPlayerTurnStart()
    {
        Debug.Log("OnPlayerStartın içndeyim");

        SetMana(manaPerTurn);
        menuManager.UpdateTurnText("Your Turn"); 
        deckManager.DrawCard(handManager); 
        gridManager.SetLocked(false);
        handManager.SetLockedCardsInHand(false); 
    }

    
    public void UpdateMana(int deltaMana)
    {
        
        currentMana = currentMana + deltaMana; 

        menuManager.UpdateManaText(currentMana, manaPerTurn);

        if(currentMana <= 0)
        {
            Debug.Log("mana bitti");
            OnPlayerTurnEnd(currentTeam);
        }
    }
    public void SetMana(int val)
    {
        currentMana = val;
        menuManager.UpdateManaText(currentMana, manaPerTurn); 
    }


}
