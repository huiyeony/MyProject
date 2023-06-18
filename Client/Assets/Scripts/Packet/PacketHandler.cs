
using Data;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class PacketHandler
{

    public static void SEnterGameHandler(PacketSession session, IMessage packet)
    {
        SEnterGame enterPacket = packet as SEnterGame;
        ServerSession serverSession = session as ServerSession;
        //
        Debug.Log(enterPacket.Info);
        Debug.Log(enterPacket.TimeInfo);
        //
        Managers.Object.Add(enterPacket.Info, true); //게임 씬에 나를  배치한다
        //조명을 위해 시간을 저장 해둠 !
        TimeInfo timeInfo = enterPacket.TimeInfo;
        Managers.Light.LightUI.TimeInfo = timeInfo;
       
    }
    public static void SLeaveGameHandler(PacketSession session, IMessage packet)
    {
        SLeaveGame leavePacket = packet as SLeaveGame;
        ServerSession serverSession = session as ServerSession;
        //
        Debug.Log(leavePacket.Id);
        
        Managers.Object.RemoveMyPlayer();//스스로를 삭제 한다 
    }
    public static void SSpawnHandler(PacketSession session, IMessage packet)
    {
        SSpawn spawnPacket = packet as SSpawn;
        ServerSession serverSession = session as ServerSession;
        //
        Debug.Log(spawnPacket.Infos);
        //
        foreach (ObjectInfo info in spawnPacket.Infos)//다른 사람을 씬에 배치한다
        {
            Managers.Object.Add(info, false);
        }



    }
    public static void SDespawnHandler(PacketSession session, IMessage packet)
    {
        SDespawn despawnPacket = packet as SDespawn;

        foreach (ObjectInfo info in despawnPacket.Infos)//다른 사람을 씬에서 삭제 한다 
        {
            Managers.Object.Remove(info.Id);
        }
    }
    public static void SMoveHandler(PacketSession session, IMessage packet)
    {
        SMove movePacket = packet as SMove;
        ServerSession serverSession = session as ServerSession;

        //(누가 ) (어디로 )이동했는지 찾는다 
        GameObject go = Managers.Object.FindById(movePacket.Id);
        if (go == null)
            return;
        if (Managers.Object.MyPlayer.Id == movePacket.Id)//'나 ' 는 내가 직접 조종 한다 
            return;

        BaseController bc = go.GetComponent<BaseController>();
        if (bc == null)
            return;
        bc.PositionInfo = movePacket.PositionInfo;

    }
    public static void SSkillHandler(PacketSession session, IMessage packet)
    {
        //todo
        SSkill skillOkPacket = packet as SSkill;
        ServerSession serverSession = session as ServerSession;

        //누가 스킬을 썼는지 확인하고
        GameObject go = Managers.Object.FindById(skillOkPacket.Id);
        if (go == null)
            return;

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc == null)
            return;

        //게임화면에 나타낸다
        cc.HandleSkill(skillOkPacket.SkillInfo.SkillId);
    }
  
    public static void SDieHandler(PacketSession session, IMessage packet)
    {
        //todo
        //죽음 이펙트 재생
        SDie diePacket = packet as SDie;
        ServerSession serverSession = session as ServerSession;

        //누가 죽었는지 확인하고 
        GameObject go = Managers.Object.FindById(diePacket.Id);
        if (go == null)
            return;

        GameObject effect = Managers.Resource.Instantiate("Effect/DieEffect");
        effect.transform.position = go.transform.position;//죽은 위치 
        effect.GetComponent<Animator>().Play("START");
        GameObject.Destroy(effect, 0.5f);

    }
    public static void SConnectedHandler(PacketSession session, IMessage packet)
    {
        Debug.Log("S_ConnectedHandler");
        //로그인 요청
        CLogin loginPacket = new CLogin();
        loginPacket.UniqueId = SystemInfo.deviceUniqueIdentifier;
        Managers.Network.Send(loginPacket);

    }
    public static void SLoginHandler(PacketSession session, IMessage packet)
    {
        SLogin loginPacket = packet as SLogin;
        ServerSession serverSession = session as ServerSession;

        //만약 내가 보유하고 있는 캐릭터가 없을때는
        //캐릭터 생성을 요청 한다 
        if (loginPacket.Players == null || loginPacket.Players.Count == 0)
        {
            CCreatePlayer createPacket = new CCreatePlayer();
            createPacket.Name = $"Player({Random.Range(1, 1000).ToString("0000")})";//임의의 이름 할당
            Managers.Network.Send(createPacket);
        }
        else
        {
            //무조건 ((첫번째)) 캐릭터로 로그인 하자
            LobbyPlayerInfo lobbyPlayer = loginPacket.Players[0];
            CEnterGame enterPacket = new CEnterGame();
            enterPacket.Name = lobbyPlayer.Name;

            Managers.Network.Send(enterPacket);//인 게임으로 입장을 요청하자


        }
    }
    
    public static void SCreatePlayerHandler(PacketSession session, IMessage packet)
    {
        SCreatePlayer createOkPacket = packet as SCreatePlayer;
        ServerSession serverSession = session as ServerSession;

        //캐릭터 생성 실패
        if (createOkPacket.Player == null )
        {
            CCreatePlayer createPacket = new CCreatePlayer();
            createPacket.Name = $"Player({Random.Range(1, 1000).ToString("0000")})";//임의의 이름 할당
            Managers.Network.Send(createPacket);
        }
        else//성공 
        {

            //무조건 첫번째 캐릭터로 로그인 하자
            LobbyPlayerInfo lobbyPlayer = createOkPacket.Player;
            CEnterGame enterPacket = new CEnterGame();
            enterPacket.Name = lobbyPlayer.Name;

            Managers.Network.Send(enterPacket);//인 게임으로 입장을 요청하자
        }

        
    }
    public static void SChangeHpHandler(PacketSession session, IMessage packet)
    {
        //todo: Hp 정보 수정 

        SChangeHp changePacket = packet as SChangeHp;
        ServerSession serverSession = session as ServerSession;

        //( 누가 ) ( 체력 ) 인지 수정 
        GameObject go = Managers.Object.FindById(changePacket.Id);
        if (go == null)
            return;

        CreatureController cc = go.GetComponent<CreatureController>();
        if (cc == null)
            return;

        cc.OnDamaged(changePacket.Hp);

    }
    public static void SItemListHandler(PacketSession session,IMessage packet)
    {
        ServerSession serverSession = (ServerSession)session;
        SItemList itemListPacket = (SItemList)packet;

        UI_GameScene sceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Inventory invenUI = sceneUI.InvenUI;

        Managers.Inven.Clear();

        //인벤토리 매니저에게 알려줌
        foreach(ItemInfo itemInfo in itemListPacket.ItemInfos)
        {
            Item item = Item.MakeItem(itemInfo);
            Managers.Inven.Add(item); //인벤토리 매니저가 원본 데이터 저장함

        }

        //refresh
        invenUI.RefreshUI();

    }
    public static void SAddItemHandler(PacketSession session, IMessage packet)
    {
        ServerSession serverSession = (ServerSession)session;
        SAddItem addPacket = (SAddItem)packet;

        UI_GameScene sceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Inventory invenUI = sceneUI.InvenUI;


        //인벤토리 매니저에게 알려줌
        foreach (ItemInfo itemInfo in addPacket.ItemInfos)
        {
            Item item = Item.MakeItem(itemInfo);
            Managers.Inven.Add(item); //인벤토리 매니저가 원본 데이터 저장함

        }

        //refresh 
        invenUI.RefreshUI();

    }

    public static void SEquipItemHandler(PacketSession session,IMessage packet)
    {
        ServerSession serverSession = (ServerSession)session;
        SEquipItem equipOkPacket = (SEquipItem)packet;//db적용 ok 패킷

        //
        Debug.Log("아이템 작용 ok!");

        UI_GameScene sceneUI = Managers.UI.SceneUI as UI_GameScene;
        UI_Inventory invenUI = sceneUI.InvenUI;
        UI_Stat statUI = sceneUI.StatUI;

        Item item = Managers.Inven.Get(equipOkPacket.ItemDbId);
        item.equipped = equipOkPacket.Equipped;

        invenUI.RefreshUI();
        statUI.RefreshUI();


    }
    public static void SLevelUpHandler(PacketSession session,IMessage packet)
    {
         ServerSession serverSession = (ServerSession)session;
        SLevelUp levelPacket = (SLevelUp)packet;

        MyPlayerController mc = Managers.Object.MyPlayer;
        mc.Stat.MergeFrom(levelPacket.StatInfo);//스탯 업그레이드

        //
        GameObject go = Managers.Object.FindById(levelPacket.Id);
        if (go == null) return;

        GameObject effect = Managers.Resource.Instantiate("Effect/LevelUp");
        effect.transform.position = go.transform.position + new Vector3(0,1); // 머리 위에 표시 
        effect.GetComponent<Animator>().Play("START");
        GameObject.Destroy(effect, 0.5f);

        Debug.Log("level up");

    }
    public static void SPingHandler(PacketSession session,IMessage packet)
    {
        //서버에게 응답 
        CPong pongPacket = new CPong();
        Managers.Network.Send(pongPacket);
        
    }



}
