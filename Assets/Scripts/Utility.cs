using System;
using System.Collections;
using UnityEngine;

namespace SejDev.SceneManagement
{
    public static class Utility
    {
        public static IEnumerator DelayOneFrame(Action callback)
        {
            yield return null;
            callback();
        }
    }
}