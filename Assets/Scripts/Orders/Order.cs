using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Order : MonoBehaviour
{
    private float time = 50;
    public OrdersMenu.Items item;
    public int quantity;
    public PlayerManager manager = PlayerManager.Instance;
    [SerializeField] private RawImage image;
    [SerializeField] private Image fillBar;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text quantityText;

    private void Start()
    {
        image.GetComponent<RawImage>().texture = OrdersMenu.Instance.GetItemImage(item);
        itemName.text = OrdersMenu.GetItemName(item);
        quantityText.text = quantity.ToString();
        manager = PlayerManager.Instance;
    }

    void Update()
    {
        time -= Time.deltaTime;
        if (time < 0)
        {
            manager.AddMoney(-5);
            Destroy(gameObject);
        }
        fillBar.fillAmount = time / 50;
        progressText.text = time.ToString() + "s";
    }
    public void SubmitItem()
    {
        quantity -= 1;
        quantityText.text = quantity.ToString();
        if (quantity <= 0)
        {
            manager.AddMoney(20);
            OrdersMenu.Instance.orders.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}
