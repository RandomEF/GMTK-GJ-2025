using UnityEngine;
using UnityEngine.InputSystem;

public class PauseGame : MonoBehaviour
{
    private PlayerManager manager;
    private MenuManager menuManager;
    private InputSystem_Actions playerInputs;
    [SerializeField] private string prePauseMenu;
    bool paused = false;

    void Start()
    {
        manager = PlayerManager.Instance; // Get the game manager
        menuManager = MenuManager.Instance;
        playerInputs = manager.inputs; // Get the player's inputs
        playerInputs.Player.Pause.performed += Pause; // Run Pause() when the player pauses the game
        playerInputs.UI.Exit.performed += Pause; // Run Pause() when the player unpauses the game
        playerInputs.CraftMenu.Pause.performed += Pause; // Run Pause() when the player unpauses the game
        playerInputs.OrdersMenu.Pause.performed += Pause; // Run Pause() when the player unpauses the game
        //previousActionMaps = playerInputs;
    }

    private void Pause(InputAction.CallbackContext context)
    {
        if (paused)
        { // If the game is paused
            playerInputs.UI.Disable();
            if (prePauseMenu == "Craft")
            {
                playerInputs.CraftMenu.Enable();
                menuManager.ChangeMenu("Craft");
            }
            else if (prePauseMenu == "Orders")
            {
                playerInputs.OrdersMenu.Enable();
                menuManager.ChangeMenu("Order");
            }
            else
            {
                playerInputs.Player.Enable(); // Enable the normal inputs
                menuManager.ChangeMenu("HUD"); // Change to the HUD
            }
            Time.timeScale = 1; // Resume the game
            paused = false; // The game is no longer paused
        }
        else
        {
            playerInputs.Disable();
            playerInputs.UI.Enable(); // Enable the menu inputs
            prePauseMenu = menuManager.lastMenu;
            menuManager.ChangeMenu("Pause"); // Change to the pause menu
            Time.timeScale = 0; // Pause the game
            paused = true; // The game is currently paused
        }
    }
}
