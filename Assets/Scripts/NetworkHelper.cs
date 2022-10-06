using System;
using System.Collections;
using System.Collections.Generic;
using ParrelSync;
using Unity.Netcode;
using UnityEngine;

public class NetworkHelper : MonoBehaviour
{
    private void Start()
    {
        string customArgument = ClonesManager.GetArgument();
        if (customArgument.Contains("client"))
        {
            NetworkManager.Singleton.StartClient();
        }
        else
        {
            NetworkManager.Singleton.StartHost();
        }

        
    }

    [ContextMenu("Host")]
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }
    
    [ContextMenu("Client")]
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
