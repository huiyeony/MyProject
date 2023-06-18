using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_Inventory : UI_Base
{
    //ui inventory 는 ui 임 !!
    List<UI_InventoryItem> _items = new List<UI_InventoryItem>();

 
    public override void Init()
    {
        
        _items.Clear();//아이템을 조종하는 스크립트 reset 

        Transform grid = transform.Find("ItemsGrid");//child transform !
        foreach(Transform child in grid)
            Destroy(child.gameObject);
        
        for(int i = 0; i< 20; i++)//아이템을 조종하는 스크립트 
        {
            //씬에서 생성
            
            GameObject go = Managers.Resource.Instantiate("UI/Scene/UI_InventoryItem", grid);
            UI_InventoryItem item = Util.GetOrAddComponent<UI_InventoryItem>(go);
            item.Init();
            _items.Add(item);
        }
        //sitemlist패킷 처리 
        RefreshUI();
    }

    //아이템이 어찌어찌 추가되면 호출됨 
    public void RefreshUI()
    {

        if (_items.Count == 0)
            return;

        //slot에 집어넣기
        List<Item> items = Managers.Inven.Items.Values.ToList();
        items.Sort((left, right) => { return left.slot - right.slot; });


        //보유하고 있는 아이템들 
        foreach(Item item in items)
        {
            
            if (item.slot < 0 || item.slot >= 20)    continue;
            _items[item.slot].SetItem(item);

        }
    }
}
