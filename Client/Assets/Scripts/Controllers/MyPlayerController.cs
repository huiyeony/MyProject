using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.PlayerLoop;
using static Define;

public class MyPlayerController : PlayerController
{
    //키보드 입력 받는 부분 이전

    public int WeaponAttack { get; set; }
    public int ArmorDefence { get; set; }

    protected override void UpdateController()
    {
        GetUIInput();

        switch (State)
        {
            case CreatureState.Idle:
                GetDirInput();
                break;
            case CreatureState.Moving:
                GetDirInput();
                break;
        }

        base.UpdateController();
    }

    void LateUpdate()//카메라 이동 
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    protected override void UpdateIdle()
    {
        // 이동 상태로 갈지 확인
        if (_moveKeyPressed)
        {
            State = CreatureState.Moving;
            return;
        }
        if (_coInputCooltime == null && Input.GetKey(KeyCode.Space))
        {
            //스킬을 써도 되는지 물어보기 
            CSkill skillPacket = new CSkill() { SkillInfo = new SkillInfo() };
            skillPacket.SkillInfo.SkillId = 2;//템플릿 정보  
            Managers.Network.Send(skillPacket);

            //todo
            //스킬 코루틴
            _coInputCooltime = StartCoroutine("CoInputCooltime", 0.3f);//0.3초 쿨타임 



        }
    }
    Coroutine _coInputCooltime = null;
    IEnumerator CoInputCooltime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _coInputCooltime = null;//다시 스킬을 쓸 수 있다 
    }

    void GetUIInput()// i 키보드 인벤토리 toggle
    {
        UI_GameScene sceneUI = Managers.UI.SceneUI as UI_GameScene;
        if (sceneUI == null)
            return;

        UI_Stat statUI = sceneUI.StatUI;
        UI_Inventory invenUI = sceneUI.InvenUI;
        

        if (Input.GetKeyDown(KeyCode.I))//토글 처리 
        {
            if (invenUI.gameObject.activeSelf)
            {
                invenUI.gameObject.SetActive(false);
            }
            else
            {
                
                invenUI.gameObject.SetActive(true);
                invenUI.RefreshUI();
            }

        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (statUI.gameObject.activeSelf)
                statUI.gameObject.SetActive(false);
            else
            {
                
                statUI.gameObject.SetActive(true);
                statUI.RefreshUI();
            }
        }


       
    }
    // 키보드 입력
    void GetDirInput()
    {
        _moveKeyPressed = true; //키보드 눌림

        if (Input.GetKey(KeyCode.W))
        {
            Dir = MoveDir.Up;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Dir = MoveDir.Down;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Dir = MoveDir.Left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Dir = MoveDir.Right;
        }
        else
        {
            _moveKeyPressed = false;//키보드 안눌림 
        }
    }
    protected override void MoveToNextPos()
    {
        if (_moveKeyPressed == false)
        {
            State = CreatureState.Idle;
            CheckUpdatedFlag();//패킷 보내기 
            return;
        }

        Vector3Int destPos = CellPos;

        switch (Dir)
        {
            case MoveDir.Up:
                destPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                destPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                destPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                destPos += Vector3Int.right;
                break;
        }

        if (Managers.Map.CanGo(destPos))
        {
            if (Managers.Object.Find(destPos) == null)
            {
                CellPos = destPos;
            }
        }
        CheckUpdatedFlag();//패킷 보내기


    }
    protected override void CheckUpdatedFlag()
    {
        if (_updated)
        {
            CMove movePacket = new CMove();
            movePacket.PositionInfo = PositionInfo;
            Managers.Network.Send(movePacket);
            _updated = false;
        }
    }
    public void RefreshStat()
    {
        WeaponAttack = 0;
        ArmorDefence = 0;

        foreach (Item item in Managers.Inven.Items.Values)
        {
            if (item.equipped == false) continue;
            switch (item.itemType)
            {
                case ItemType.Weapon:
                    WeaponAttack += ((Weapon)item).damage;
                    break;
                case ItemType.Armor:
                    ArmorDefence += ((Armor)item).defence;
                    break;
            }
        }
    }

}
