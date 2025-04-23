using System;
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


public class LoginClient : MonoBehaviour
{

    //private string authenticationEndPoint = "http://localhost:13756/signin";
    private string authenticationEndPoint;

    
    public event Action<string, bool> OnResponse;
   
    void Awake()
    {
    #if UNITY_WEBGL && !UNITY_EDITOR
            string host = Application.absoluteURL;

            if (host.Contains("localhost"))
            {
                authenticationEndPoint = "http://localhost:13756/signin";
            }
            else
            {
                authenticationEndPoint = "https://web-authserver.up.railway.app/signin";
            }
    #else
            authenticationEndPoint = "http://localhost:13756/signin";
    #endif

            Debug.Log("Auth endpoint: " + authenticationEndPoint);
    }
    public void OnTryLogin(string username, string password)
    {
        StartCoroutine(TryLogin(username, password));
    }

    

    private IEnumerator TryLogin(string username, string password)
    {
        
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

            OnResponse?.Invoke(response.token, response.isSuccess);  
     
        }
        else
        {
            OnResponse?.Invoke("", false);
        }
    }
}
