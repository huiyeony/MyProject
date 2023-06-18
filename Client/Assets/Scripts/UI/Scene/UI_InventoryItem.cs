using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.UI;

public class UI_InventoryItem : UI_Base
{
    [SerializeField]
    Image _icon = null;

    [SerializeField]
    Image _frame = null;

    public int ItemDbId { get; private set; }
    public int TemplateId { get; private set; }
    public int Count { get; private set; }
    public int Slot { get; private set; }
    public bool Equipped { get; private set; }

    public override void Init()
    {
        //todo
        //토글 형태로 (equip) / (not equip) 결정 !
        gameObject.BindEvent((e) =>
        {
            //서버에 알려줌 
            CEquipItem equipPacket = new CEquipItem();
            equipPacket.ItemDbId = ItemDbId;
            equipPacket.Equipped = !Equipped;
            Managers.Network.Send(equipPacket);

        });
        _frame.gameObject.SetActive(Equipped);
    }
    public void SetItem(Item item)
    {
        //아이템 정보 메모리 저장 
        ItemDbId = item.itemDbId;
        TemplateId = item.templateId;
        Count = item.count;
        Slot = item.slot;
        Equipped = item.equipped;

        //template 정보 가져옴 
        ItemData itemData = null;
        Managers.Data.ItemDict.TryGetValue(TemplateId, out itemData);
        if (itemData == null)
            return;
        //sprite 바꾸기
        Sprite sprite = Managers.Resource.Load<Sprite>(itemData.dataPath);
        _icon.sprite = sprite;

        //todo : 아이템 슬롯 이동
        _frame.gameObject.SetActive(Equipped);

    }
 
}
