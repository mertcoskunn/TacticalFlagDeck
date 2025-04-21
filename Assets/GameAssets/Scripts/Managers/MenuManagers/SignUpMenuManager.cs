using TMPro; 
using UnityEngine;
using UnityEngine.UI;
public class SignUpMenuManager : MonoBehaviour
{
    [SerializeField]  private GameObject signUpMenuObject;
    [SerializeField]  private TMP_InputField  signUpUserNameInputField;
    [SerializeField]  private TMP_InputField  signUpPasswordInputField;
    [SerializeField]  private TMP_Text  signUpErrorText;
    [SerializeField]  private Button signUpButton;
    [SerializeField]  private Button backSignUpMenuButton;

    
    [SerializeField]  private GameObject loginMenuObject;
    [SerializeField]  private SignUpClient signUpClient;

    


    void Start()
    {
        if(signUpButton != null)
        {
            signUpButton.onClick.AddListener(OnSignUpButtonClick); 
        }

        if(backSignUpMenuButton != null )
        {
            backSignUpMenuButton.onClick.AddListener(OnBackSignUpMenuButton); 
        }
    }


    void OnSignUpButtonClick()
    {
        signUpClient.OnResponse += OnSignUpResponse;
        Debug.Log(signUpUserNameInputField.text);
        Debug.Log(signUpPasswordInputField.text);
        signUpClient.OnTrySignUp(signUpUserNameInputField.text, signUpPasswordInputField.text);

    }

    void OnSignUpResponse(string token, bool isSuccess){
       if(isSuccess)
       {
        signUpMenuObject.SetActive(false);
        loginMenuObject.SetActive(true);
       }
       else{
        signUpErrorText.text = "Invalid username or password"; 
       } 
    }

    void OnBackSignUpMenuButton()
    {
        signUpMenuObject.SetActive(false);
        loginMenuObject.SetActive(true); 
    }

    public void Open()
    {
        signUpUserNameInputField.text = "";
        signUpPasswordInputField.text = "";

        signUpMenuObject.SetActive(true); 

    }
    
    public void Close()
    {
        signUpUserNameInputField.text = "";
        signUpPasswordInputField.text = "";

        signUpMenuObject.SetActive(false); 

    }
}
