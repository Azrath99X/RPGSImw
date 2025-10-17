using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toolbar_UI : MonoBehaviour
{
    // List untuk menyimpan slot toolbar yang akan diatur di Inspector Unity
    [SerializeField] private List<Slot_Ui> toolbarSlots = new List<Slot_Ui>();

    // Slot yang sedang dipilih saat ini
    private Slot_Ui selectedSlot;

    public void SelectSlot(Slot_Ui slot)
    {
        SelectSlot(slot.slotID);
    }

    // Fungsi untuk memilih slot berdasarkan indeks
    public void SelectSlot(int index)
    {
        // Jika ada slot yang sebelumnya dipilih, matikan highlight-nya
        if (selectedSlot != null)
        {
            selectedSlot.SetHighlight(false);
        }

        // Pilih slot baru berdasarkan indeks dan aktifkan highlight-nya
        selectedSlot = toolbarSlots[index];
        selectedSlot.SetHighlight(true);

        GameManager.Instance.player.inventory.toolbar.SelectSlot(index);
    }

    // Fungsi untuk memeriksa input tombol angka (1-4) untuk memilih slot toolbar
    private void CheckAlphanumericKeys()
    {
        // Jika tombol angka ditekan, panggil fungsi SelectSlot dengan indeks yang sesuai
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SelectSlot(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SelectSlot(5);
        if (Input.GetKeyDown(KeyCode.Alpha7)) SelectSlot(6);
    }

    // Fungsi yang dipanggil setiap frame untuk memeriksa input pengguna
    void Update()
    {
        CheckAlphanumericKeys(); // Periksa input tombol angka
    }

    // Fungsi yang dipanggil saat script diinisialisasi
    private void Start()
    {
        SelectSlot(0); // Secara default, pilih slot pertama
    }
}