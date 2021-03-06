using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
using System;

public class DataHandler : MonoBehaviour
{
    private GameObject furniture;

    [SerializeField] private ButtonManager buttonPrefab;
    [SerializeField] private GameObject buttonContainer;
    [SerializeField] private List<Item> _items;
    [SerializeField] private String label;

    private int current_id = 0;

    private static DataHandler instance;
    public static DataHandler Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DataHandler>();
            }
            return instance;
        }
    }

    private async void Start()
    {
        //LoadItems();
        await Get(label);
        CreateButtons();
    }

    //void LoadItems()
    //{
    //    var items_obj = Resources.LoadAll("Items", typeof(Item));
    //    foreach (var item in items_obj)
    //    {
    //        items.Add(item as Item);
    //    }
    //}

    void CreateButtons()
    {
        foreach (Item i in _items)
        {
            ButtonManager b = Instantiate(buttonPrefab, buttonContainer.transform);
            b.ItemId = current_id;
            b.ButtonTexture = i.itemImage;
            current_id++; //repeat the above foreach function while more than one furnitures exist.
        }
    }

    public void SetFurniture(int id)
    {
        furniture = _items[id].itemPrefab;
    }

    public GameObject GetFurniture() //make the private GameObject to Public
    {
        return furniture;
    }

    public async Task Get(String label)
    {
        var locations = await Addressables.LoadResourceLocationsAsync(label).Task;      //using async, "await" is needed in the script
        foreach (var location in locations)
        {
            var obj = await Addressables.LoadAssetAsync<Item>(location).Task;       //Create an object using that location
            _items.Add(obj);
        }
    }
}
