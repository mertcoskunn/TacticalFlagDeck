using System; 

using Unity.Collections; 
using Unity.Networking.Transport; 
using TMPro; 

using UnityEngine;

public class Server : MonoBehaviour
{
    #region Singleton implemantation
    public static Server Instance {set; get;}
    private void Awake()
    {
        Instance = this; 
    }
    #endregion
    
    public TMP_Text text_field; 
    private int ct = 0; 
    public NetworkDriver driver; 
    private NativeList<NetworkConnection> connections; 

    private bool isActive = false; 
    private const float keepAliveTickRate = 20.0f; 
    private float lastKeepAlive; 

    public Action connectionDropped; 

    public void Init(ushort port){
        driver = NetworkDriver.Create();
        NetworkEndpoint endPoint = NetworkEndpoint.AnyIpv4; 
        endPoint.Port = port; 

        if(driver.Bind(endPoint) != 0)
        {
            Debug.Log("Unable to bind on port" + endPoint.Port); 
            return; 
        }
        else
        {
            driver.Listen();
            Debug.Log("Listening on port "+endPoint.Port);
        }

        connections = new NativeList<NetworkConnection>(2, Allocator.Persistent); 
        isActive = true; 


    }


    public void Shutdown(){
        if(isActive)
        {
            isActive = false;
            if (driver.IsCreated)
            {
                driver.Dispose();
            }
            if (connections.IsCreated)
            {
                connections.Dispose();
            } 
        }
    }

    public void OnDestroy(){
        Shutdown();
    }


    public void Update()
    {
        if(!isActive)
        {
            return;
        }
        
        KeepAlive();
        driver.ScheduleUpdate().Complete();
        CleanupConnections();
        AcceptNewConnections();
        UpdateMessagePump();

    }


    private void CleanupConnections()
    {
        for(int i=0; i<connections.Length;i++)
        {
            if(!connections[i].IsCreated)
            {
                connections.RemoveAtSwapBack(i);
                i--;
            }
        }
    }

    private void AcceptNewConnections(){
        NetworkConnection c;
        while((c=driver.Accept()) != default(NetworkConnection))
        {
            connections.Add(c); 
        }
    }


    private void UpdateMessagePump(){
        
        
        DataStreamReader stream; 
        for(int i = 0; i<connections.Length; i++)
        {
            NetworkEvent.Type cmd; 
            while((cmd = driver.PopEventForConnection(connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if(cmd == NetworkEvent.Type.Data)
                {
                    NetUtility.OnData(stream, connections[i], this);
                }
                else if (cmd ==NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnect from server"); 
                    connections[i] = default(NetworkConnection);
                    connectionDropped?.Invoke();
                    Shutdown();
                }
            }

        }
    }

    public void SendToClient(NetworkConnection connection, NetMessage msg)
    {
        DataStreamWriter writer; 
        driver.BeginSend(connection, out writer); 
        msg.Serialize(ref writer);
        driver.EndSend(writer); 
    }
    public void Brodcast(NetMessage msg)
    {
        for(int i = 0; i<connections.Length; i++)
        {
            if(connections[i].IsCreated)
            {
                SendToClient(connections[i], msg);
            }
        }
    }

    private void KeepAlive()
    {
        if((Time.time - lastKeepAlive) > Time.time)
        {
            ct += 1 ;
            text_field.text = ct.ToString() ;
            Brodcast(new NetKeepAlive());
            
        }
    }

}
