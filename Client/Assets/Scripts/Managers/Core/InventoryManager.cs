using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class InventoryManager 
{
    
    //(db)와 (템플릿 )정보를 통합한 item 클래스를 관리함 
    public Dictionary<int, Item> Items { get; private set; } = new Dictionary<int, Item>();
    object _lock = new object();

    //인벤토리에 아이템 추가 
    public void Add(Item item)
    {
        lock (_lock)
        {
            Items.Add(item.itemDbId, item);
        }
    }

    public Item Get(int itemDbId)
    {
        lock (_lock)
        {
            Item item = null;
            Items.TryGetValue(itemDbId, out item);
            if (item != null)
                return item;
            return null;
        }
    }
    public bool Find(int itemDbId, Func<Item, bool> func)
    {
        lock (_lock)
        {
            foreach (Item item in Items.Values)
            {
                if (func.Invoke(item) == true)
                    return true;

            }
            return false;
        }
    }
    public void Clear()
    {
        Items.Clear();//딕셔너리 비우기  
    }
}
