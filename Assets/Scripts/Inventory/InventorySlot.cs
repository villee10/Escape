using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Viktigt för klick!

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public Image iconImage; 
    public bool isFull = false;
    public string itemName; 

    public void SetItem(Sprite itemSprite, string name) 
    {
        itemName = name; 
        iconImage.sprite = itemSprite;
    
        // De här två raderna ser till att bilden faktiskt SYNS
        iconImage.enabled = true; 
        iconImage.color = Color.white; 
    
        isFull = true;
    }

    public void ClearSlot()
    {
        isFull = false;
        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false; // Döljer bilden helt
        }
    }

    // Denna körs automatiskt av Unity när du klickar på slotten
    public void OnPointerClick(PointerEventData eventData)
    {
        // Hitta PlayerInventory som sitter på din MainPlayer och säg till att vi klickat
        PlayerInventory inventory = Object.FindFirstObjectByType<PlayerInventory>();
        if (inventory != null)
        {
            inventory.HandleSlotClick(this);
        }
    }
}