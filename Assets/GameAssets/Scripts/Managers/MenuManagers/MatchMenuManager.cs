using UnityEngine;

public class MatchMenuManager : MonoBehaviour
{
    [SerializeField]  private GameObject matchMenuObject;
    

    [SerializeField]  private MatchMakingNew matchManager;
    [SerializeField]  private MatchClient matchClient;


    

   

   public void Open()
   {
    matchClient.OnMatchStart += HandleMatchReady; 
    matchClient.TryConnect();

   }

   private void HandleMatchReady(NetworkMessage msg)
   {
        matchMenuObject.SetActive(true);
        //matchManager.HandleMatchStartClient(msg); 
   }

   public void Close()
   {
     matchClient.Disconnect();
     matchMenuObject.SetActive(false); 
    //matchMakingMenuObject.SetActive(false); 
    
   }
}
