using System.Collections;
using System.Collections.Generic;
using Data;
using Google.Protobuf.Protocol;
using UnityEngine;

public class Item
{
    //db데이터를 서버에 백업 
    public int itemDbId;
    public int templateId;
    public int count;
    public int slot;
    public bool equipped;

    public ItemType itemType;

    public Item(ItemType type)//base 
    {
        itemType = type;
    }
    public static Item MakeItem(ItemInfo itemInfo)
    {
        ItemData data = null;
        Managers.Data.ItemDict.TryGetValue(itemInfo.TemplateId, out data);
        //아이템 정보 불러오기 
        if (data == null)
            return null;

        Item item = null;
        switch (data.itemType)
        {
            case ItemType.Weapon:
                item = new Weapon(itemInfo.TemplateId);
                break;
            case ItemType.Armor:
                item = new Armor(itemInfo.TemplateId);
                break;
            case ItemType.Consumable:
                item = new Consumable(itemInfo.TemplateId);
                break;

        }

        {
            //db정보를 서버로 백업 
            item.itemDbId = itemInfo.ItemDbId;
            item.templateId = itemInfo.TemplateId;
            item.count = itemInfo.Count;
            item.slot = itemInfo.Slot;
            item.equipped = itemInfo.Equipped;

        }

        return item;

    }
}
public class Weapon : Item
{
    public WeaponType weaponType;
    public int damage;

    public Weapon(int templateId) : base(ItemType.Weapon)
    {
        ItemData item = null;
        Managers.Data.ItemDict.TryGetValue(templateId, out item);//아이템 데이터 가져옴
        if (item == null)
            return;
        WeaponData weapon = (WeaponData)item;//명시적 형 변환 

        weaponType = weapon.weaponType;
        damage = weapon.damage;
    }


}
public class Armor : Item
{
    public ArmorType armorType;
    public int defence;

    public Armor(int templateId) : base(ItemType.Armor)
    {
        ItemData item = null;
        Managers.Data.ItemDict.TryGetValue(templateId, out item);//아이템 데이터 가져옴 
        if (item == null)
            return;

        ArmorData armor = (ArmorData)item;//명시적 형 변환

        armorType = armor.armorType;
        defence = armor.defence;

    }

}
public class Consumable : Item
{
    public ConsumableType consumableType;
    public int maxCount;

    public Consumable(int templateId) : base(ItemType.Consumable)
    {
        ItemData item = null;
        Managers.Data.ItemDict.TryGetValue(templateId, out item);//아이템 데이터 가져옴 
        if (item == null)
            return;

        ConsumableData consumable = (ConsumableData)item;//명시적 형 변환

        consumableType = consumable.consumableType;
        maxCount = consumable.maxCount;


    }
}