using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    
    public int Id { get; set; } //고유 번호
    StatInfo _stat = new StatInfo();//원본 데이터
    PositionInfo _positionInfo = new PositionInfo();//원본 데이터 !!!
    

    protected bool _updated = false; //state & moveDir & cellPos 변화 감지



    protected Animator _animator;
    protected SpriteRenderer _sprite;

    protected bool _moveKeyPressed = false;


    protected virtual void UpdateAnimation()
    {
        //
    }
    public virtual void OnDamaged(int hp)
    {
        Hp = hp;//hp수정
                

        Debug.Log($"todo : {hp}");
    }


    public void SyncPos()
    {
        //cellPos 바로 적용
        Vector3 dest = CellPos + new Vector3(0.5f, 0.5f, 0);
        transform.position = dest;

    }
    
    public StatInfo Stat//스폰패킷에서 정보 받을 것임 
    {
        get { return _stat; }
        set
        {
            if (_stat.Equals(value))//똑같으면 리턴 
                return;
            _stat = value;
        }
    }

    public virtual int Hp
    {
        get
        {
            return _stat.Hp;
        }
        set
        {
            if (_stat.Hp.Equals(value))
                return;
            _stat.Hp = value;
        }
    }
    //stat 정보에서 정신력을 추가하자 !
    public virtual int Mental
    {
        get { return _stat.Mental; }
        set
        {
            _stat.Mental = value;
        }
    }
    public Vector3Int CellPos
    {
        get { return new Vector3Int(_positionInfo.PosX, _positionInfo.PosY, 0); }
        set
        {
            if (_positionInfo.PosX == value.x && _positionInfo.PosY == value.y)
                return;

            _updated = true;//패킷 보내기 

            _positionInfo.PosX = value.x;
            _positionInfo.PosY = value.y;

        }
    }


    public PositionInfo PositionInfo
    {
        get
        {
            return _positionInfo;
        }

        set
        {
            if (_positionInfo.Equals(value))//바뀐게 없음 
                return;

            _updated = true;//패킷 보내기 

            State = value.State;
            Dir = value.MoveDir;
            CellPos = new Vector3Int(value.PosX, value.PosY, 0);


            UpdateAnimation();//애니메이션 업데이트 
        }

    }

    public virtual CreatureState State //원본 데이터를 참조한다 
    {
        get { return PositionInfo.State; }
        set
        {
            if (_positionInfo.State == value)
                return;

            _updated = true;//패킷 보내기  

            _positionInfo.State = value;
            UpdateAnimation();
        }
    }

    public MoveDir Dir
    {
        get { return PositionInfo.MoveDir; }
        set
        {
            if (_positionInfo.MoveDir == value)
                return;

            _updated = true;//패킷 보내기

            _positionInfo.MoveDir = value;


            UpdateAnimation();
        }
    }

    public MoveDir GetDirFromVec(Vector3Int dir)
    {
        if (dir.x > 0)
            return MoveDir.Right;
        else if (dir.x < 0)
            return MoveDir.Left;
        else if (dir.y > 0)
            return MoveDir.Up;
        else
            return MoveDir.Down;
    }

    public Vector3Int GetFrontCellPos()
    {
        Vector3Int cellPos = CellPos;

        switch (Dir)
        {
            case MoveDir.Up:
                cellPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                cellPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                cellPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                cellPos += Vector3Int.right;
                break;
        }

        return cellPos;
    }

    

    void Start()
    {
        Init();
    }

    void Update()
    {
        UpdateController();
    }

    public virtual void Init()
    {
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();

        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;

        UpdateAnimation();
    }

    protected virtual void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Skill:
                UpdateSkill();
                break;
            case CreatureState.Dead:
                UpdateDead();
                break;
        }
    }

    protected virtual void UpdateIdle()
    {

    }

    //목적지를 설정하고 스르륵 이동함 
    protected virtual void UpdateMoving()
    {
        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        Vector3 moveDir = destPos - transform.position;

        // 도착 여부 체크
        float dist = moveDir.magnitude;
        if (dist < Stat.Speed * Time.deltaTime)
        {
            transform.position = destPos;
            MoveToNextPos();
        }
        else
        {
            transform.position += moveDir.normalized * Stat.Speed * Time.deltaTime;
            State = CreatureState.Moving;
        }
    }

    protected virtual void MoveToNextPos()
    {

    }

    protected virtual void UpdateSkill()
    {

    }

    protected virtual void UpdateDead()
    {

    }

    public virtual void OnDamaged()
    {

    }
}
