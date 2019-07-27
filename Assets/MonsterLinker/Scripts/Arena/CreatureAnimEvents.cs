﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureAnimEvents : MonoBehaviour
{
    public AnimationHandler animationhandler;
    public AttackRoundHandler attackroundhandler;
    public QTEHandler qtehandler;

    //QTEStateSwitch(eQTEState QTEState)

    public void AttackAnimStart()
    {
        print("animation starts");
        if (qtehandler.curQTEType != eQTEType.FA)
        {
            StartCoroutine(qtehandler.WaitForStart());
        }
    }

    public void FAQTEcall(int FAnumber)
    {
        //qtehandler.AnimString = ("FA" + FAnumber);
        qtehandler.CallFAQTE("FA"+FAnumber);
        //qtehandler.WaitingTime = 0.0f;
        //StartCoroutine(qtehandler.WaitForStart());
    }

    public void FADmg()
    {
        switch (GameStateSwitch.Instance.GameState)
        {
            case eGameState.QTEAttack:
                GameStateSwitch.Instance.baeffectshandler.CalculatePlayerBaseDmg();

                break;
            case eGameState.QTEBlock:
                GameStateSwitch.Instance.baeffectshandler.CalculateEnemyBaseDmg();

                break;                
        }
        GameStateSwitch.Instance.baeffectshandler.DealDMG();
    }
        public void EnduranceStart()
    {
        print("start endurance button mash now");
        StartCoroutine(qtehandler.ButtonMash());
    }

    public void HitImpact()
    {
        float duration = Random.Range(0.15f, 0.25f);
        float magnitude = Random.Range(2.0f, 3.0f);

        //print("impact, calling animation");
        StartCoroutine(GameStateSwitch.Instance.camshake.Shake(duration, magnitude));
        animationhandler.HurtCheck();
    }

    public void SFXHeavy()
    {
        SoundController.Instance.StartSFX(SoundController.SFX.impact_heavy);
    }
    public void SFXNormal()
    {
        SoundController.Instance.StartSFX(SoundController.SFX.impact_normal);
    }
    public void SFXLight()
    {
        SoundController.Instance.StartSFX(SoundController.SFX.impact_light);
    }

    public void SFXEndurance()
    {
        //SoundController.Instance.StartLoopingSFX(SoundController.SFX.powerCharge1, 1.0f);
    }

    public void EndEndurance()
    {
        //SoundController.Instance.StopLoopingSound()
    }

    public void AttackAnimEnd()
    {
        print("animation end");
        attackroundhandler.NextAttack();
        //animationhandler.ResetToIdle();
    }    
}