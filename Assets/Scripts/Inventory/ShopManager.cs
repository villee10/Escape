using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public TextMeshProUGUI goldText;
    
    // Vi skapar en referens till spelarens inventory
    private PlayerInventory playerInv;

    void Start()
    {
        // Hittar spelaren automatiskt via taggen "Player"
        playerInv = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        UpdateGoldUI();
    }

    public void BuyDynamicItem(ShopItem item)
    {
        if (item == null || playerInv == null) return;

        if (playerInv.gold >= item.price)
        {
            // Vi försöker lägga till föremålet först
            bool wasAdded = playerInv.AddItem(item.itemName, item.itemSprite);

            if (wasAdded)
            {
                // Bara om det fick plats drar vi pengarna!
                playerInv.gold -= item.price;
                UpdateGoldUI();
                Debug.Log("Köpte " + item.itemName);
            }
            else
            {
                Debug.Log("Köp avbrutet: Inventoryt är fullt!");
            }
        }
        else
        {
            Debug.Log("För fattig!");
        }
    }

    void Update()
    {
        UpdateGoldUI();
    }
    
    void UpdateGoldUI()
    {
        // kollar om playerInv finns, sen hämtar vi guldet därifrån!
        if(goldText != null && playerInv != null) 
        {
            goldText.text = playerInv.gold.ToString() + " G";
        }
    }
}