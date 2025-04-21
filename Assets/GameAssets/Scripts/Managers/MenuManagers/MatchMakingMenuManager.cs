using UnityEngine;
using UnityEngine.UI;
public class MatchMakingMenuManager: MonoBehaviour
{
    [SerializeField]  private GameObject matchMakingMenuObject;
    [SerializeField]  private Button backMatchMakingMenuButton;

    [SerializeField]  private MainMenuManager mainMenuManager;

    [SerializeField]  private MatchMakingClient matchMakingClient;
    [SerializeField]  private MatchMenuManager matchMenuManager;


    void Start()
    {
        if(backMatchMakingMenuButton != null)
        {
            backMatchMakingMenuButton.onClick.AddListener(OnBackMatchMakingMenuButtonClick); 
        }

       
    }


   void OnBackMatchMakingMenuButtonClick()
   {
       Close();
       mainMenuManager.Open();
   }

   public void Open()
   {
    matchMakingMenuObject.SetActive(true); 
    matchMakingClient.OnMatchReady += HandleMatchReady; 
    matchMakingClient.TryConnect();
   }

   private void HandleMatchReady(string type, string matchID)
   {
        GameManager.Instance.SetMatchID(matchID);
        Close();
        matchMenuManager.Open();
   }

   public void Close()
   {
    matchMakingClient.Disconnect();
    matchMakingMenuObject.SetActive(false); 
    
   }
}
