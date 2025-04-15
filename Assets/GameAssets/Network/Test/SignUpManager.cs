using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Networking;  
using UnityEngine.UI;





[System.Serializable]
public class SignUpAutResultData
{
    public string userName;
    public bool isSuccess;
}


public class SignUpManager : MonoBehaviour
{

    [SerializeField] private string authenticationEndPoint = "http://127.0.0.1:13756/signup";
    [SerializeField] private Button loginButton;



    void Start()
    {
         if(loginButton != null)
         {
            loginButton.onClick.AddListener(OnSignUp);
        }
    }
    public void OnSignUp()
    {
        StartCoroutine(TrySignUp());
    }

    

    private IEnumerator TrySignUp()
    {
        string username = "mert2244"; 
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
            SignUpAutResultData response = JsonUtility.FromJson<SignUpAutResultData>(request.downloadHandler.text);
            Debug.Log(response.userName);
            Debug.Log(response.isSuccess); 
     
        }
        else
        {
        Debug.LogError("Failed to connect: " + request.error);
        }
    }
}
