using System; 
using Unity.Collections; 
using Unity.Networking.Transport; 
using UnityEngine;

public class Client : MonoBehaviour
{
    #region Singleton implemantation
    public static Client Instance {set; get;}
    private void Awake()
    {
        Instance = this; 
    }
    #endregion

    public NetworkDriver driver; 
    private NetworkConnection connection; 
    private bool isActive = false; 
    private const float keepAliveTickRate = 20.0f; 
    public Action connectionDropped; 

    public void Init(string ip, ushort port){
        driver = NetworkDriver.Create();
        NetworkEndpoint endPoint = NetworkEndpoint.Parse(ip, port); 

        connection = driver.Connect(endPoint);
        isActive = true;

        RegisterToEvent(); 
    }


    public void Shutdown(){
        if(isActive)
        {
            UnregisterToEvent();
            isActive = false;
            if (driver.IsCreated)
            {
                driver.Dispose();
            }   
           
            connection = default(NetworkConnection);
        }
    }

    public void OnDestroy(){
        Shutdown();
    }


public void Update()
    {
        if (!isActive || !driver.IsCreated)
        return;
        
        driver.ScheduleUpdate().Complete();
        CheckAlive(); 
        UpdateMessagePump();

    }


    private void CheckAlive()
    {
        
        if(!connection.IsCreated && isActive)
        {
            Debug.Log("Something wrong, lost connections to server"); 
            connectionDropped?.Invoke();
            Shutdown();
        }
    }
    

    private void UpdateMessagePump(){
        
        if (!driver.IsCreated || !connection.IsCreated)
            return;

        DataStreamReader stream; 
        
        NetworkEvent.Type cmd;

        while((cmd = connection.PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
        {

            if(cmd == NetworkEvent.Type.Connect)
            {
                SendToServer(new NetWelcome());
               
                
            }
            else if(cmd == NetworkEvent.Type.Data)
            {
                NetUtility.OnData(stream, default(NetworkConnection));
            }
            else if (cmd ==NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from server");
                connection = default(NetworkConnection); 
                connectionDropped?.Invoke();
                Shutdown();
            }
        }   
    }

    public void SendToServer(NetMessage msg){
        DataStreamWriter writer; 
        driver.BeginSend(connection, out writer);
        msg.Serialize(ref writer);
        driver.EndSend(writer);

    }


    private void RegisterToEvent(){
        NetUtility.C_KEEP_ALIVE += OnKeepAlive; 

    }

    private void UnregisterToEvent(){
        NetUtility.C_KEEP_ALIVE -= OnKeepAlive; 
        
    }



    private void OnKeepAlive(NetMessage msg)
    {
        SendToServer(msg); 
    }
}
