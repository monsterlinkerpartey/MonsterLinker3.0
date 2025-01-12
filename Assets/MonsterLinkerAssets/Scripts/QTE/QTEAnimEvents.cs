﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Animation events for the QTE Zones
/// Attached to the QTE Animator
/// </summary>
public class QTEAnimEvents : MonoBehaviour
{
    public static eQTEZone QTEZone;
    [Tooltip("Drag n drop")]
    public QTEHandler qtehandler;

    bool start = false;
    float timer;

    public void Update()
    {
        //if (start)
        //{
        //    timer += Time.deltaTime;
        //}
    }

    public void ZStartQTE()
    {
        //start = true;
        QTEZone = eQTEZone.Fail;
        qtehandler.QTEInput = eQTEInput.QTE;
        print("cur Zone: " + QTEZone);
        //print("timer:" + timer);
    }

    public void ZGoodTime()
    {
        QTEZone = eQTEZone.Good;
        print("cur Zone: " + QTEZone);
        //print("timer:" + timer);
    }

    public void ZPerfectTime()
    {
        QTEZone = eQTEZone.Perfect;
        print("cur Zone: " + QTEZone);
        //print("timer:" + timer);
    }

    public void ZEndQTE()
    {
        //start = false;
        QTEZone = eQTEZone.Fail;
        StartCoroutine(qtehandler.CheckQTEZone());
        print("cur Zone: " + QTEZone);
        qtehandler.QTEInput = eQTEInput.None;
        //print("Full length: " + timer);
        //timer = 0f;
    }
}
