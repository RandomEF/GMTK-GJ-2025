using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CraftBench : Interactable
{
    private MenuManager menuManager;
    public Camera playerCamera;
    public Camera benchView;
    public RopeTypes selectedRope = RopeTypes.weak;
    public float progress = 0;
    private InputSystem_Actions inputs;
    public int nextCheckpoint = 0;
    private int checkpointsTouched = 5;
    public int ropeLoops = 0;
    public Transform spawnLocation;

    [Header("Rope Prefabs")]
    public GameObject weak;
    public GameObject basic;
    public GameObject reinforced;
    public GameObject strong;
    public GameObject best;

    [Header("Loops Needed")]
    public int weakNeeded = 3;
    public int basicNeeded = 5;
    public int reinforcedNeeded = 7;
    public int strongNeeded = 10;
    public int bestNeeded = 15;

    [Header("UI")]
    public TMP_Text loopCount;
    public TMP_Text loopRequired;
    public TMP_Dropdown dropdown;

    public enum RopeTypes
    {
        weak,
        basic,
        reinforced,
        strong,
        best
    }
    public void ChangeRope()
    {
        switch (dropdown.value)
        {
            case 0:
                selectedRope = RopeTypes.weak;
                break;
            case 1:
                selectedRope = RopeTypes.basic;
                break;
            case 2:
                selectedRope = RopeTypes.reinforced;
                break;
            case 3:
                selectedRope = RopeTypes.strong;
                break;
            case 4:
                selectedRope = RopeTypes.best;
                break;
            default:
                Debug.LogWarning("selectedRope did not match any known value for the maximum loops, returned 0");
                break;
        }
        loopRequired.text = "Loops Required: " + GetLoopsNeeded().ToString();
    }
    public int GetLoopsNeeded()
    {
        switch (selectedRope)
        {
            case RopeTypes.weak:
                return weakNeeded;
            case RopeTypes.basic:
                return basicNeeded;
            case RopeTypes.reinforced:
                return reinforcedNeeded;
            case RopeTypes.strong:
                return strongNeeded;
            case RopeTypes.best:
                return bestNeeded;
            default:
                Debug.LogWarning("selectedRope did not match any known value for the maximum loops, returned 0");
                return 0;
        }
    }
    public GameObject GetNewRope()
    {
        switch (selectedRope)
        {
            case RopeTypes.weak:
                return weak;
            case RopeTypes.basic:
                return basic;
            case RopeTypes.reinforced:
                return reinforced;
            case RopeTypes.strong:
                return strong;
            case RopeTypes.best:
                return best;
            default:
                Debug.LogWarning("selectedRope did not match any known prefab, returned weak version");
                return weak;
        }
    }
    /*
    change cameras
    get player to move sewing needle on a track in a loop
    have checkpoints to make sure that the player is following the loop
    change input system to a separate ui map
    */

    void Start()
    {
        gameObject.tag = "Interact"; // Mark the current object as an interactable
        menuManager = MenuManager.Instance;
        inputs = PlayerManager.Instance.inputs;
        inputs.CraftMenu.Exit.started += LeaveCrafting;
    }

    override public void Interact(GameObject player)
    {
        Debug.Log("Switched to bench");
        playerCamera.enabled = false;
        benchView.enabled = true;
        inputs.Player.Disable();
        inputs.CraftMenu.Enable();
        menuManager.ChangeMenu("Craft");
    }

    public void LeaveCrafting(InputAction.CallbackContext context)
    {
        Debug.Log("Leaving bench");
        playerCamera.enabled = true;
        benchView.enabled = false;
        inputs.Player.Enable();
        inputs.CraftMenu.Disable();
        menuManager.ChangeMenu("HUD");
    }

    public void EnteredCheckpoint(int checkpoint)
    {
        Debug.Log($"Received checkpoint information from number {checkpoint}");
        if (checkpoint == nextCheckpoint)
        {
            checkpointsTouched += 1;
            nextCheckpoint += 1;
            if (nextCheckpoint == 6)
            {
                nextCheckpoint = 0;
            }
        }
        if (nextCheckpoint == 1 && checkpointsTouched == 6)
        {
            ropeLoops += 1;
            loopCount.text = "Loops completed: " + ropeLoops.ToString();
            checkpointsTouched = 0;
            if (ropeLoops == GetLoopsNeeded())
            {
                Instantiate(GetNewRope(), spawnLocation.position, spawnLocation.rotation);
                ropeLoops = 0;
                loopCount.text = "Loops completed: " + ropeLoops.ToString();
            }
        }
    }
}
