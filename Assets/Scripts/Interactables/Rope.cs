using UnityEngine;

public class Rope : Interactable
{
    public float ropeIntegrity = 1000;
    public float ropeWeakness => 10;
    public bool isBeingUsed = false;

    override public void Interact(GameObject player)
    {
        player.GetComponent<PlayerInteraction>().EquipItem(transform.gameObject);
    }
    private void Update()
    {
        if (isBeingUsed)
        {
            ropeIntegrity -= ropeWeakness * Time.deltaTime;
        }
        if (ropeIntegrity < 0)
        {
            transform.parent.parent.GetComponent<Machine>().RopeBroke();
        }
    }
}
