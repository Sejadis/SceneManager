using System.Collections;
using System.Collections.Generic;
using ParrelSync;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInitializer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!ClonesManager.IsClone())
        {
            SceneManager.LoadScene("SceneManager", LoadSceneMode.Additive);
        }

        SceneManager.LoadScene("Network", LoadSceneMode.Additive);
    }
}