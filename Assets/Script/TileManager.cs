using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] private Tilemap interactableTile;
    [SerializeField] private Tile hiddenInteractableTile;
    [SerializeField] private Tile plowedTile;   
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


}
