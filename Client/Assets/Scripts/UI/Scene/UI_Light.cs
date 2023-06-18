using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class UI_Light : UI_Base
{
    const int secondsInDay = 86400; 
    [SerializeField] Color nightLightColor;
    [SerializeField] AnimationCurve nightTimeCurve;
    [SerializeField] Color dayLightColor = Color.white;

    [SerializeField] float timeScale = 180f;
    [SerializeField] Light2D globalLight;

    TimeInfo _timeInfo = new TimeInfo();//원본 데이터 
    public TimeInfo TimeInfo
    {
        get { return _timeInfo; }
        set { Init(); _timeInfo.MergeFrom(value); }
    }

    public float Time
    {
        get { return TimeInfo.Time; }
        set { Init();  TimeInfo.Time = value; }
    }
    public int Days
    {
        get { return TimeInfo.Days; }
        set { Init();  TimeInfo.Days = value; }
    }

    public float Hours
    {
        get
        {
            return Time / 3600f;
        }
  
    }


   
    private void Update()
    {

        if (!_init) return;
        //이전 시간에서 부터~ 
        Time += UnityEngine.Time.deltaTime * timeScale;
        int hh = (int)Hours;
        float v = nightTimeCurve.Evaluate(Hours); //시간의 커브값 받기

        Color c = Color.Lerp(dayLightColor, nightLightColor, v); // (a,b,r) a와 b사이의 t(0~1) 비율에 위치한 색반환
        globalLight.color = c;
        if (Time > secondsInDay)
        {
            NextDay();

        }

    }
    private void NextDay()
    {
        Time = 0;
        Days += 1;
        //다음날이 되었어용
        CNextDay nextDayPkt = new CNextDay();
        int days = Managers.Light.LightUI.Days;
        float time = Managers.Light.LightUI.Time;

        nextDayPkt.TimeInfo = new TimeInfo() { Days = days, Time = time };
        Managers.Network.Send(nextDayPkt);


    }
    bool _init;
    public override void Init()
    {
        if(!_init)
            _init = true;//초기화 했음 
    }
}


