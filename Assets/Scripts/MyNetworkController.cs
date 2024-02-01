using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MyNetworkController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += ClienteConectado;

    }

    private void ClienteConectado(ulong obj)
    {
       Debug.Log("Cliente conectado "+obj);
    }

    public void StartAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }
    
    public void StartAsClient()
    {
        NetworkManager.Singleton.StartClient();
    }public void StartAsServer()
    {
        NetworkManager.Singleton.StartServer();
    }

    
}
