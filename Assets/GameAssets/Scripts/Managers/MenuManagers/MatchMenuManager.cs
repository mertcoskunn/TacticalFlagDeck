using TMPro; 
using UnityEngine;
using UnityEngine.UI;
public class MatchMenuManager : MonoBehaviour
{
    [SerializeField]  private GameObject matchMenuObject;
    [SerializeField]  private MatchManager matchManager;
    [SerializeField]  private MatchClient matchClient;

    [SerializeField] private TMP_Text manaText;
    [SerializeField] private TMP_Text turnText;

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
   }

   public void SetManaText(int currentMana, int totalMana)
   {
     manaText.text = "Mana: " + currentMana + "/" + totalMana;
   }
   public void SetTurnText(string text)
   {
     turnText.text = text;
   }

}
