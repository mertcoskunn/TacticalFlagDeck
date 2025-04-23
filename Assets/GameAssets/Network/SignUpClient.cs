using System;
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


public class SignUpClient : MonoBehaviour
{
 
    //string authenticationEndPoint = "http://127.0.0.1:13756/signup";
    private string authenticationEndPoint;


    public event Action<string, bool> OnResponse;
    
    void Awake()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
                string host = Application.absoluteURL;

                if (host.Contains("localhost"))
                {
                    authenticationEndPoint = "http://localhost:13756/signup";
                }
                else
                {
                    authenticationEndPoint = "https://web-authserver.up.railway.app/signup";
                }
        #else
                authenticationEndPoint = "http://localhost:13756/signup";
        #endif

                Debug.Log("Auth endpoint: " + authenticationEndPoint);
    }
    public void OnTrySignUp(string username, string password)
    {
        StartCoroutine(TrySignUp(username, password));
    }

    

    private IEnumerator TrySignUp(string username, string password)
    {
       
        var data = new AccountData {username=username, password=password}; 
        string json = JsonUtility.ToJson(data);
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(json); 


        UnityWebRequest request = new UnityWebRequest(authenticationEndPoint, "POST");
        request.uploadHandler = new UploadHandlerRaw(postData);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if(request.result == UnityWebRequest.Result.Success)
        {
            SignUpAutResultData response = JsonUtility.FromJson<SignUpAutResultData>(request.downloadHandler.text);
            Debug.Log(response.userName);
            Debug.Log(response.isSuccess);

            OnResponse?.Invoke("", response.isSuccess); 
        }
        else
        {
            OnResponse?.Invoke("", false);
            Debug.LogError("Failed to connect: " + request.error);
        }
    }
}
