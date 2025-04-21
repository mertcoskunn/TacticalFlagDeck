using TMPro; 
using UnityEngine;
using UnityEngine.UI;

public class EndMatchMenuManager : MonoBehaviour
{
    [SerializeField]  private GameObject endMatchMenuObject;
    [SerializeField]  private Button backButton;
    [SerializeField]  private TMP_Text resultText;

    
    [SerializeField]  private MainMenuManager mainMenuManager;

    void Start()
    {
        if(backButton != null)
        {
            backButton.onClick.AddListener(OnButonClick); 
        }

        
    }

    private void OnButonClick()
    {
        Close();
        mainMenuManager.Open(); 
    }

    public void Open(string text)
    {
        resultText.text = text; 
        endMatchMenuObject.SetActive(true); 
    }

    public void Close()
    {
        endMatchMenuObject.SetActive(false); 
    }
}
