using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] private Tilemap interactableTile;
    [SerializeField] private Tile hiddenInteractableTile;
    [SerializeField] private Tile plowedTile;   
    [SerializeField] private Tile wateredTile;   

    void Start()
    {
        foreach (var pos in interactableTile.cellBounds.allPositionsWithin)
        {
            TileBase  tile = interactableTile.GetTile(pos);
            if (tile != null && tile.name == "Interactable_vision")
            {
                interactableTile.SetTile(pos, hiddenInteractableTile);
            }
                

        }
    }

    public void evolveTile(Vector3Int position)
    {
        interactableTile.SetTile(position, plowedTile);
    }
            
    public string GetTileName(Vector3Int position)
    {
        TileBase tile = interactableTile.GetTile(position);
        if (tile != null)
        {
            return tile.name;
        }
        return "";
    }

    public void WaterTile(Vector3Int position)
    {
        // Cek apakah ubin di posisi itu adalah ubin bajakan kering
        string tileName = GetTileName(position);
        
        // Pastikan plowedTile tidak null sebelum mengakses .name
        if (plowedTile != null && tileName == "Sowed tanah iyeah (updated0.1)")
        {
            interactableTile.SetTile(position, wateredTile);
            Debug.Log($"Menyiram ubin di {position}");
        }
    }

    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return interactableTile.WorldToCell(worldPosition);
    }

    public Vector3 GetCellCenter(Vector3Int gridPosition)
    {
        return interactableTile.GetCellCenterWorld(gridPosition);
    }


    


}
