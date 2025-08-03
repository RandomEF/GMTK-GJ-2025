using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class OrdersMenu : Interactable
{
    private InputSystem_Actions inputs;
    private MenuManager menuManager;
    private PlayerManager manager;
    public static OrdersMenu Instance;
    public GameObject orderPrefab;
    public GameObject content;
    public List<GameObject> orders;

    [Header("Item Images")]
    public Texture2D spring;
    public Texture2D solenoid;
    public Texture2D mobius;
    public Texture2D hula;
    public Texture2D racing;

    public enum Items
    {
        spring,
        solenoid,
        mobius,
        hula,
        racing
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Sets the player manager to this object
        }
    }

    public Texture2D GetItemImage(Items item)
    {
        switch (item)
        {
            case Items.spring:
                return spring;
            case Items.solenoid:
                return solenoid;
            case Items.mobius:
                return mobius;
            case Items.hula:
                return hula;
            case Items.racing:
                return racing;
            default:
                throw new System.Exception($"{item} not a valid item enum");
        }
    }
    static public string GetItemName(Items item)
    {
        switch (item)
        {
            case Items.spring:
                return "Spring";
            case Items.solenoid:
                return "Solenoid";
            case Items.mobius:
                return "Mobius Strip";
            case Items.hula:
                return "Hula Hoop";
            case Items.racing:
                return "Racing Track";
            default:
                throw new System.Exception($"{item} not a valid item enum");
        }
    }

    void Start()
    {
        gameObject.tag = "Interact"; // Mark the current object as an interactable
        menuManager = MenuManager.Instance;
        manager = PlayerManager.Instance;
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
    private void Update()
    {
        // if (manager.isPlaying && orders.Count == 0)
        // {
        //     StartCoroutine(Timer());
        // }
        
    }
    public void CheckItems()
    {
        foreach (Items item in manager.producedItems.Keys.ToList())
        {
            foreach (GameObject order in orders)
            {
                if (order.GetComponent<Order>().item == item && manager.producedItems[item] > 0)
                {
                    order.GetComponent<Order>().SubmitItem();
                    manager.AddItem(item, -1);
                    //manager.producedItems[item] -= 1;
                }
            }
        }
    }
    public IEnumerator Timer()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(15);
            MakeOrder();
            //StartCoroutine(Timer());
        }
    }
    public void MakeOrder()
    {
        GameObject order = Instantiate(orderPrefab, content.transform); // Make a new object
        orders.Add(order);

        order.GetComponent<Order>().item = (Items)Random.Range(0, 5);
        order.GetComponent<Order>().quantity = Random.Range(1, 4);
    }
}
