using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{
	protected Coroutine _coSkill;

	public override void Init()
	{
		base.Init();
        AddHpUI();//머리위에 hp 추가
		AddMentalUI();//머리위에 mental추가 
    }

    

    protected override void UpdateController()
	{
		
		base.UpdateController();
	}

	
    public override void HandleSkill(int skillId)
	{
		
		if(_coSkill == null && skillId == 2)
		{
			_coSkill = StartCoroutine("CoStartShootArrow");
		}

	}

	//protected IEnumerator CoStartPunch()
	//{


	//	State = CreatureState.Skill;//스킬상태 
	//	yield return new WaitForSeconds(1.0f);//쿨타임 
	//	State = CreatureState.Idle;
	//	_coSkill = null;
	//	CheckUpdatedFlag();

 //   }

	protected IEnumerator CoStartShootArrow()
	{

		State = CreatureState.Skill;
		yield return new WaitForSeconds(0.5f);
		State = CreatureState.Idle;
		_coSkill = null;

		CheckUpdatedFlag();//패킷 전송 
	}

    protected virtual void CheckUpdatedFlag()
    {
       
    }

    public override void OnDamaged()
	{
		Debug.Log("Player HIT !");
	}
}
