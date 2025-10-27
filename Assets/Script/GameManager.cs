using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public ItemManager itemManager;
    public Player player;
    public TileManager tileManager;
    public UIManager uiManager;


    public void Awake()
    {
        if (Instance != null == Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(this.gameObject);

        itemManager = GetComponent<ItemManager>();
        tileManager = GetComponent<TileManager>();
        player = FindObjectOfType<Player>();
        uiManager = GetComponent<UIManager>();
        
    } 
}
