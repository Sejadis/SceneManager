using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenManager : NetworkBehaviour
{
    public Image progressBar;
    private int clientCount;
    private List<ulong> completedClients = new();
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log("test");
        NetworkManager.SceneManager.OnSceneEvent += OnSceneEvent;
    }

    private void OnSceneEvent(SceneEvent sceneEvent)
    {
        switch (sceneEvent.SceneEventType)
        {
            case SceneEventType.Load:
                progressBar.fillAmount = 0;
                completedClients.Clear();
                break;
            case SceneEventType.Unload:
                break;
            case SceneEventType.Synchronize:
                break;
            case SceneEventType.ReSynchronize:
                break;
            case SceneEventType.LoadEventCompleted:
                break;
            case SceneEventType.UnloadEventCompleted:
                break;
            case SceneEventType.LoadComplete:
                if (IsServer)
                {
                    completedClients.Add(sceneEvent.ClientId);
                    SetLoadCompleteClientRPC((float)completedClients.Count / NetworkManager.Singleton.ConnectedClients.Count);
                }
                break;
            case SceneEventType.UnloadComplete:
                break;
            case SceneEventType.SynchronizeComplete:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    [ClientRpc]
    private void SetLoadCompleteClientRPC(float fillAmount)
    {
        Debug.Log("setting fill");
        progressBar.fillAmount = fillAmount;
    }
}
