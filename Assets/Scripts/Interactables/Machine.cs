using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Machine : Interactable
{
    public float progress = 0f;
    public Transform ropeSlot;
    public GameObject heldRope;
    private bool isWorking = false;
    public float rate = 1f;
    private PlayerManager manager;
    public OrdersMenu.Items generatedItem;
    public Animator animator;
    public SkinnedMeshRenderer machineRenderer;
    public Color notWorkingColor;
    public Color workingColor;
    public Color emptyColor;

    [Header("GUI Variables")]
    public RawImage itemImage;
    public Image progressBar;
    public TMP_Text progressPercent;
    public TMP_Text ropeName;
    public TMP_Text currentState;
    public TMP_Text itemName;
    public TMP_Text ropeState;

    void Start()
    {
        gameObject.tag = "Interact"; // Mark the current object as an interactable
        manager = PlayerManager.Instance;
        animator.speed = 0;
        machineRenderer.materials[1].color = notWorkingColor;
        machineRenderer.materials[1].SetColor("_EmissionColor", notWorkingColor);
        itemName.text = OrdersMenu.GetItemName(generatedItem);
        itemImage.texture = OrdersMenu.Instance.GetItemImage(generatedItem);
    }

    override public void Interact(GameObject player)
    { // TODO This if can be optimised by changing condition orders and maybe moving some things into functions
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
                interactionScript.heldItem.transform.SetParent(interactionScript.itemHold.transform);
                interactionScript.heldItem.GetComponent<Rope>().isBeingUsed = false;
                animator.speed = 1;
                machineRenderer.materials[1].color = workingColor;
                machineRenderer.materials[2].color = heldRope.GetComponent<Renderer>().material.color;
                machineRenderer.materials[1].SetColor("_EmissionColor", workingColor);

                currentState.text = "STATE : Working";
                ropeName.text = heldRope.GetComponent<Rope>().ropeName;
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
                machineRenderer.materials[2].color = heldRope.GetComponent<Renderer>().material.color;
                machineRenderer.materials[1].SetColor("_EmissionColor", workingColor);

                currentState.text = "STATE : Working";
                ropeName.text = heldRope.GetComponent<Rope>().ropeName;
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
                machineRenderer.materials[2].color = emptyColor;
                machineRenderer.materials[1].SetColor("_EmissionColor", notWorkingColor);

                ropeState.text = "Rope Health: No rope";
                currentState.text = "STATE : No Rope";
                ropeName.text = "Empty";
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
            progressBar.fillAmount = progress / 100;
            progressPercent.text = progress.ToString() + "%";
            ropeState.text = "Rope Health: " + heldRope.GetComponent<Rope>().ropeIntegrity.ToString();
        }
        if (progress >= 100)
        {
            //Instantiate(producedItem, position: productionLocation.position, rotation: productionLocation.rotation);
            manager.AddItem(generatedItem, 1);
            //Debug.Log(manager.producedItems[generatedItem]);
            progress -= 100;
        }
    }
    public void RopeBroke() // Called by the broken rope
    {
        Destroy(heldRope);
        heldRope = null;
        animator.speed = 0;
        machineRenderer.materials[1].color = notWorkingColor;
        machineRenderer.materials[2].color = emptyColor;
        machineRenderer.materials[1].SetColor("_EmissionColor", notWorkingColor);

        ropeState.text = "Rope Health: No rope";
        currentState.text = "STATE : Rope Broke";
        ropeName.text = "Empty";
        isWorking = false;
    }
}
