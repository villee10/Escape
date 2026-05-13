using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    [Header("Stats & Data")]
    public int gold = 1000;
    public List<string> items = new List<string>();
    
    [Header("UI Referenser")]
    public TextMeshProUGUI inventoryGoldText;
    public GameObject inventoryPanel; 
   
    [Header("Inventory Slots")]
    public List<InventorySlot> uiSlots = new List<InventorySlot>();
    
    [Header("Hotbar System")]
    public RectTransform selectionHighlight; 
    public List<RectTransform> hotbarSlotTransforms = new List<RectTransform>(); 
    
    
    [Header("Hand System")]
    public Transform handTransform; // Dra in ditt "Hand"-objekt här
    private GameObject currentItemInHand; // Håller koll på vad vi håller just nu
    
    
    
    public GameObject hotbarParent; 
    private int selectedSlotIndex = 0;
    
    private bool isOpen = false;

    void Start()
    {
        // 1. Göm stora inventory-panelen som vanligt
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    
        // 2. Hantera Hotbaren baserat på om vi nått en checkpoint
        if (hotbarParent != null)
        {
            // Om jag HAR nått en checkpoint (t.ex. vid respawn), visa hotbaren.
            // Annars (när spelet precis startat), håll den gömd.
            hotbarParent.SetActive(CheckpointManager.hasReachedCheckpoint);
        }

        // 3. Sätt första slotten som vald
        ChangeSelectedSlot(0);
    }

    void Update()
    {
        // Om hotbaren är gömd, gör ingenting av det nedanför. 
        // Det förhindrar att man råkar byta föremål innan man nått bron.
        if (hotbarParent != null && !hotbarParent.activeSelf) return;

        if (inventoryGoldText != null)
        {
            inventoryGoldText.text = "Guld: " + gold.ToString();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }

        // Dessa körs nu bara om hotbaren faktiskt syns på skärmen
        if (Input.GetKeyDown(KeyCode.Alpha1)) ChangeSelectedSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) ChangeSelectedSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) ChangeSelectedSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) ChangeSelectedSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) ChangeSelectedSlot(4);
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            Time.timeScale = 0f; 
        }
        else
        {
            Time.timeScale = 1f; 
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public bool AddItem(string itemName, Sprite itemIcon)
    {
        // Vi skapar en lista där vi kollar stora väskan FÖRST
        List<InventorySlot> allAvailableSlots = new List<InventorySlot>();
        allAvailableSlots.AddRange(uiSlots);         // Kolla stora väskan först
        allAvailableSlots.AddRange(GetHotbarSlots()); // Om väskan är full, kolla hotbaren

        foreach (InventorySlot slot in allAvailableSlots)
        {
            if (!slot.isFull)
            {
                items.Add(itemName);
                slot.SetItem(itemIcon, itemName);
                Debug.Log(itemName + " lades i inventoryt!");
                return true; 
            }
        }

        Debug.Log("Inventoryt är fullt!");
        return false; 
    }

    public bool CanAfford(int price)
    {
        return gold >= price;
    }
    
    void ChangeSelectedSlot(int newIndex)
    {
        selectedSlotIndex = newIndex;

        if (selectionHighlight != null && hotbarSlotTransforms.Count > newIndex)
        {
            selectionHighlight.position = hotbarSlotTransforms[newIndex].position;
        }

        // Uppdatera vad vi håller i handen
        UpdateHandItem();
    }

    void UpdateHandItem()
    {
        // 1. Kolla att vi faktiskt har en hand-referens
        if (handTransform == null) 
        {
            Debug.LogWarning("Hand Transform saknas på MainPlayer!");
            return;
        }

        // 2. Ta bort gammalt föremål
        if (currentItemInHand != null) Destroy(currentItemInHand);

        // 3. Säkerhetskoll för hotbar-listan
        if (hotbarSlotTransforms == null || hotbarSlotTransforms.Count <= selectedSlotIndex) return;

        InventorySlot selectedSlot = hotbarSlotTransforms[selectedSlotIndex].GetComponent<InventorySlot>();

        // 4. Om slotten finns och har ett föremål
        if (selectedSlot != null && selectedSlot.isFull && !string.IsNullOrEmpty(selectedSlot.itemName))
        {
            GameObject prefab = Resources.Load<GameObject>(selectedSlot.itemName); 
    
            if(prefab != null) 
            {
                currentItemInHand = Instantiate(prefab, handTransform);
                currentItemInHand.transform.localPosition = Vector3.zero;
                currentItemInHand.transform.localRotation = Quaternion.identity;
            }
        }
    }

    // --- NYA FUNKTIONER FÖR KLICK OCH FLYTT ---

    public void HandleSlotClick(InventorySlot clickedSlot)
    {
        if (!clickedSlot.isFull) return;

        bool isMainInventory = uiSlots.Contains(clickedSlot);

        if (isMainInventory)
            MoveItem(clickedSlot, GetHotbarSlots());
        else
            MoveItem(clickedSlot, uiSlots);

        UpdateHandItem();
    }

    private void MoveItem(InventorySlot source, List<InventorySlot> targetList)
    {
        foreach (InventorySlot target in targetList)
        {
            if (!target.isFull)
            {
                target.SetItem(source.iconImage.sprite, source.itemName);
                source.ClearSlot();
                return;
            }
        }
    }

    private List<InventorySlot> GetHotbarSlots()
    {
        List<InventorySlot> slots = new List<InventorySlot>();
        foreach (RectTransform rect in hotbarSlotTransforms)
        {
            if (rect != null)
            {
                InventorySlot s = rect.GetComponent<InventorySlot>();
                if (s != null) slots.Add(s);
            }
        }
        return slots;
    }
}