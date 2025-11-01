using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic; // Diperlukan untuk Dictionary
using System.IO; // Tambahkan ini di paling atas
using System.Linq; // Tambahkan ini di paling atas

public class TileManager : MonoBehaviour
{
    // Singleton Pattern
    public static TileManager Instance { get; private set; }

    [Header("Tilemap")]
    [SerializeField] private Tilemap interactableTile;

    [Header("Tile Assets (Base)")]
    [SerializeField] private Tile hiddenInteractableTile; // Ubin "Interactable_vision" Anda
    
    [Header("Tile Assets (Farming)")]
    [SerializeField] private Tile plowedTile; 
    [SerializeField] private Tile wetPlowedTile; 
    
    [Header("Tile Assets (Turnip Growth 4-Day)")]
    [SerializeField] private Tile seededTile; // Hari 0 (Kering)
    [SerializeField] private Tile wetSeededTile; // Hari 0 (Basah)
    [SerializeField] private Tile sproutTile; // Hari 1 (Kering)
    [SerializeField] private Tile wetSproutTile; // Hari 1 (Basah)
    [SerializeField] private Tile growthDay2Dry; // Hari 2 (Kering) - BARU
    [SerializeField] private Tile growthDay2Wet; // Hari 2 (Basah) - BARU
    [SerializeField] private Tile growthDay3Dry; // Hari 3 (Kering) - BARU
    [SerializeField] private Tile growthDay3Wet; // Hari 3 (Basah) - BARU
    [SerializeField] private Tile matureTile; // Hari 4 (Dewasa)

    private Dictionary<string, TileBase> tileAssetsByName;
    private bool isTileDatabaseInitialized = false;

    

    // Daftar untuk menyimpan perubahan agar tidak konflik saat iterasi
    private Dictionary<Vector3Int, TileBase> tileChanges = new Dictionary<Vector3Int, TileBase>();

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        InitializeTileDatabase(); // Panggil fungsi baru ini
    }

    void Start()
    {
        foreach (var pos in interactableTile.cellBounds.allPositionsWithin)
        {
            TileBase tile = interactableTile.GetTile(pos);
            if (tile != null && tile.name == "Interactable_vision")
            {
                interactableTile.SetTile(pos, hiddenInteractableTile);
            }
        }
    }

    // 1. Berlangganan ke TimeManager
    void OnEnable()
    {
        TimeManager.OnDayChanged += HandleDayChanged;
    }

    void OnDisable()
    {
        TimeManager.OnDayChanged -= HandleDayChanged;
    }

    // 2. LOGIKA INTI: Pertumbuhan & Pengeringan saat Hari Berganti
    private void HandleDayChanged()
    {
        Debug.Log("TileManager: Mendeteksi hari baru. Memproses ubin...");
        
        // Bersihkan daftar perubahan
        tileChanges.Clear();

        foreach (var pos in interactableTile.cellBounds.allPositionsWithin)
        {
            TileBase tile = interactableTile.GetTile(pos);
            if (tile == null) continue;

            // --- LOGIKA PERTUMBUHAN (Hanya cek versi BASAH) ---
            // Urutan 'if' dari TUA ke MUDA sangat penting
            
            // Hari 3 (Basah) -> Hari 4 (Dewasa)
            if (tile.name == GetTileName(growthDay3Wet))
            {
                tileChanges[pos] = matureTile;
            }
            // Hari 2 (Basah) -> Hari 3 (Kering)
            else if (tile.name == GetTileName(growthDay2Wet))
            {
                tileChanges[pos] = growthDay3Dry;
            }
            // Hari 1 (Basah) -> Hari 2 (Kering)
            else if (tile.name == GetTileName(wetSproutTile))
            {
                tileChanges[pos] = growthDay2Dry;
            }
            // Hari 0 (Basah) -> Hari 1 (Kering)
            else if (tile.name == GetTileName(wetSeededTile))
            {
                tileChanges[pos] = sproutTile;
            }

            // --- LOGIKA PENGERINGAN (Jika tidak tumbuh) ---
            // Ubin-ubin ini tidak terdeteksi di atas, berarti mereka kering ATAU tidak disiram
            
            // Benih kering (Hari 0 Kering) -> Tetap
            else if (tile.name == GetTileName(seededTile)) { /* Tetap */ }
            // Tunas kering (Hari 1 Kering) -> Tetap
            else if (tile.name == GetTileName(sproutTile)) { /* Tetap */ }
            // Hari 2 Kering -> Tetap
            else if (tile.name == GetTileName(growthDay2Dry)) { /* Tetap */ }
            // Hari 3 Kering -> Tetap
            else if (tile.name == GetTileName(growthDay3Dry)) { /* Tetap */ }
            
            // Tanah bajakan basah -> Kering
            else if (tile.name == GetTileName(wetPlowedTile))
            {
                tileChanges[pos] = plowedTile;
            }
        }
        
        // Terapkan semua perubahan
        foreach(var change in tileChanges)
        {
            interactableTile.SetTile(change.Key, change.Value);
        }
    }

    // 3. FUNGSI INTERAKSI

    // Dipanggil oleh cangkul (Hoe)
    public void evolveTile(Vector3Int position)
    {
        interactableTile.SetTile(position, plowedTile);
    }

    // Dipanggil oleh alat siram (Watering Can) - DIPERBARUI
    public void WaterTile(Vector3Int position)
    {
        string tileName = GetTileName(position);
        
        if (tileName == GetTileName(plowedTile))
            interactableTile.SetTile(position, wetPlowedTile);
        else if (tileName == GetTileName(seededTile)) 
            interactableTile.SetTile(position, wetSeededTile);
        else if (tileName == GetTileName(sproutTile))
            interactableTile.SetTile(position, wetSproutTile);
        else if (tileName == GetTileName(growthDay2Dry))
            interactableTile.SetTile(position, growthDay2Wet);
        else if (tileName == GetTileName(growthDay3Dry))
            interactableTile.SetTile(position, growthDay3Wet);
    }

    // Dipanggil oleh benih (Seed)
    public bool PlantSeed(Vector3Int position)
    {
        string tileName = GetTileName(position);

        if (tileName == GetTileName(plowedTile))
        {
            interactableTile.SetTile(position, seededTile);
            return true; 
        }
        else if (tileName == GetTileName(wetPlowedTile))
        {
            interactableTile.SetTile(position, wetSeededTile);
            return true;
        }
        return false; 
    }

    // 4. FUNGSI HELPER (Getters)

    // Helper untuk mendapatkan nama tile dengan aman (mencegah error null)
    public string GetTileName(TileBase tile)
    {
        if (tile == null) return "";
        return tile.name;
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

    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return interactableTile.WorldToCell(worldPosition);
    }
    
    public Vector3 GetCellCenter(Vector3Int gridPosition)
    {
        return interactableTile.GetCellCenterWorld(gridPosition);
    }

    private void InitializeTileDatabase()
    {
        if (isTileDatabaseInitialized) return;

        tileAssetsByName = new Dictionary<string, TileBase>();
        
        // Ambil SEMUA tile yang Anda referensikan di skrip ini
        TileBase[] allTiles = {
            hiddenInteractableTile, plowedTile, wetPlowedTile, seededTile,
            wetSeededTile, sproutTile, wetSproutTile, growthDay2Dry,
            growthDay2Wet, growthDay3Dry, growthDay3Wet, matureTile
        };

        foreach (TileBase tile in allTiles)
        {
            if (tile != null && !tileAssetsByName.ContainsKey(tile.name))
            {
                tileAssetsByName.Add(tile.name, tile);
            }
        }
        isTileDatabaseInitialized = true;
        Debug.Log($"[TileManager] Database tile diinisialisasi dengan {tileAssetsByName.Count} tile unik.");
    }
    
    public List<TileState> GetChangedTileData()
    {
        if (!isTileDatabaseInitialized) InitializeTileDatabase();

        List<TileState> data = new List<TileState>();
        BoundsInt bounds = interactableTile.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileBase tile = interactableTile.GetTile(pos);

                // Hanya simpan jika petak BUKAN default
                if (tile != null && tile.name != hiddenInteractableTile.name)
                {
                    data.Add(new TileState { position = pos, tileName = tile.name });
                }
            }
        }
        return data;
    }

    // Fungsi untuk MENERAPKAN data petak (untuk di-load)
    public void LoadTileData(List<TileState> data)
    {
        if (!isTileDatabaseInitialized) InitializeTileDatabase();
        
        // 1. Bersihkan seluruh tilemap kembali ke default
        BoundsInt bounds = interactableTile.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                interactableTile.SetTile(new Vector3Int(x, y, 0), hiddenInteractableTile);
            }
        }

        // 2. Terapkan data yang tersimpan
        if (data == null) return;
        
        foreach (var state in data)
        {
            if (tileAssetsByName.TryGetValue(state.tileName, out TileBase tile))
            {
                interactableTile.SetTile(state.position, tile);
            }
        }
        Debug.Log($"[SaveData] Berhasil load {data.Count} state petak.");
    }

    // Helper untuk Player.cs
    public string GetHiddenTileName() { return GetTileName(hiddenInteractableTile); }
    public string GetPlowedTileName() { return GetTileName(plowedTile); }
    public string GetWetPlowedTileName() { return GetTileName(wetPlowedTile); }
    public string GetSeededTileName() { return GetTileName(seededTile); }
    public string GetSproutTileName() { return GetTileName(sproutTile); }
    public string GetGrowthDay2DryName() { return GetTileName(growthDay2Dry); }
    public string GetGrowthDay3DryName() { return GetTileName(growthDay3Dry); }
}