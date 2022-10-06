using System;
using System.Collections;
using System.Collections.Generic;
using ParrelSync;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace SejDev.SceneManagement
{
    public class SceneManager : NetworkBehaviour
    {
        public SceneEventProgressStatus status = SceneEventProgressStatus.None;

        private string nextSceneName;

        public Scene loadedScene;
        public GameObject loadingScreen;

        // Start is called before the first frame update
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            NetworkManager.SceneManager.OnSceneEvent += OnSceneEvent;
        }

        private void OnSceneEvent(SceneEvent sceneEvent)
        {
            Debug.Log("SCENE EVENT: " + sceneEvent.SceneEventType);

            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.Load:
                    loadingScreen.SetActive(true);
                    if (ClonesManager.IsClone())
                    {
                        sceneEvent.AsyncOperation.allowSceneActivation = false;
                        StartCoroutine(AllowSceneActivation(sceneEvent.AsyncOperation));
                    }
                    break;
                case SceneEventType.Unload:
                    break;
                case SceneEventType.Synchronize:
                    break;
                case SceneEventType.ReSynchronize:
                    break;
                case SceneEventType.LoadEventCompleted:
                    status = SceneEventProgressStatus.None;
                    loadingScreen.SetActive(false);
                    break;
                case SceneEventType.UnloadEventCompleted:
                    StartCoroutine(Utility.DelayOneFrame(() =>
                    {
                        status = SceneEventProgressStatus.None;
                        if (!string.IsNullOrEmpty(nextSceneName))
                        {
                            LoadScene(nextSceneName);
                            nextSceneName = string.Empty;
                        }
                    }));
                    break;
                case SceneEventType.LoadComplete:
                    if (sceneEvent.ClientId == NetworkManager.Singleton.LocalClientId)
                    {
                        loadedScene = sceneEvent.Scene;
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

        private IEnumerator AllowSceneActivation(AsyncOperation operation)
        {
            string argument = ClonesManager.GetArgument();
            var args = argument.Split(" ");
            int.TryParse(args[1], out var delay);
            yield return new WaitForSeconds(delay);
            operation.allowSceneActivation = true;
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void SwitchScene(string sceneName)
        {
            if (status != SceneEventProgressStatus.None ||
                (loadedScene.IsValid() && loadedScene.name.Equals(sceneName))) return;
            Debug.Log("Connected Clients: " + NetworkManager.ConnectedClients.Count);
            loadingScreen.SetActive(true);
            if (loadedScene.IsValid())
            {
                UnloadScene();
                nextSceneName = sceneName;
            }
            else
            {
                LoadScene(sceneName);
            }
        }

        [ContextMenu("Unload")]
        private void UnloadScene()
        {
            if (status != SceneEventProgressStatus.None || !loadedScene.IsValid()) return;

            // Debug.Log("starting unloading of scene " + loadedScene.name);
            status = NetworkManager.SceneManager.UnloadScene(loadedScene);
            // Debug.Log("started unloading of scene " + loadedScene.name);
        }


        private void LoadScene(string sceneName)
        {
            if (status != SceneEventProgressStatus.None) return;

            // Debug.Log("starting loading of scene " + sceneName);
            status = NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            // Debug.Log("started loading of scene " + sceneName);

            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogWarning($"Failed to load {sceneName} " +
                                 $"with a {nameof(SceneEventProgressStatus)}: {status}");
            }
        }

        [ContextMenu("Load Test")]
        private void LoadTestScene()
        {
            LoadScene("Test");
        }

        [ContextMenu("Switch to Test 2")]
        private void SwitchToTestScene()
        {
            SwitchScene("Test2");
        }
    }
}