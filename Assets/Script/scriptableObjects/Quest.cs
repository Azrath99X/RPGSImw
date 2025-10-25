// File: Quest.cs
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest")]
public class Quest : ScriptableObject
{
    public string questID; // ID unik, misal: "FIND_HOE"
    public string questName; // Nama di UI, misal: "Cari Cangkul!"
    [TextArea(3, 5)]
    public string description;

    public bool isComplete = false;
    // Anda bisa tambahkan 'Rewards' di sini nanti
}