using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
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
    
    public async void StartAsHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback = CheckConnection;
        StartHostWithRelay();
        NetworkManager.Singleton.OnClientConnectedCallback += ClienteConectado;
        NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnected;
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
    
    /// <summary>
    /// Starts a game host with a relay allocation: it initializes the Unity services, signs in anonymously and starts the host with a new relay allocation.
    /// </summary>
    /// <param name="maxConnections">Maximum number of connections to the created relay.</param>
    /// <returns>The join code that a client can use.</returns>
    /// <exception cref="ServicesInitializationException"> Exception when there's an error during services initialization </exception>
    /// <exception cref="UnityProjectNotLinkedException"> Exception when the project is not linked to a cloud project id </exception>
    /// <exception cref="CircularDependencyException"> Exception when two registered <see cref="IInitializablePackage"/> depend on the other </exception>
    /// <exception cref="AuthenticationException"> The task fails with the exception when the task cannot complete successfully due to Authentication specific errors. </exception>
    /// <exception cref="RequestFailedException"> See <see cref="IAuthenticationService.SignInAnonymouslyAsync"/></exception>
    /// <exception cref="ArgumentException">Thrown when the maxConnections argument fails validation in Relay Service SDK.</exception>
    /// <exception cref="RelayServiceException">Thrown when the request successfully reach the Relay Allocation service but results in an error.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the UnityTransport component cannot be found.</exception>
    public async Task<string> StartHostWithRelay(int maxConnections=5)
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
        var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        Debug.Log("JOIN CODE: "+joinCode);
        return NetworkManager.Singleton.StartHost() ? joinCode : null;
    }

    
}
