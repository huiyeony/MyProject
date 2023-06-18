using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class ItemController : BaseController
{
    public int TemplateId { get; set; }
    public int count;


    //radius = 50 
    public float MagnetStr { get; set; } = 3f;
    public int MagnetDir { get; set; } = 1; //1인력 /-1척력 

    Transform _trans;
    Rigidbody2D _rigid2d;
    bool _magnetInZone;
    Transform _magnetTrans;

    private void Start()
    {
        Init();
    }
    public override void Init()
    {
        _sprite = GetComponent<SpriteRenderer>();

        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;

        State = CreatureState.Idle;

        //xxxxbase.Init호출xxxx
    }





}

