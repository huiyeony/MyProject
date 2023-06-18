using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpUI : MonoBehaviour
{
    [SerializeField]
    Transform Hp;

    public void UpdateHpUI(float ratio)
    {
        ratio = Math.Clamp(ratio, 0, 1);
        Hp.localScale = new Vector3(ratio, 1, 1);
        Debug.Log(ratio);
        
    }
}
