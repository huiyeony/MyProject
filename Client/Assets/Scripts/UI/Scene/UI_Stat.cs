using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google.Protobuf.Protocol;
using Data;

public class UI_Stat : UI_Base
{
    enum Images
    {
        Slot_Helmet,
        Slot_Armor,
        Slot_Boots,
        Slot_Amulet,
        Slot_Ring,
        Slot_Weapon
        
    }
    enum Texts
    {
        AttackText,
        AttackValueText,
        DefenceText,
        DefenceValueText
    }
    bool _init = false;
    public override void Init()
    {
        //이미지 ,텍스트 물고 있기 
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));
        _init = true;
        RefreshUI();
    }
    public void RefreshUI()
    {
        if (!_init)
            return;

        //모두 가리기 
        Get<Image>((int)Images.Slot_Helmet).enabled = false;
        Get<Image>((int)Images.Slot_Armor).enabled = false;
        Get<Image>((int)Images.Slot_Boots).enabled = false;
        Get<Image>((int)Images.Slot_Amulet).enabled = false;
        Get<Image>((int)Images.Slot_Ring).enabled = false;
        Get<Image>((int)Images.Slot_Weapon).enabled = false;

        Get<Text>((int)Texts.AttackValueText).enabled = false;
        Get<Text>((int)Texts.DefenceValueText).enabled = false;


        //이미지 로드 하기
        foreach (Item item in Managers.Inven.Items.Values)
        {
            if (item.equipped == false)//착용중 아님 
                continue;


            ItemData data = null;
            Managers.Data.ItemDict.TryGetValue(item.templateId, out data);
            Sprite sprite = Managers.Resource.Load<Sprite>(data.dataPath);


            if (item.itemType == ItemType.Weapon)
            {
                Get<Image>((int)Images.Slot_Weapon).enabled = true;
                Get<Image>((int)Images.Slot_Weapon).sprite = sprite;
            }
            else if (item.itemType == ItemType.Armor)
            {
                Armor armor = (Armor)item;
                switch (armor.armorType)
                {
                    case ArmorType.Helmet:
                        Get<Image>((int)Images.Slot_Helmet).enabled = true;
                        Get<Image>((int)Images.Slot_Helmet).sprite = sprite;
                        break;
                    case ArmorType.Armor:
                        Get<Image>((int)Images.Slot_Armor).enabled = true;
                        Get<Image>((int)Images.Slot_Armor).sprite = sprite;
                        break;
                    case ArmorType.Boots:
                        Get<Image>((int)Images.Slot_Boots).enabled = true;
                        Get<Image>((int)Images.Slot_Boots).sprite = sprite;
                        break;
                    case ArmorType.Amulet:
                        Get<Image>((int)Images.Slot_Amulet).enabled = true;
                        Get<Image>((int)Images.Slot_Amulet).sprite = sprite;
                        break;
                    case ArmorType.Ring:
                        Get<Image>((int)Images.Slot_Ring).enabled = true;
                        Get<Image>((int)Images.Slot_Ring).sprite = sprite;
                        break;
                }
            }

        }
        //텍스트 로드 하기
        
        MyPlayerController player = Managers.Object.MyPlayer;
        player.RefreshStat();

        //공격 
        Get<Text>((int)Texts.AttackValueText).enabled = true;
        int totalAttack = player.Stat.Attack + player.WeaponAttack;
        Get<Text>((int)Texts.AttackValueText).text = $"{totalAttack}(+{player.WeaponAttack})";
        //방어 
        Get<Text>((int)Texts.DefenceValueText).enabled = true;
        Get<Text>((int)Texts.DefenceValueText).text = $"{player.ArmorDefence}";


    }

}
