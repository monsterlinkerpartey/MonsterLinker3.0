﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handling the Fight rounds, calling animations and qtes and damage
public class AttackRoundHandler : MonoBehaviour
{
    public List<Attack> curAttackList;
    public Attack curAttack;
    public int maxRounds;
    public int curRound;

    public AnimationHandler animationhandler;
    public QTEHandler qtehandler;
    public BAEffectsHandler baeffectshandler;
    public TurnChanger turnchanger;

    public bool NoExtraSlot = false;

    //Called by GameStateSwitch, depending on state gives enemy or player attack list
    public void GetAttackList(List<Attack> curAttacks)
    {
        //curAttackList.Clear(); >>> Throws error in 2nd round for monster that attacked 2nd in 1st round
        curAttackList = curAttacks;
        maxRounds = curAttackList.Count;
        curRound = 1;
        curAttack = curAttackList[curRound-1];

        SetEffectValues();
        baeffectshandler.deathCheck = true;
        StartAttack();
    }   
    
    public void NextAttack()
    {
        baeffectshandler.deathCheck = false;
        baeffectshandler.QTEResultModifier = 0.0f;
        print("attack " + curAttack.name + " done");
        if (curRound < maxRounds)
        {
            print("next attack");
            curRound += 1;
            curAttack = curAttackList[curRound-1];
            //if this is the last attack
            if (curRound == maxRounds)
            {
                print("check for death after this attack");        
                baeffectshandler.deathCheck = true;
            }
            SetEffectValues();
            StartAttack();
        }

        else
        {
            baeffectshandler.LerpHP();
            print("turn done");
            CheckForTurn();
        }
    }

    public void CheckForTurn()
    {
        //baeffectshandler.CheckForDeath();

        if (GameStateSwitch.Instance.FightResult == eFightResult.None)
        {
            if (turnchanger.Turns == eTurn.EnemyFirst)
            {
                StartCoroutine(turnchanger.SwitchTurn(eTurn.PlayerSecond));
            }
            else if (turnchanger.Turns == eTurn.PlayerFirst)
            {
                StartCoroutine(turnchanger.SwitchTurn(eTurn.EnemySecond));
            }
            else
            {
                StartCoroutine(turnchanger.SwitchTurn(eTurn.BothDone));
            }
        }
    }

    //Called by GameStateSwitch at the beginning of a turn
    public void StartAttack()
    {
        switch (GameStateSwitch.Instance.GameState)
        {
            case eGameState.QTEAttack:
                //baeffectshandler.Playerturn = true;

                switch (curAttack.AttackType)
                {
                    case eAttackType.FA:  
                        if (!qtehandler.EnduranceDone)
                        {
                            //endurance qte callen
                            qtehandler.SetType(eQTEType.FAEndurance);
                            animationhandler.EnduranceAnimStart();
                        }
                        else
                        {
                            //FA QTE und animation callen
                            animationhandler.EnduranceAnimEnd();
                            //baeffectshandler.PlayerPaysRP();
                            qtehandler.SetType(eQTEType.FA);
                            animationhandler.PlayerAttack(curAttack.AnimationName);
                        }    
                        break;
                    case eAttackType.BA:
                        qtehandler.SetType(eQTEType.Attack);
                        animationhandler.PlayerAttack(curAttack.AnimationName);
                        break;
                    default:
                        Debug.LogError("Attack Type not specified!");
                        break;
                }
                break;
            case eGameState.QTEBlock:
                if (curAttack.AttackType == eAttackType.FA)
                    baeffectshandler.EnemyPaysRP();

                animationhandler.PlayerAnim.SetBool("block", true);
                //baeffectshandler.Playerturn = false;
                qtehandler.SetType(eQTEType.Block);       
                animationhandler.EnemyAttack(curAttack.AnimationName);

                break;
            default:
                Debug.LogError("not the right game state!");
                break;
        }
    }

    public void SetEffectValues()
    {
        baeffectshandler.curAttack = curAttack;
    }

    //TODO:
    //- attacklist von feralartcheck/enemystatemachine für wieviele "runden"
    //- auslesen aus curattack: 
    //      - BA oder FA
    //      - animation: an animationshandler
    //      - base effects: an baeffectshandler
    //      - qtehandler: info geben ob block/attack, BA/FA
    //      - nach letzter runde an turnchanger weitergeben

    //    if (turnchanger.Turns == eTurn.EnemyFirst)
    //{
    //    turnchanger.SwitchTurn(eTurn.PlayerSecond);
    //}
    //else if (turnchanger.Turns == eTurn.PlayerFirst)
    //{
    //    turnchanger.SwitchTurn(eTurn.EnemySecond);
    //}
    //else
    //{
    //    turnchanger.SwitchTurn(eTurn.BothDone);
    //}


}
