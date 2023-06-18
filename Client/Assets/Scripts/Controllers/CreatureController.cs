using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using Google.Protobuf.Protocol;
using UnityEngine;
using static Define;

public class CreatureController : BaseController
{


	public HpUI _hpUI;
    public MentalUI _mentalUI;

    void Start()
    {
        Init();
    }

    void Update()
    {
        UpdateController();
    }
    protected override void UpdateAnimation()
    {
        //null 방지 

        _animator = gameObject.GetComponent<Animator>();
        _sprite = gameObject.GetComponent<SpriteRenderer>();

        if (State == CreatureState.Idle)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    _animator.Play("IDLE_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("IDLE_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("IDLE_RIGHT");
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("IDLE_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
        else if (State == CreatureState.Moving)
        {
            switch (Dir)
            {
                case MoveDir.Up:
                    _animator.Play("WALK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("WALK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("WALK_RIGHT");
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("WALK_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
        else if (State == CreatureState.Skill)
        {

            switch (Dir)
            {
                case MoveDir.Up:
                    _animator.Play("ATTACK_WEAPON_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Down:
                    _animator.Play("ATTACK_WEAPON_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.Left:
                    _animator.Play("ATTACK_WEAPON_RIGHT");
                    _sprite.flipX = true;
                    break;
                case MoveDir.Right:
                    _animator.Play("ATTACK_WEAPON_RIGHT");
                    _sprite.flipX = false;
                    break;
            }


        }
    }
    public virtual void HandleSkill(int skillId)
    {
        //

    }
    public void AddHpUI() //머리위에 hp 추가하기 
    {
		
		GameObject go = Managers.Resource.Instantiate("UI/HpUI", transform);//주인님에 종속적
		go.name = "HpUI";
		go.transform.localPosition = new Vector3(0, 0.5f, 0);
	 
		_hpUI = go.GetComponent<HpUI>();
		UpdateHpUI();//ui갱신 
		
	}
    public void AddMentalUI()//머리 위에 정신력 추가하기 
    {
        GameObject go = Managers.Resource.Instantiate("UI/MentalUI", transform);
        go.name = "MentalUI";
        go.transform.localPosition = new Vector3(0, 1f, 0);

        _mentalUI = go.GetComponent<MentalUI>();
        UpdateMentalUI();//ui갱신 


    }

	public void UpdateHpUI()//hp 빨간 부분 갱신하자 
	{
        if (_hpUI == null)
            return;

        float ratio = 0.0f;
		if (Stat.MaxHp > 0)
			ratio = ((float)Hp) / Stat.MaxHp;

        //spawnPacketHandler -> objectManager add-> ~controller updateHp -> ~controller init
        
        _hpUI.UpdateHpUI(ratio);
		
	}
    public void UpdateMentalUI()
    {
        float ratio = 0f;
        if (Stat.Mental > 0)
            ratio = ((float)Mental) / 200; //최대 정신력 200

        _mentalUI.UpdateMentalUI(ratio);
    }
	


	public override int Hp
	{
		get
		{
			return base.Hp;
		}
		set
		{
			base.Hp = value;
			UpdateHpUI();
		}
	}
    public override int Mental
    {
        get { return Stat.Mental; }
        set
        {
            Stat.Mental = value;
            UpdateMentalUI();//바로 갱신 
        }
    }
    public override void Init()
    {
        base.Init();

    }
   



	
}
