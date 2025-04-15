using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Networking;  
using UnityEngine.UI;


[System.Serializable]
public class AccountData
{
    public string username;
    public string password;
}


[System.Serializable]
public class AuthResultData
{
    public string token;
    public bool isSuccess;
}


public class LoginManager : MonoBehaviour
{

    [SerializeField] private string authenticationEndPoint = "http://127.0.0.1:13756/signin";
    [SerializeField] private Button loginButton;



    void Start()
    {
         if(loginButton != null)
         {
            loginButton.onClick.AddListener(OnLoginClick);
        }
    }
    public void OnLoginClick()
    {
        StartCoroutine(TryLogin());
    }

    

    private IEnumerator TryLogin()
    {
        string username = "mert22"; 
        string password = "1234"; 

        var data = new AccountData {username=username, password=password}; 
        string json = JsonUtility.ToJson(data);
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(json); 


        UnityWebRequest request = new UnityWebRequest(authenticationEndPoint, "POST");
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            AuthResultData response = JsonUtility.FromJson<AuthResultData>(request.downloadHandler.text);
            Debug.Log(response.token);
            Debug.Log(response.isSuccess); 
     
        }
        else
        {
        Debug.LogError("Failed to connect: " + request.error);
        }
    }
}
