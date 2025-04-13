using TMPro; 
using Unity.Networking.Transport; 
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public static MenuManager Instance {set; get;}
    public Server server; 
    public Client client; 

    //[SerializeField] private TMP_InputField adressInput; 
    public Button startLocalGameButton;
    public Button onlineGameButton;
    public Button hostGameButton;
    public Button connectGame; 
    public Button backButtonOnlineGameMenu;
    public Button backButtonHostGameMenu;
    public Button backButtonToMainMenu;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI turnText;
 

    public GameObject mainMenuObject;
    public GameObject onlineMenuObject;
    public GameObject hostMenuObject;
    public GameObject matchMenuObject; 
    public GameObject resultMenuObject; 

    private int playerCount = -1;
    private int currentTeam = -1;



    private void Awake()
    {
        RegisterToEvent();
    }



    void Start()
    {

        if(startLocalGameButton != null){
            startLocalGameButton.onClick.AddListener(OnLocalGameButtonClick);
        }
        if(onlineGameButton != null){
            onlineGameButton.onClick.AddListener(OnOnlineGameButtonClick);

            
        }

        if(hostGameButton != null){
            hostGameButton.onClick.AddListener(OnHostButtonClick);
        }
        

        if(connectGame != null){
            connectGame.onClick.AddListener(OnConnectGameButtonClick);
           
        }
       

        if(backButtonOnlineGameMenu != null){
            backButtonOnlineGameMenu.onClick.AddListener(OnBackButtonOnlineMenuClick);
        }

        if(backButtonHostGameMenu != null){
            backButtonHostGameMenu.onClick.AddListener(OnBackButtonHostMenuClick);
        }

        if(backButtonToMainMenu != null){
            backButtonToMainMenu.onClick.AddListener(OnBackButtonToMainMenu);
        }


    }

    // Update is called once per frame
    void OnLocalGameButtonClick(){
        mainMenuObject.SetActive(false); 
    }

     void OnOnlineGameButtonClick(){
         mainMenuObject.SetActive(false);
         onlineMenuObject.SetActive(true); 
    }

    void OnHostButtonClick(){
         onlineMenuObject.SetActive(false);
         hostMenuObject.SetActive(true);
         server.Init(8080);
         client.Init("127.0.0.1", 8080);  
    }

    void OnConnectGameButtonClick(){
        client.Init("127.0.0.1", 8080);
    }

    void OnBackButtonOnlineMenuClick(){
        onlineMenuObject.SetActive(false);
        mainMenuObject.SetActive(true); 
    }

    void OnBackButtonHostMenuClick(){
        hostMenuObject.SetActive(false);
        onlineMenuObject.SetActive(true);
        server.Shutdown();
        client.Shutdown();
        playerCount = -1;  
    }

    void OnBackButtonToMainMenu(){
        resultMenuObject.SetActive(false);
        mainMenuObject.SetActive(true); 
    }


    private void RegisterToEvent()
    {
        NetUtility.S_WELCOME += OnWelcomeServer;
        NetUtility.C_WELCOME += OnWelcomeClient; 
        NetUtility.C_START_GAME += GameStart; 
    }

    private void UnregisterToEvent()
    {
         NetUtility.S_WELCOME -= OnWelcomeServer;
    }

    private void OnWelcomeServer(NetMessage msg, NetworkConnection cnn)
    {
        NetWelcome nw = msg as NetWelcome;
        nw.AssignedTeam = ++playerCount; 
        Server.Instance.SendToClient(cnn, nw);

        if(playerCount == 1 )
        {
            Server.Instance.Brodcast(new NetStartGame());
        }
    }

    private void OnWelcomeClient(NetMessage msg)
    {
        NetWelcome nw = msg as NetWelcome;
        currentTeam = nw.AssignedTeam;
        Debug.Log("My ASSIGN TEAM IS " + currentTeam);
        
    }

    public void GameStart(NetMessage msg)
    {
        mainMenuObject.SetActive(false);
        onlineMenuObject.SetActive(false);
        hostMenuObject.SetActive(false); 

        matchMenuObject.SetActive(true); 
    }


    public void OpenResultMenu(string result)
    {
        matchMenuObject.SetActive(false);
        Invoke("ShutDownNetwork", 0.5f); 
        resultText.text = result; 
        resultMenuObject.SetActive(true);
        currentTeam = -1;
        playerCount = -1;   
    }


    private void ShutDownNetwork(){
        client.Shutdown();
        server.Shutdown();
    }


    public void UpdateTurnText(string turn)
    {
        turnText.text = turn; 
    }

    public void UpdateManaText(int currentMana, int totalMana)
    {
        manaText.text = "Mana: " + currentMana + "/" + totalMana;
    }
}
