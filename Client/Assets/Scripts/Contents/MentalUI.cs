using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MentalUI : MonoBehaviour
{
    [SerializeField]
    Transform Mental;

    public void UpdateMentalUI(float ratio)
    {
        ratio = Math.Clamp(ratio, 0, 1);
        Mental.localScale = new Vector3(ratio, 1, 1);
     

    }
}
