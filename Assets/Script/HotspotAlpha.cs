// File: HotspotAlpha.cs
using UnityEngine;
using UnityEngine.UI; // Diperlukan untuk Image
using UnityEngine.EventSystems; // Diperlukan untuk event Select/Deselect

[RequireComponent(typeof(Image))]
public class HotspotAlpha : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private Image hotspotImage;
    private Color visibleColor;
    private Color invisibleColor;

    void Awake()
    {
        hotspotImage = GetComponent<Image>();
        
        // 1. Simpan warna asli saat terlihat
        visibleColor = hotspotImage.color;
        visibleColor.a = 1f; // Pastikan alpha-nya 1 (100%)

        // 2. Tentukan warna saat tidak terlihat
        invisibleColor = hotspotImage.color;
        invisibleColor.a = 0f; // Set alpha ke 0 (0%)

        // 3. Mulai dalam keadaan tidak terlihat
        hotspotImage.color = invisibleColor;
    }

    // Fungsi ini dipanggil OTOMATIS oleh EventSystem saat tombol ini DIPILIH
    public void OnSelect(BaseEventData eventData)
    {
        hotspotImage.color = visibleColor;
    }

    // Fungsi ini dipanggil OTOMATIS oleh EventSystem saat tombol ini TIDAK LAGI DIPILIH
    public void OnDeselect(BaseEventData eventData)
    {
        hotspotImage.color = invisibleColor;
    }
}