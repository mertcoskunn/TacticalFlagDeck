using System;
using UnityEngine;
using NativeWebSocket;


[Serializable]
public class MatchMakingMessage
{
    public string type;
    public string match_id;
}

public class MatchMakingClient : MonoBehaviour
{
    WebSocket websocket;
    public event Action<string, string> OnMessageArrived;
    public event Action<string, string> OnMatchReady;

    async public void TryConnect()
    {
        websocket = new WebSocket("ws://localhost:3000");

        websocket.OnOpen += () =>
        {
            MatchMakingMessage msg = new MatchMakingMessage
            {
                type = "HELLO",
                match_id = ""
            };

            string json = JsonUtility.ToJson(msg);

            websocket.SendText(json);
            Debug.Log("WebSocket connected");
                
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("WebSocket error: " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("WebSocket closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            string jsonMessage = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("MatchMaking Server Message: " + jsonMessage);

            try
            {
                MatchMakingMessage message = JsonUtility.FromJson<MatchMakingMessage>(jsonMessage);
                Debug.Log("Type: " + message.type);
                Debug.Log("Game_id: " + message.match_id);
                OnMessageArrived?.Invoke(message.type, message.match_id);
                if(message.type == "match_ready")
                {
                    OnMatchReady?.Invoke(message.type, message.match_id);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("JSON couldn't was parsed : " + ex.Message);
            }
        };

        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif
    }

    public async void Disconnect()
    {
        if(websocket != null)
        {
            await websocket.Close();
        }
        
    }

    public async void OnApplicationQuit()
    {
        if(websocket != null)
        {
            await websocket.Close();
        }
        
    }
}
