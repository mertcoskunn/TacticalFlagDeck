using System;
using UnityEngine;
using NativeWebSocket;

[Serializable]
public class MatchMessage
{
    public string type;
    public string matchID;
    public int currentTeam;
    public Vector2 initPoisiton;
    public Vector2 targetPoisiton;
    public int cardIndex;
    public Vector2 gridPosition;

}

public class MatchClient : MonoBehaviour
{
    WebSocket websocket;
    public event Action<string, string> OnMessageArrived;
    public event Action<NetworkMessage> OnMatchStart;
    public event Action<CreateCharMessage> OnCharCreate;
    public event Action<MoveMessage> OnMove;
    public event Action<AttackMessage> OnAttack;
    public event Action<TurnEndMessage> OnTurnEnd;
    public event Action<MatchEndMessage> OnMatchEnd;

    async public void TryConnect()
    {
        //string endPoint = "ws://localhost:4000";
        string endPoint ;
#if UNITY_WEBGL && !UNITY_EDITOR
    string host = Application.absoluteURL;

    if (host.Contains("localhost"))
    {
        endPoint = "ws://localhost:4000";
    }
    else
    {
        endPoint = "wss://web-matchserver.up.railway.app";
    }
#else
    endPoint = "ws://localhost:4000";
#endif
        
        websocket = new WebSocket(endPoint);

        websocket.OnOpen += () =>
        {
            HelloMessage msg = new HelloMessage(GameManager.Instance.AuthToken, GameManager.Instance.MatchID);
        
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

            try
            {
                NetworkMessage message = JsonUtility.FromJson<NetworkMessage>(jsonMessage);
                
                OnMessageArrived?.Invoke(message.type, message.matchID);

                if(message.type == "match_connection_ready")
                {
                    InitMatchMessage msg = new InitMatchMessage(GameManager.Instance.AuthToken, GameManager.Instance.MatchID);
                    string json = JsonUtility.ToJson(msg);

                    websocket.SendText(json);

                }

                if(message.type == "START_MATCH")
                {
                    OnMatchStart?.Invoke(message);
                }

                if(message.type == "CHAR_CREATE")
                {
                    CreateCharMessage msg = JsonUtility.FromJson<CreateCharMessage>(jsonMessage);
                    OnCharCreate?.Invoke(msg); 

                }
                if(message.type == "MOVE")
                {
                    MoveMessage msg = JsonUtility.FromJson<MoveMessage>(jsonMessage);
                    OnMove?.Invoke(msg); 

                }
                if(message.type == "ATTACK")
                {
                    AttackMessage msg = JsonUtility.FromJson<AttackMessage>(jsonMessage);
                    OnAttack?.Invoke(msg); 

                }

                if(message.type == "TURN_END")
                {
                    TurnEndMessage msg = JsonUtility.FromJson<TurnEndMessage>(jsonMessage);
                    OnTurnEnd?.Invoke(msg); 
                }

                if(message.type == "MATCH_END")
                {
                    MatchEndMessage msg = JsonUtility.FromJson<MatchEndMessage>(jsonMessage);
                    OnMatchEnd?.Invoke(msg); 
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


    public async void SendMessageToServer(NetworkMessage msg)
    {
        
        string json = JsonUtility.ToJson(msg);
        
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            websocket.SendText(json);
            Debug.Log($"Sent message to server: {json}");
        }
        else
        {
            Debug.LogWarning("WebSocket is not open. Unable to send message.");
        }
    }


}
