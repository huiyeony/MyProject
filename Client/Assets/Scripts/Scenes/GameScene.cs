using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameScene : BaseScene
{

    void Start()
    {

        Init();//초기화 함수 
    }
    
    protected override void Init()
    {
        //트로이의 목마 같은 역할
        base.Init();
        SceneType = Define.Scene.Game; //어떤 씬인지 저장 

        Managers.UI.ShowSceneUI<UI_GameScene>(); //씬 유아이 프리팹 로드 하자

        //맵을 로드 하자
        Managers.Map.LoadMap(1);
        //스크린 크기
        Screen.SetResolution(640, 480, false);


    }
 
  
    public override void Clear()
    {
        //todo
    }
}
