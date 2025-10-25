using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    [System.Serializable]
    public class Slot
    {
        public string itemName;
        public int count;
        public int maxCount;
        public Sprite icon;

        public Slot()
        {
            itemName = "";
            count = 0;
            maxCount = 64;
        }

        public bool isEmpty
        {
            get 
            {
                if (itemName == "" && count == 0)
                {
                    return true;
                }
                return false;
            }
        }

        public bool canAdd(string ItemName)
        {
            if (this.itemName == ItemName && count < maxCount)
            {
                return true;
            }

            return false;
        }

        public void add(Item item)
        {
            this.itemName = item.data.itemName;
            this.icon = item.data.icon;
            count++;
        }

        public void removeItem()
        {
            if (count > 0)
            {
                count--;
                
                if (count == 0)
                {
                    icon = null;
                    itemName = "";
                }
            }
        }
    }

    public List<Slot> slots = new List<Slot>();
    public Slot selectedSlot = null;


    public Inventory(int numSlots)
    {
        for (int i = 0; i < numSlots; i++)
        {
            Slot slot = new Slot();
            slots.Add(slot);
        }
    }

    public void add(Item item)
    {
        foreach(Slot slot in slots)
        {
            if (slot.itemName == item.data.itemName && slot.canAdd(item.data.itemName))
            {
                slot.add(item);
                return;
            }
        }

        foreach(Slot slot in slots)
        {
            if (slot.itemName == "")
            {
                slot.add(item);
                return;
            }
        }
    }

    public void Add(Item item, int amountToAdd)
    {
        if (item == null || amountToAdd <= 0) return;

        int amountLeft = amountToAdd;

        // 1. Coba isi tumpukan (stack) yang ada terlebih dahulu
        foreach (Slot slot in slots)
        {
            if (amountLeft <= 0) break; // Semua item sudah ditempatkan

            // Cek jika slot ini berisi item yang sama dan belum penuh
            if (!slot.isEmpty && slot.itemName == item.data.itemName && slot.count < slot.maxCount)
            {
                int spaceAvailable = slot.maxCount - slot.count;
                int amountToMove = Mathf.Min(amountLeft, spaceAvailable);

                slot.count += amountToMove;
                amountLeft -= amountToMove;
            }
        }

        // 2. Jika masih ada sisa, cari slot kosong
        if (amountLeft > 0)
        {
            foreach (Slot slot in slots)
            {
                if (amountLeft <= 0) break; // Semua item sudah ditempatkan

                if (slot.isEmpty)
                {
                    // Konfigurasi slot untuk item baru
                    slot.itemName = item.data.itemName;
                    slot.icon = item.data.icon;
                    // (Anda mungkin perlu mengatur maxCount di sini jika berbeda per item)
                    // slot.maxCount = data.maxStack; 

                    int amountToMove = Mathf.Min(amountLeft, slot.maxCount);
                    
                    slot.count = amountToMove; // Set jumlahnya
                    amountLeft -= amountToMove;
                }
            }
        }
        
        // 3. (Opsional) Jika masih ada sisa, berarti inventaris penuh
        if (amountLeft > 0)
        {
            Debug.LogWarning($"Inventaris penuh. Gagal menambahkan {amountLeft}x {item.data.itemName}.");
        }
    }

    // Check if inventory can accommodate the specified amount of items
    public bool CanAdd(ItemData itemData, int amountToAdd)
    {
        if (itemData == null || amountToAdd <= 0) return false;

        int amountLeft = amountToAdd;

        // Check existing stacks first
        foreach (Slot slot in slots)
        {
            if (amountLeft <= 0) break;

            if (!slot.isEmpty && slot.itemName == itemData.itemName && slot.count < slot.maxCount)
            {
                int spaceAvailable = slot.maxCount - slot.count;
                amountLeft -= Mathf.Min(amountLeft, spaceAvailable);
            }
        }

        // Check empty slots
        if (amountLeft > 0)
        {
            foreach (Slot slot in slots)
            {
                if (amountLeft <= 0) break;

                if (slot.isEmpty)
                {
                    amountLeft -= Mathf.Min(amountLeft, slot.maxCount);
                }
            }
        }

        return amountLeft <= 0;
    }

    // Overloaded method to add items directly from ItemData (for dialogue system)
    public void Add(ItemData itemData, int amountToAdd)
    {
        if (itemData == null || amountToAdd <= 0) return;

        int amountLeft = amountToAdd;

        // 1. Coba isi tumpukan (stack) yang ada terlebih dahulu
        foreach (Slot slot in slots)
        {
            if (amountLeft <= 0) break; // Semua item sudah ditempatkan

            // Cek jika slot ini berisi item yang sama dan belum penuh
            if (!slot.isEmpty && slot.itemName == itemData.itemName && slot.count < slot.maxCount)
            {
                int spaceAvailable = slot.maxCount - slot.count;
                int amountToMove = Mathf.Min(amountLeft, spaceAvailable);

                slot.count += amountToMove;
                amountLeft -= amountToMove;
            }
        }

        // 2. Jika masih ada sisa, cari slot kosong
        if (amountLeft > 0)
        {
            foreach (Slot slot in slots)
            {
                if (amountLeft <= 0) break; // Semua item sudah ditempatkan

                if (slot.isEmpty)
                {
                    // Konfigurasi slot untuk item baru
                    slot.itemName = itemData.itemName;
                    slot.icon = itemData.icon;
                    // (Anda mungkin perlu mengatur maxCount di sini jika berbeda per item)
                    // slot.maxCount = data.maxStack; 

                    int amountToMove = Mathf.Min(amountLeft, slot.maxCount);
                    
                    slot.count = amountToMove; // Set jumlahnya
                    amountLeft -= amountToMove;
                }
            }
        }
        
        // 3. (Opsional) Jika masih ada sisa, berarti inventaris penuh
        if (amountLeft > 0)
        {
            Debug.LogWarning($"Inventaris penuh. Gagal menambahkan {amountLeft}x {itemData.itemName}.");
        }
    }

    public void Remove(int index)
    {
        slots[index].removeItem();
    }

    public void Remove(int index, int numToRemove)
    {
        if( slots[index].count >= numToRemove)
        {
            for(int i = 0; i < numToRemove; i++)
            {
                Remove(index);
            }
        }
    }

    public void moveSlot(int fromIndex, int toIndex, Inventory inventory, int numToMove = 1)
{
    // Get references to the slots
    Slot fromSlot = slots[fromIndex];
    Slot toSlot = inventory.slots[toIndex];

    // Do nothing if the origin slot is empty or we're moving to the same slot
    if (fromSlot.isEmpty || fromIndex == toIndex)
    {
        return;
    }
    
    // Check if the destination slot is empty or if we can add the same item to its stack
    if (toSlot.isEmpty || toSlot.canAdd(fromSlot.itemName))
    {
        for(int i = 0; i < numToMove; i++)
        {
            // If the destination was empty, we must copy the item's data over
            if (toSlot.isEmpty)
            {
                toSlot.itemName = fromSlot.itemName;
                toSlot.icon = fromSlot.icon;
            }

            // Add one to the destination count
            toSlot.count++;

            // Remove one from the origin slot
            fromSlot.removeItem();
        }
    }   
}
    public void SelectSlot(int Index)
    {
        if (slots != null && slots.Count > 0)
        {
            selectedSlot = slots[Index];
        } 
    }
}
