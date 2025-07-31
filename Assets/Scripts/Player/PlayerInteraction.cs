using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    Camera playerHead;
    public Transform itemHold;
    public GameObject heldItem;
    public bool isHoldingItem = false;
    public PlayerManager manager;
    private InputSystem_Actions playerInputs;
    private LayerMask notPlayerLayer;

    private void Start()
    {
        manager = PlayerManager.Instance; // Get the game manager
        playerInputs = manager.inputs; // Get the player's inputs
        playerHead = Camera.main; // Get the camera
        playerInputs.Player.Interact.started += Interact; // Run Interact() when the player presses the interact key
        playerInputs.Player.DropRope.started += Drop;
        Debug.Log("Registered interaction inputs");
        notPlayerLayer = ~LayerMask.GetMask("Player"); // Get the layer that the player isn't on
    }
    public void Interact(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        Debug.DrawLine(playerHead.transform.position, playerHead.transform.position + playerHead.transform.rotation * Vector3.forward, Color.red);
        Debug.Log("Pressed Interact");
        if (Physics.Raycast(origin: playerHead.transform.position, direction: playerHead.transform.rotation * Vector3.forward, hitInfo: out hit, layerMask: notPlayerLayer, maxDistance: 10f))
        { // If the raycast collides with something
            Debug.Log("Raycast hit " + hit.collider.gameObject.name);
            if (hit.collider.gameObject.tag == "Interact")
            { // If the hit object is interactable
                InteractHit(hit.collider.gameObject); // Interact with it
            }
        }
    }
    private void Update() {
        Debug.DrawLine(playerHead.transform.position, playerHead.transform.position + playerHead.transform.rotation * Vector3.forward, Color.red);
    }
    private void InteractHit(GameObject hit)
    {
        hit.GetComponent<Interactable>().Interact(transform.gameObject); // Call Interact() on the interactable
    }
    public void EquipItem(GameObject item)
    {
        if (isHoldingItem)
        {
            heldItem.transform.SetParent(null, true); // Free the weapon from the player
            heldItem.GetComponent<Rigidbody>().isKinematic = false; // Allow the physics system to push it
            heldItem.GetComponent<Rigidbody>().detectCollisions = true; // Allow it to detect collisions
        }
        item.transform.SetParent(playerHead.transform, true); // Parent the weapon to the camera
        item.GetComponent<Rigidbody>().isKinematic = true; // Disable the physics system from interacting with it
        item.GetComponent<Rigidbody>().detectCollisions = false; // Don't allow it to detect collisions
        item.transform.position = itemHold.position; // Put the weapon in the held position
        item.transform.rotation = itemHold.rotation; // Put the weapon in the held position
        heldItem = item;
        isHoldingItem = true;
    }
    public void Drop(InputAction.CallbackContext inputType)
    {
        heldItem.transform.SetParent(null, true); // Free the weapon from the player
        heldItem.GetComponent<Rigidbody>().isKinematic = false; // Allow the physics system to push it
        heldItem.GetComponent<Rigidbody>().detectCollisions = true; // Allow it to detect collisions
        isHoldingItem = false;
        heldItem = null;
    }
}
