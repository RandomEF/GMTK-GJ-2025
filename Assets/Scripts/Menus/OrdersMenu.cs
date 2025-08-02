using UnityEngine;
using UnityEngine.InputSystem;

public class OrdersMenu : Interactable
{
    private InputSystem_Actions inputs;
    private MenuManager menuManager;

    [Header("Item Images")]
    public Texture2D spring;

    public enum Items
    {
        spring,
        solenoid,
        mobius,
        hula,
        racing
    }

    void Start()
    {
        gameObject.tag = "Interact"; // Mark the current object as an interactable
        menuManager = MenuManager.Instance;
        inputs = PlayerManager.Instance.inputs;
        inputs.OrdersMenu.Exit.started += LeaveOrders;
    }

    override public void Interact(GameObject player)
    {
        Debug.Log("Switched to orders");
        inputs.Player.Disable();
        inputs.CraftMenu.Enable();
        menuManager.ChangeMenu("Orders");
    }

    public void LeaveOrders(InputAction.CallbackContext context)
    {
        Debug.Log("Leaving orders");
        inputs.Player.Enable();
        inputs.CraftMenu.Disable();
        menuManager.ChangeMenu("HUD");
    }
}
