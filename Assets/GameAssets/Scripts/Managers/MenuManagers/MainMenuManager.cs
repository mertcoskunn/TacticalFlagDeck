using TMPro; 
using UnityEngine;
using UnityEngine.UI;
public class MainMenuManager: MonoBehaviour
{
    [SerializeField]  private GameObject mainMenuMenuObject;
    [SerializeField]  private Button findMatchUpButton;

    
    [SerializeField]  private MatchMakingMenuManager matchMakingMenuManager;

    


    void Start()
    {
        if(findMatchUpButton != null)
        {
            findMatchUpButton.onClick.AddListener(OnFindMatchButoonClick); 
        }

       
    }


   void OnFindMatchButoonClick()
   {

        Close();
        matchMakingMenuManager.Open();        
   }

   public void Open()
   {
        mainMenuMenuObject.SetActive(true);

   }

   public void Close()
   {
        mainMenuMenuObject.SetActive(false);
   }
}
