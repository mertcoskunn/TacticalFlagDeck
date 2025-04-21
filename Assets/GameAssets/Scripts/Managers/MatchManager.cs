using UnityEngine;

public class MatchManager : MonoBehaviour
{
    [SerializeField] private DeckManager deckManager; 
    [SerializeField] private GridManager gridManager;
    [SerializeField] private HandManager handManager;
    [SerializeField] private MatchClient matchClient;

    [SerializeField] private MatchMenuManager matchMenuManager;
    [SerializeField] private EndMatchMenuManager endMatchMenuManager;

    private int manaPerTurn = 3; 
    private int currentMana = 3;   
    void Start()
    {
        RegisterToEvent();
        
    }

    private void RegisterToEvent()
    {
        matchClient.OnMatchStart += HandleMatchStartClient;
        matchClient.OnCharCreate += HandleCharCreateClient;
        matchClient.OnMove += HandleMakeMoveClient;
        matchClient.OnAttack += HandleAttackClient; 
        matchClient.OnTurnEnd += HandleTurnEndClient;
        matchClient.OnMatchEnd += HandleMatchEndClient;


        gridManager.OnAddObjectToGrid += HandleObjectAddedToGrid;
        gridManager.OnMakeMove += HandleMakeMove;
        gridManager.OnAttack  += HandleCharAttack;
        gridManager.OnMatchEnd += OnMatchEnd;
    }

    public void HandleMatchStartClient(NetworkMessage msg)
    {
        SetMana(manaPerTurn);
        GameManager.Instance.SetCurrentTeam(msg.currentTeam);
        gridManager.Init(msg.currentTeam);
        gridManager.SetLocked(true); 

        if(msg.currentTeam == 0)
        {
            deckManager.Init(5);            
            handManager.SetLockedCardsInHand(false);
            gridManager.SetLocked(false);
            matchMenuManager.SetTurnText("Your Turn");
        }
        else
        {
            deckManager.Init(4);
            handManager.SetLockedCardsInHand(true);
            matchMenuManager.SetTurnText("Enemy Turn"); 
        }
    }

    
    public void HandleCharCreateClient(CreateCharMessage msg)
    {
        if(msg.currentTeam != GameManager.Instance.CurrentTeam)
        {
            gridManager.AddObjectToGrid(msg.cardIndex, new Vector2(msg.posX, msg.posY), msg.currentTeam);
        }
    }
    public void HandleMakeMoveClient(MoveMessage msg)
    {
        if(msg.currentTeam != GameManager.Instance.CurrentTeam)
        {
            gridManager.MoveObject(new Vector2(msg.initPosX, msg.initPosY), new Vector2(msg.targetPosX, msg.targetPosY));
        }
    }
    public void HandleAttackClient(AttackMessage msg)
    {
        if(msg.currentTeam != GameManager.Instance.CurrentTeam)
        {
            Debug.Log("Burdayım burda HandleAttackClienyın içinde"); 
            gridManager.Attack(new Vector2(msg.initPosX, msg.initPosY), new Vector2(msg.targetPosX, msg.targetPosY));
        }
    }
    private void HandleTurnEndClient(TurnEndMessage msg)
    {
        if(msg.currentTeam != GameManager.Instance.CurrentTeam)
        {
            OnPlayerTurnStart();
        }
    }
    private void HandleMatchEndClient(MatchEndMessage msg)
    {
        handManager.SetLockedCardsInHand(true);
        gridManager.SetLocked(true);

        gridManager.Reset();
        handManager.Reset();
        deckManager.Reset();
        
        if(msg.currentTeam != GameManager.Instance.CurrentTeam)
        {
           matchMenuManager.Close();
           endMatchMenuManager.Open("Failed");
        }
        else
        {
            matchMenuManager.Close();
            endMatchMenuManager.Open("Victory");
        }    
    }
    public void HandleObjectAddedToGrid(int currentTeam, int cardIndex, Vector2 gridPoisition)
    {
        UpdateMana(-1); 
        CreateCharMessage nw = new CreateCharMessage(GameManager.Instance.AuthToken, GameManager.Instance.MatchID, GameManager.Instance.CurrentTeam, cardIndex, (int)gridPoisition.x, (int)gridPoisition.y);
        matchClient.SendMessageToServer(nw);
    }
    public void HandleMakeMove(Vector2 initPos, Vector2 targetPos)
    {
        UpdateMana(-1);
        MoveMessage nw = new MoveMessage(GameManager.Instance.AuthToken, GameManager.Instance.MatchID, GameManager.Instance.CurrentTeam, (int)initPos.x, (int)initPos.y, (int)targetPos.x, (int)targetPos.y);
        matchClient.SendMessageToServer(nw);
    }
    public void HandleCharAttack(Vector2 initPos, Vector2 targetPos)
    {
        UpdateMana(-1);
        AttackMessage msg = new AttackMessage(GameManager.Instance.AuthToken, GameManager.Instance.MatchID, GameManager.Instance.CurrentTeam, (int)initPos.x, (int)initPos.y, (int)targetPos.x, (int)targetPos.y);
        matchClient.SendMessageToServer(msg);
    }  
    public void SetMana(int val)
    {
        currentMana = val;
        matchMenuManager.SetManaText(currentMana, manaPerTurn); 
    }
    public void UpdateMana(int deltaMana)
    {
        SetMana(currentMana + deltaMana); 
        matchMenuManager.SetManaText(currentMana, manaPerTurn);
        if(currentMana <= 0)
        {
            OnPlayerTurnEnd(GameManager.Instance.CurrentTeam);
        }
    }

    public void OnPlayerTurnEnd(int team)
    {
        handManager.SetLockedCardsInHand(true);
        gridManager.SetLocked(true); 
        
        matchMenuManager.SetTurnText("Enemy Turn");
        
        TurnEndMessage nw = new TurnEndMessage(GameManager.Instance.AuthToken, GameManager.Instance.MatchID, GameManager.Instance.CurrentTeam);
        matchClient.SendMessageToServer(nw);

    } 
    public void OnPlayerTurnStart()
    {
        SetMana(manaPerTurn);
        matchMenuManager.SetTurnText("Your Turn"); 
        deckManager.DrawCard(handManager); 
        gridManager.SetLocked(false);
        handManager.SetLockedCardsInHand(false); 
    }
    private void OnMatchEnd(int team)
    {
        handManager.SetLockedCardsInHand(true);
        gridManager.SetLocked(true); 

        MatchEndMessage msg = new MatchEndMessage(GameManager.Instance.AuthToken, GameManager.Instance.MatchID, GameManager.Instance.CurrentTeam);
        matchClient.SendMessageToServer(msg); 
    }
}
