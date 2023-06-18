﻿using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using static Define;

public class ArrowController : BaseController
{


	public override void Init()
	{
		switch (Dir)
		{
			case MoveDir.Up:
				transform.rotation = Quaternion.Euler(0, 0, 0);
				break;
			case MoveDir.Down:
				transform.rotation = Quaternion.Euler(0, 0, -180);
				break;
			case MoveDir.Left:
				transform.rotation = Quaternion.Euler(0, 0, 90);
				break;
			case MoveDir.Right:
				transform.rotation = Quaternion.Euler(0, 0, -90);
				break;
		}


		base.Init();


        State = CreatureState.Moving;
    

    }

	protected override void UpdateAnimation()
	{

	}

}
