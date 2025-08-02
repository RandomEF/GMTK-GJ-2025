using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class Machine : Interactable
{
    public float progress = 0f;
    public Transform ropeSlot;
    public GameObject heldRope;
    bool isWorking = false;
    public float rate = 1f;
    private PlayerManager manager;
    public OrdersMenu.Items generatedItem;
    public Animator animator;
    public SkinnedMeshRenderer machineRenderer;
    public Color notWorkingColor;
    public Color workingColor;

    void Start()
    {
        gameObject.tag = "Interact"; // Mark the current object as an interactable
        manager = PlayerManager.Instance;
        animator.speed = 0;
        machineRenderer.materials[1].color = notWorkingColor;
        machineRenderer.materials[1].SetColor("_EmissionColor", notWorkingColor);
    }

    override public void Interact(GameObject player)
    {
        /*
        4 cases:
            Player no rope, machine no rope
            Player holding rope, machine no rope
            Player holding rope, machine rope
            Player no rope, machine rope
        If player is holding rope:
            take rope from player
            tell rope to go to placement slot
            set state to working
        */
        Debug.Log("Machine received interaction");
        PlayerInteraction interactionScript = player.GetComponent<PlayerInteraction>();
        if (interactionScript.heldItem)
        {
            if (isWorking) // Player rope, machine rope
            {
                GameObject tempRope = heldRope;
                heldRope = interactionScript.heldItem;
                interactionScript.heldItem = tempRope;
                heldRope.transform.position = ropeSlot.position;
                heldRope.transform.SetParent(ropeSlot.transform);
                heldRope.GetComponent<Rope>().isBeingUsed = true;
                interactionScript.heldItem.transform.position = interactionScript.itemHold.position;
                interactionScript.heldItem.transform.rotation = interactionScript.itemHold.rotation;
                interactionScript.heldItem.GetComponent<Rope>().isBeingUsed = false;
                interactionScript.heldItem.transform.SetParent(interactionScript.itemHold.transform);
                animator.speed = 1;
                machineRenderer.materials[1].color = workingColor;
                machineRenderer.materials[1].SetColor("_EmissionColor", workingColor);
                isWorking = true;
                Debug.Log("Swapped ropes");
            }
            else // Player rope, machine no rope
            {
                heldRope = interactionScript.heldItem;
                heldRope.transform.position = ropeSlot.position;
                heldRope.transform.SetParent(ropeSlot.transform);
                interactionScript.isHoldingItem = false;
                interactionScript.heldItem = null;
                heldRope.GetComponent<Rope>().isBeingUsed = true;
                animator.speed = 1;
                machineRenderer.materials[1].color = workingColor;
                machineRenderer.materials[1].SetColor("_EmissionColor", workingColor);
                isWorking = true;
                Debug.Log("Using rope");
            }
        }
        else
        {
            if (isWorking) // Player no rope, machine rope
            {
                interactionScript.heldItem = heldRope;
                heldRope = null;
                interactionScript.heldItem.transform.position = interactionScript.itemHold.position;
                interactionScript.heldItem.transform.rotation = interactionScript.itemHold.rotation;
                interactionScript.heldItem.transform.SetParent(interactionScript.itemHold.transform);
                interactionScript.heldItem.GetComponent<Rope>().isBeingUsed = false;
                interactionScript.isHoldingItem = true;
                animator.speed = 0;
                machineRenderer.materials[1].color = notWorkingColor;
                machineRenderer.materials[1].SetColor("_EmissionColor", notWorkingColor);
                isWorking = false;
                Debug.Log("Lost rope");
            }
        }
    }
    void Update()
    {
        if (isWorking)
        {
            progress += Time.deltaTime * rate;
        }
        if (progress >= 100)
        {
            //Instantiate(producedItem, position: productionLocation.position, rotation: productionLocation.rotation);
            try
            {
                manager.producedItems[generatedItem] += 1;
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                manager.producedItems[generatedItem] = 1;
            }
            Debug.Log(manager.producedItems[generatedItem]);
            progress -= 100;
        }
    }
    public void RopeBroke() // Called by the broken rope
    {
        Destroy(heldRope);
        heldRope = null;
        animator.speed = 0;
        machineRenderer.materials[1].color = notWorkingColor;
        machineRenderer.materials[1].SetColor("_EmissionColor", notWorkingColor);
        isWorking = false;
    }
}
