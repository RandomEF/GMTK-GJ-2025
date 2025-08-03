using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public GameObject player;
    public PlayerLook lookScript;
    private MenuManager menuManager;
    public InputSystem_Actions inputs;
    public Dictionary<OrdersMenu.Items, int> producedItems = new Dictionary<OrdersMenu.Items, int>();
    public TMP_Text moneyText;
    public int money = 0;
    public TMP_Text gameOverText;
    public GameObject gameOverButton;
    public bool isPlaying = false;
    private OrdersMenu ordersMenu;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Sets the player manager to this object
        }
        inputs = new InputSystem_Actions(); // make a new set of player inputs
        inputs.UI.Enable();
    }

    void Start()
    {
        menuManager = MenuManager.Instance; // Get the menu manager
        ordersMenu = OrdersMenu.Instance;
        DontDestroyOnLoad(gameObject); // Since the game will start in the title screen, it makes sure that the player, manager, and all things attached are loaded and maintained across all scene loads
    }

    public void SetCursorMode(bool state)
    {
        lookScript.SetCursorLock(state);
    }

    public void StartGame()
    {
        menuManager.ChangeMenu("HUD");
        inputs.UI.Disable();
        inputs.Player.Enable();
        isPlaying = true;
        ordersMenu.MakeOrder();
        StartCoroutine(ordersMenu.Timer());
    }

    public void AddMoney(int amount)
    {
        money += amount;
        moneyText.text = "Money: " + money.ToString();
        if (money < 0)
        {
            GameOver();
        }
    }
    public void AddItem(OrdersMenu.Items item, int amount)
    {
        try
        {
            producedItems[item] += amount;
        }
        catch (System.Collections.Generic.KeyNotFoundException)
        {
            producedItems[item] = 1;
        }
        ordersMenu.CheckItems();
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        gameOverText.text = "Game Over\n";
        gameOverButton.SetActive(true);
        SetCursorMode(false);
        inputs.Player.Disable();
        inputs.UI.Enable();
        isPlaying = false;
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
