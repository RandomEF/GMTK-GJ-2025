using System;
using System.Data;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public GameObject player;
    public MenuManager menuManager;
    public InputSystem_Actions inputs;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Sets the player manager to this object
        }
        inputs = new InputSystem_Actions(); // make a new set of player inputs
        inputs.Player.Enable();
    }

    void Start()
    {
        menuManager = MenuManager.Instance; // Get the menu manager
        DontDestroyOnLoad(gameObject); // Since the game will start in the title screen, it makes sure that the player, manager, and all things attached are loaded and maintained across all scene loads
    }

    public void SetCursorMode()
    {
        if (SceneManager.GetActiveScene().name == "Title")
        {
            Camera.main.GetComponent<PlayerLook>().SetCursorLock(false); // Set whether the cursor should be locked or not
        }
        else
        {
            Camera.main.GetComponent<PlayerLook>().SetCursorLock(true);
        }
    }

    public void Quit()
    {
#if UNITY_EDITOR // If this function is being run inside the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false; // Set the current playing state to false
#else
        Application.Quit(); // Call the system-level quit on the game
#endif
    }
}
