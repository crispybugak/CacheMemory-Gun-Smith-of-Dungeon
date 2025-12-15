using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialSkip : MonoBehaviour
{
    private void FixedUpdate()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SceneTransitionManager.Instance.LoadScene("04.JHJ/00.Scenes/JHJ.LobbyScene");
        }
    }
}
