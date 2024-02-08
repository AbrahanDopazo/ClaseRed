using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MyNetworkController : NetworkBehaviour
{
  

    private void ClienteConectado(ulong obj)
    {
       Debug.Log("Cliente conectado "+obj);
    }
    
    private void ClientDisconnected(ulong obj)
    {
        Debug.Log("Cliente desconectado "+obj);
        Debug.Log("Declined Reason: " + NetworkManager.Singleton.DisconnectReason);
    }
    public void StartAsHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback = CheckConnection;
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.OnClientConnectedCallback += ClienteConectado;
        NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnected;
    }

    public int playerCount = 0;

    private void CheckConnection(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse respons)
    {
        if (request.ClientNetworkId > 0)
        {
            playerCount++;
        }
        if (playerCount > 2)
        {
            respons.Reason = "Excedido máximo nro de xogadores";
            respons.Approved = false;
        }
        else
        { 
            respons.Approved = true;
            respons.CreatePlayerObject = true;
            respons.Position = Vector3.zero;
            respons.Rotation = Quaternion.identity;
            respons.Pending = false;
        }
    }

    public void PlayerReceivesDamage()
    {
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerPrefab>().TakeDamage(5f);
        
    }

    private int pcount = 0;

    public void LaunchPing()
    {
        PingRpc(pcount++);
    }
    
    [Rpc(SendTo.Server)]
    public void PingRpc(int pingCount)
    {
        // Server -> Clients because PongRpc sends to NotServer
        // Note: This will send to all clients.
        // Sending to the specific client that requested the pong will be discussed in the next section.
        PongRpc(pingCount, "PONG!");
    }

    [Rpc(SendTo.NotServer)]
    void PongRpc(int pingCount, string message) 
    {
        Debug.Log($"Received pong from server for ping {pingCount} and message {message}");
    }
    
    public void PlayerHealsDamage()
    {
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerPrefab>().Healing(5f);
    }
    public void StartAsClient()
    {
        NetworkManager.Singleton.StartClient();
        NetworkManager.Singleton.OnClientConnectedCallback += ClienteConectado;
        NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnected;
    }
    public void StartAsServer()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback = CheckConnection;
        NetworkManager.Singleton.StartServer();
        NetworkManager.Singleton.OnClientConnectedCallback += ClienteConectado;
        NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnected;
    }

    
}
