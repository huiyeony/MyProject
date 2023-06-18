using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GameScene : UI_Scene
{
    //항상 씬에 존재 -> UI_Scene 
    public UI_Stat StatUI { get; set; }
    public UI_Inventory InvenUI { get; set; }


    public override void Init()
    {
        base.Init();

        StatUI = gameObject.GetComponentInChildren<UI_Stat>();
        InvenUI = gameObject.GetComponentInChildren<UI_Inventory>();

        //일단 안보이게 함  
        StatUI.gameObject.SetActive(false);
        InvenUI.gameObject.SetActive(false);
    }
}
