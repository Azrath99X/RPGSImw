// File: Character.cs
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Dialogue/Character")]
public class Character : ScriptableObject
{
    public string characterName; // e.g., "Desta" , "Ari" , "Narrator" [cite: 28]
    public Sprite portrait; // Opsional, untuk UI
    // Anda bisa tambahkan field lain seperti warna teks, font, dll.
}