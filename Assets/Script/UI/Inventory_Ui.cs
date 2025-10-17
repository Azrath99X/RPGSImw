using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Define the roles this UI panel can have.
public enum PanelType { Inventory, Toolbar }

public class Inventory_Ui : MonoBehaviour
{

    [Header("UI References")]
    public string inventoryName;
    public List<Slot_Ui> slots = new List<Slot_Ui>();
    

    [SerializeField] private Canvas canvas;

    private Inventory inventory;

    private void Awake()
    {
        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
        }
    }

    void Start()
    {
        inventory = GameManager.Instance.player.inventory.GetInventoryByName(inventoryName);
        setupSlots();
        Refresh();

    }

    public void setupSlots()
    {
        int counter = 0;
        
        foreach(Slot_Ui slot in slots)
        {
            slot.slotID = counter;
            counter++;
            slot.inventory = inventory;
        }
    }
    
    public void Refresh()
    {

        for (int i = 0; i < slots.Count; i++)
        {
            if (i < inventory.slots.Count && !string.IsNullOrEmpty(inventory.slots[i].itemName))
            {
                slots[i].setItem(inventory.slots[i]);
            }
            else
            {
                slots[i].setEmpty();
            }
        }
    }

    public void Remove()
    {
        if (UIManager.draggedSlot == null) return;
        
        Inventory sourceInventory = inventory;

        Item itemToDrop = GameManager.Instance.itemManager.GetItem(sourceInventory.slots[UIManager.draggedSlot.slotID].itemName);
        if (itemToDrop == null) return;

        if (UIManager.dragAll)
        {
            int count = sourceInventory.slots[UIManager.draggedSlot.slotID].count;
            GameManager.Instance.player.dropItem(itemToDrop, count);
            sourceInventory.Remove(UIManager.draggedSlot.slotID, count);
        }
        else
        {
            GameManager.Instance.player.dropItem(itemToDrop);
            sourceInventory.Remove(UIManager.draggedSlot.slotID);
        }

        Refresh();
    }
    
    public void beginSlotDrag(Slot_Ui slot)
    {
        if (slot == null || slot.itemIcon.sprite == null) return;
        
        UIManager.draggedSlot = slot;
        UIManager.draggedImage = Instantiate(UIManager.draggedSlot.itemIcon, canvas.transform);
        UIManager.draggedImage.raycastTarget = false;
        UIManager.draggedImage.rectTransform.sizeDelta = new Vector2(50, 50);

        MoveToMousePos(UIManager.draggedImage.gameObject);
    }

    public void slotDrag()
    {
        if (UIManager.draggedImage == null) return;
        
        MoveToMousePos(UIManager.draggedImage.gameObject);
    }

    public void endSlotDrag()
    {
        if (UIManager.draggedImage != null)
        {
            Destroy(UIManager.draggedImage.gameObject);
        }
        UIManager.draggedSlot = null;
    }

    public void slotDrop(Slot_Ui slot)
    {
        if (UIManager.dragAll)
        {
            UIManager.draggedSlot.inventory.moveSlot(UIManager.draggedSlot.slotID, slot.slotID, slot.inventory) ;
        }
        else
        {
            UIManager.draggedSlot.inventory.moveSlot(UIManager.draggedSlot.slotID, slot.slotID, slot.inventory,
            UIManager.draggedSlot.inventory.slots[UIManager.draggedSlot.slotID].count) ;
        }
        GameManager.Instance.uiManager.RefreshAll();
    }

    private void MoveToMousePos(GameObject toMove)
    {
        if (canvas != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                Input.mousePosition,
                canvas.worldCamera,
                out Vector2 localPoint
            );
            toMove.transform.position = canvas.transform.TransformPoint(localPoint);
        }
    }
}