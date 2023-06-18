using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using Google.Protobuf;
using ServerCore;
using UnityEngine;
using Data;

public class ObjectManager
{
	Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();
	
	public MyPlayerController MyPlayer { get; private set; }//스스로에 대한 컨트롤러 가지고 있는다

	public GameObjectType GetObjectTypeById(int objectId)
	{
		return (GameObjectType)(objectId >> 24 & 0x7F);
	}
    public void Add(ObjectInfo info, bool mySelf = false)//씬에 플레이어 배치 한다 -> 수정 
	{
		//비전 큐브가 나를 한번더 스폰함
		if (MyPlayer != null && info.Id == MyPlayer.Id)
			return;
		if (_objects.ContainsKey(info.Id))
			return;
		GameObjectType type = GetObjectTypeById(info.Id);//오브젝트 타입
		if(type == GameObjectType.Player)
		{
            if (mySelf)//자기 자신 일때
            {

                GameObject go = Managers.Resource.Instantiate("Creature/Myplayer");//씬에 추가
                go.name = info.Name;

                MyPlayer = go.GetComponent<MyPlayerController>();
                MyPlayer.Id = info.Id;//아이디 저장
                MyPlayer.PositionInfo = info.PositionInfo;//목표 위치 등록 
				MyPlayer.Stat.MergeFrom(info.StatInfo);//hp,,,정보 

				
				//메모리에 저장 
                _objects.Add(info.Id, go);

            }
            else
            {
                // 플레이어를 만든다
                GameObject go = Managers.Resource.Instantiate("Creature/Player");//씬에 추가 
                go.name = info.Name;

                PlayerController pc = go.GetComponent<PlayerController>();
				
				pc.Id = info.Id;//아이디 저장
                pc.PositionInfo = info.PositionInfo;//목표 위치 등록 
				pc.Stat.MergeFrom(info.StatInfo);//hp,,정보


                _objects.Add(info.Id, go);
            }
        }
		else if(type == GameObjectType.Monster)///몬스터 일때 
		{
			// 몬스터  만든다
			try
			{
                GameObject go = Managers.Resource.Instantiate("Creature/Monster");//씬에 추가 
                go.name = info.Name;

                MonsterController mc = go.GetComponent<MonsterController>();


                mc.Id = info.Id;//아이디 저장
                mc.PositionInfo = info.PositionInfo;//목표 위치 등록 
                mc.Stat.MergeFrom(info.StatInfo);//hp,,정보


                _objects.Add(info.Id, go);
            }
			catch(Exception e)
			{
				Debug.Log(e.ToString());
			}
           
        }
		else if(type == GameObjectType.Projectile)//발사체 일때 
		{
			
			GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
			go.name = "arrow";

            ArrowController ac = go.GetComponent<ArrowController>();
            ac.Id = info.Id;
            ac.PositionInfo = info.PositionInfo;
            ac.Stat.MergeFrom(info.StatInfo);
            //막바로 cellpos으로 지정 
            ac.SyncPos();

			_objects.Add(info.Id, go);//목록에 추가 

		}
   
        else if(type == GameObjectType.Resource)//아이템 일때
		{
			//아이템 프리팹 이름 찾기
			ItemData data = null;
			Managers.Data.ItemDict.TryGetValue(info.TemplateId, out data);

			GameObject go = Managers.Resource.Instantiate($"Item/{data.prefabPath}");
			go.name = "item";

			ItemController ic = go.GetComponent<ItemController>();
			ic.Id = info.Id; // GenerateId
			ic.TemplateId = info.TemplateId;//템플릿 아이디 
			ic.PositionInfo.MergeFrom(info.PositionInfo);
			ic.Stat.MergeFrom(info.StatInfo);

			ic.SyncPos();

			_objects.Add(info.Id, go);//메모리에 저장 
		}										

		
	}
	public void Add(int id ,GameObject go)
	{
		_objects.Add(id, go);
	}
	public void RemoveMyPlayer()//내 플레이어를 삭제 한다 
	{
		if (MyPlayer == null)
			return;


		MyPlayer = null; //참조 삭제
		//todo : 
		RemoveAll();
    }
	public void Remove(int id)
	{
		GameObject go = FindById(id);
		if (go == null)
			return;
		//메모리에서 삭제 
		_objects.Remove(id);
		//씬에서 삭제
		Managers.Resource.Destroy(go);
	}
	public void RemoveAll()
	{

		foreach (GameObject go in _objects.Values)//씬에서 삭제 
			Managers.Resource.Destroy(go);


        _objects.Clear();//메모리에서 삭제 
    }
	public GameObject FindById(int id)
	{
		GameObject go = null;
		_objects.TryGetValue(id, out go);
		if (go != null)
			return go;
		return null;

	}
	public GameObject Find(Vector3Int cellPos)
	{
		foreach (GameObject obj in _objects.Values)
		{
			CreatureController cc = obj.GetComponent<CreatureController>();
			if (cc == null)
				continue;

			if (cc.CellPos == cellPos)
				return obj;
		}

		return null;
	}

	public GameObject Find(Func<GameObject, bool> condition)
	{
		foreach (GameObject obj in _objects.Values)
		{
			if (condition.Invoke(obj))
				return obj;
		}

		return null;
	}

	public void Clear()
	{
		_objects.Clear();
	}
}
