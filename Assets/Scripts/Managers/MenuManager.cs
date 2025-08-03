using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    public PlayerManager manager;

    public Canvas main;
    public Canvas options;
    public Canvas pause;
    public Canvas hud;
    public Canvas orders;
    public Canvas craft;

    public string currentMenu = "";
    public string lastMenu = "";
    [SerializeField] public Dictionary<string, (Canvas, bool)> currentCanvas;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Sets the menu manager to this object
        }
        currentCanvas = new Dictionary<string, (Canvas, bool)>()
        { // Assign the canvas and the interactability of each canvas
            { "Main", (main, true) },
            { "Pause", (pause, true) },
            { "HUD", (hud, false) },
            { "Options", (options, true) },
            { "Orders", (orders, true)},
            { "Craft", (craft, true)}
        };
    }

    void Start()
    {
        manager = PlayerManager.Instance; // Grab the player manager
        //FetchCanvases(); // Fetch all the canvases
        DisableAll(); // Disable every canvas
        ChangeMenu("Main"); // Change the canvas
    }/*
    public void FetchCanvases()
    { // Attempt to fetch a reference to every canvas
        foreach (string canvas in currentCanvas.Keys.ToList())
        { // get a copy of the keys
            if (currentCanvas[canvas].Item1 == null)
            { // If there is no canvas object
                try
                {
                    currentCanvas[canvas] = (
                        GameObject.Find(canvas).GetComponent<Canvas>(),
                        currentCanvas[canvas].Item2
                    ); // try to find the canvas and assign it if so
                }
                catch (NullReferenceException e)
                { // If it doesn't exist, log the error
                    Debug.LogError("Caught exception: " + e.Message);
                    continue;
                }
            }
        }
        Debug.Log("Fetched all canvases");
    }*/

    public void DisableAll()
    {
        foreach ((Canvas, bool) canvas in currentCanvas.Values.ToList())
        { // get a copy of the values
            if (canvas.Item1 != null)
            { // If the canvas exists
                canvas.Item1.GetComponent<CanvasGroup>().interactable = false; // Make the player unable to interact with the menu
                canvas.Item1.GetComponent<CanvasGroup>().blocksRaycasts = false; // Make it invisible to all inputs
                canvas.Item1.GetComponent<CanvasGroup>().alpha = 0; // Make it invisible to the player
            }
        }
        Debug.Log("Disabled all canvases");
    }

    public void ChangeMenu(string menu)
    {
        if (!currentCanvas.Keys.Contains(menu)){ // If the menu is not in the dictionary keys
            Debug.LogError($"The menu '{menu}' is not in the registered list of accepted menus");
        }
        if (currentMenu != "" && currentCanvas[currentMenu].Item1 != null)
        { // Disable the current menu if it is exists
            currentCanvas[currentMenu].Item1.GetComponent<CanvasGroup>().interactable = false; // Make the player unable to interact with the menu
            currentCanvas[currentMenu].Item1.GetComponent<CanvasGroup>().blocksRaycasts = false; // Make it invisible to all inputs
            currentCanvas[currentMenu].Item1.GetComponent<CanvasGroup>().alpha = 0; // Make it invisible to the player
            lastMenu = currentMenu; // Sets the previous menu to the current one
        } // Enable the new canvas
        currentCanvas[menu].Item1.GetComponent<CanvasGroup>().interactable = currentCanvas[menu].Item2; // Make it interactable if it is supposed to be
        currentCanvas[menu].Item1.GetComponent<CanvasGroup>().blocksRaycasts = currentCanvas[menu].Item2; // Make it possible for inputs to sense it if you should be able to interact with it
        currentCanvas[menu].Item1.GetComponent<CanvasGroup>().alpha = 1; // Make it visible to the player
        currentMenu = menu; // Changes the current menu
        Debug.Log($"Changed menu to '{menu}'");
        manager.SetCursorMode(!currentCanvas[menu].Item2);
    }

    public void GoBack()
    {
        if (lastMenu != "" && currentCanvas[lastMenu].Item1 != null)
        {
            ChangeMenu(lastMenu); // change to the previous menu
        }
        else
        {
            Debug.LogWarning("No back menu option set!");
        }
    }

    public bool GetCurrentCanvasInteractability()
    {
        return currentCanvas[currentMenu].Item2;
    }
}
