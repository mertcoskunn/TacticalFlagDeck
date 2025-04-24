using TMPro; 
using UnityEngine;
using UnityEngine.UI;
public class LoginMenuManager : MonoBehaviour
{
    [SerializeField]  private GameObject loginMenuObject;
    [SerializeField]  private TMP_InputField  loginUserNameInputField;
    [SerializeField]  private TMP_InputField  loginPasswordInputField;
    [SerializeField]  private TMP_Text  loginErrorText;
    [SerializeField]  private Button loginButton;
    [SerializeField]  private Button signUpMenuButton;

    //[SerializeField]  private GameObject signUpMenuObject;

    [SerializeField]  private SignUpMenuManager signUpMenuManager;
    [SerializeField]  private MainMenuManager mainMenuManager;
    //[SerializeField]  private GameObject mainMenuObject;

    [SerializeField]  private LoginClient loginClient;

    


    void Start()
    {
        if(loginButton != null)
        {
            loginButton.onClick.AddListener(OnLoginButtonClick);
            loginButton.interactable = false; 
        }

        if(signUpMenuButton != null )
        {
            signUpMenuButton.onClick.AddListener(OnSignUpMenuButtonClick); 
        }


        loginUserNameInputField.onValueChanged.AddListener(delegate { OnInputValueChanged(); });
        loginPasswordInputField.onValueChanged.AddListener(delegate { OnInputValueChanged(); });
    }


    void OnInputValueChanged()
    {
        bool usernameNotEmpty = !string.IsNullOrWhiteSpace(loginUserNameInputField.text);
        bool passwordNotEmpty = !string.IsNullOrWhiteSpace(loginPasswordInputField.text);

        loginButton.interactable = usernameNotEmpty && passwordNotEmpty;
    }   

    void OnLoginButtonClick()
    {
        loginClient.OnResponse += OnLoginResponse;
        Debug.Log(loginUserNameInputField.text);
        Debug.Log(loginPasswordInputField.text);
        loginClient.OnTryLogin(loginUserNameInputField.text, loginPasswordInputField.text);

    }

    void OnLoginResponse(string token, bool isSuccess){
       if(isSuccess)
       {
        GameManager.Instance.SetAuthToken(token); 
        Close();
        mainMenuManager.Open(); 
       }
       else{
        loginErrorText.text = "Invalid username or password"; 
       } 
    }

    void OnSignUpMenuButtonClick()
    {
        Close();
        signUpMenuManager.Open();     
    }

    public void Open()
    {
        loginUserNameInputField.text = "";
        loginPasswordInputField.text = "";

        loginMenuObject.SetActive(true); 

    }
    public void Close()
    {
        loginUserNameInputField.text = "";
        loginPasswordInputField.text = "";

        loginMenuObject.SetActive(false);

    }
}
