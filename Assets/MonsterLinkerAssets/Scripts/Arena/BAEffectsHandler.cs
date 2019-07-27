﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BAEffectsHandler : MonoBehaviour
{
    //[Tooltip("Is it the players turn - kinda redundant -> block oder attack state?")]
    //public bool Playerturn;

    public float maxPlayerHP;
    public float curPlayerHP;
    public float curPlayerRP;

    public float maxEnemyHP;
    public float curEnemyHP;
    public float curEnemyRP;

    public float curDMG;
    public int RPgained;

    [Header("At the end of one attack round")]
    public float TotalDmgTaken;
    public float TotalDmgDealt;

    [HideInInspector] public Attack curAttack;

    [HideInInspector] public ArenaUIHandler arenaui;
    [HideInInspector] public StatusBarHandler enemystatusbar;
    [HideInInspector] public StatusBarHandler playerstatusbar;
    [HideInInspector] public ImplantHandler implanthandler;

    [Header("Set by QTE Handler")]
    [Tooltip("Influenced by the QTE result")]
    public float QTEResultModifier;
    [Tooltip("Influenced by the Endurance QTE result")]
    public float EnduranceModifier;
    [Tooltip("Influenced by Implant choice")]
    public float ImplantModifier;
    [Tooltip("Damage modifier of the current enemy")]
    public float EnemyDMGModifier;
    [Tooltip("How many RP enemy gains for every hit taken")]
    public int EnemyRPgain;

    //TODO shove all these to the implantshandler
    //[Tooltip("If Rising Rage is active")]
    //public float RisingRageModifier = 1.0f;
    //[Header("For Rising Rage")]
    //public float PlayerRPatAttackStart;
    //[Tooltip("Multiplier for percentage, e.g. x2 -> 1RP gains 2Dmg")]
    //public float RRPercentage;
    ////Set by GameStateSwitch during Ini Check
    //public List<Attack> curEnemyAttacks;
    //public List<Attack> curPlayerAttacks;

    public void StartHpandRPValues(float playerHP, int playerRP, float enemyHP, int enemyRP, int enemyRPgain, float enemyDMGModifier)
    {
        maxPlayerHP = playerHP;
        curPlayerHP = playerHP;
        curPlayerRP = playerRP;

        maxEnemyHP = enemyHP;
        curEnemyHP = enemyHP;
        curEnemyRP = enemyRP;
        EnemyRPgain = enemyRPgain;
        EnemyDMGModifier = enemyDMGModifier;
    }

    public void SetImplantModifier(float implantModifier)
    {
        ImplantModifier = implantModifier;
    }

    public void SetEnduranceModifier(int mashCount)
    {
        EnduranceModifier = (float)mashCount / 100.0f;
        //return Mathf.RoundToInt(curAttack.DMG + (curAttack.DMG * EnduranceModifier));
    }

    public void SetFA_QTEResultModifier(float dmgModifier)
    {
        QTEResultModifier += dmgModifier;
    }

    public void SetQTEResultModifier(float dmgModifier, int rpggained)
    {
        QTEResultModifier = dmgModifier;
        RPgained = rpggained;
    }

    public void CalculatePlayerBaseDmg()
    {
        float tempBaseDMG1 = curAttack.DMG + (curAttack.DMG * ImplantModifier);               
        float tempBaseDMG2 = tempBaseDMG1 + (tempBaseDMG1 * EnduranceModifier);
        curDMG = tempBaseDMG2 * QTEResultModifier;
    }

    public void CalculateEnemyBaseDmg()
    {
        float tempBaseDMG = curAttack.DMG + (curAttack.DMG * EnemyDMGModifier);
        curDMG = tempBaseDMG - (tempBaseDMG * QTEResultModifier);
    }

    public void DealDMG()
    {
        switch (GameStateSwitch.Instance.GameState)
        {
            case eGameState.QTEAttack:
                CalculatePlayerBaseDmg();
                EnemyTakesDmg(curDMG, EnemyRPgain);
                PlayerPaysRP();

                break;
            case eGameState.QTEBlock:
                CalculateEnemyBaseDmg();
                PlayerTakesDmg(curDMG, RPgained);
                EnemyPaysRP();

                break;
        }

        HPandRPClamp();
        UpdateHPandRPCounter();
        UpdateHPandRPbars();
    }

    public void PlayerPaysRP()
    {
        curPlayerRP -= curAttack.RPCost;
        playerstatusbar.RPTick(Mathf.RoundToInt(curPlayerRP));
    }

    public void EnemyPaysRP()
    {
        curEnemyRP -= curAttack.RPCost;
        enemystatusbar.RPTick(Mathf.RoundToInt(curEnemyRP));
    }

    public void PlayerTakesDmg(float curDMG, int RPgained)
    {
        print("dealing dmg to player");
        curPlayerHP -= curDMG;
        curPlayerRP += RPgained;
        curEnemyHP += curAttack.HPGain;
        TotalDmgTaken += curDMG;

        //StartCoroutine(arenaui.ShowDmgCounters(Mathf.RoundToInt(curDMG)));
        //GameStateSwitch.Instance.statusbarhandler.LerpPlayerHP();
        //arenaui.SetPlayerHPandRP(Mathf.RoundToInt(curPlayerHP), Mathf.RoundToInt(curPlayerRP));
        //arenaui.SetEnemyHPandRP(Mathf.RoundToInt(curEnemyHP), Mathf.RoundToInt(curEnemyRP));
        //print("Player takes " + (curDMG) + " DMG\n Enemy gains " + curAttack.RPGain + " RP");
        //print("Player HP: " + curPlayerHP + ", Player RP: " + curPlayerRP +", Enemy HP: " + curEnemyHP+", Enemy RP: " + curEnemyRP);
    }

    public void EnemyTakesDmg(float curDMG, int RPgained)
    {
        curEnemyHP -= curDMG;
        curEnemyRP += RPgained;
        curPlayerHP += curAttack.HPGain;
        TotalDmgDealt += curDMG;

        //print("dealing dmg to enemy");
        //curEnemyHP -= curDMG;
        //curPlayerHP += curAttack.HPGain;
        //StartCoroutine(arenaui.ShowDmgCounters(Mathf.RoundToInt(curDMG)));
        //TotalDmgDealt += curDMG;
        //arenaui.SetPlayerHPandRP(Mathf.RoundToInt(curPlayerHP), Mathf.RoundToInt(curPlayerRP));
        //arenaui.SetEnemyHPandRP(Mathf.RoundToInt(curEnemyHP), Mathf.RoundToInt(curEnemyRP));
        //print("Enemy takes " + (curDMG) + " DMG\n Player gains " + curAttack.RPGain + " RP");
        //print("Player HP: " + curPlayerHP + ", Player RP: " + curPlayerRP + ", Enemy HP: " + curEnemyHP+", Enemy RP: " + curEnemyRP);
    }

    //Make sure HP and RP cannot go over their max value
    public void HPandRPClamp()
    {
        if (Mathf.Round(curEnemyHP) >= Mathf.Round(maxEnemyHP))
            curEnemyHP = Mathf.Round(maxEnemyHP);

        if (Mathf.Round(curPlayerHP) >= Mathf.Round(maxPlayerHP))
            curPlayerHP = Mathf.Round(maxPlayerHP);

        if (Mathf.RoundToInt(curEnemyRP) >= (int)100)
            curEnemyRP = 100.0f;

        if (Mathf.RoundToInt(curPlayerRP) >= (int)100)
            curPlayerRP = 100.0f;
    }

    public void UpdateHPandRPCounter()
    {
        int curEnemyHP_percent = Mathf.RoundToInt(((curEnemyHP/maxEnemyHP) *100));
        int curPlayerHP_percent = Mathf.RoundToInt(((curPlayerHP/maxPlayerHP) *100));

        arenaui.SetPlayerHPandRP(curPlayerHP_percent, Mathf.RoundToInt(curPlayerRP));
        arenaui.SetEnemyHPandRP(curEnemyHP_percent, Mathf.RoundToInt(curEnemyRP));
    }

    public void UpdateHPandRPbars()
    {
        playerstatusbar.HPTick(Mathf.Round(curPlayerHP));
        playerstatusbar.RPTick(Mathf.RoundToInt(curPlayerRP));
        enemystatusbar.HPTick(Mathf.Round(curEnemyHP));
        enemystatusbar.RPTick(Mathf.RoundToInt(curEnemyRP));
    }

    public void ShowTotalDmg(float totaldmg)
    {
        //TODO show total damge dealt and taken at the end of the round
        print("total damage this round: " + totaldmg);
    }

    public void ResetDmgCount()
    {
        TotalDmgTaken = 0f;
        TotalDmgDealt = 0f;

        EnduranceModifier = 0f;
        ImplantModifier = 0f;
        QTEResultModifier = 0f;
    }

    public void CheckForDeath()
    {
        if (Mathf.RoundToInt(curEnemyHP) > (int)0 && Mathf.RoundToInt(curPlayerHP) > (int)0)
        {
            GameStateSwitch.Instance.FightResult = eFightResult.None;
            print("fight state: " + GameStateSwitch.Instance.FightResult);
        }
        else if (Mathf.RoundToInt(curEnemyHP) <= (int)0)
        {
            GameStateSwitch.Instance.FightResult = eFightResult.Victory;
            print("fight state: " + GameStateSwitch.Instance.FightResult);
            GameStateSwitch.Instance.SwitchState(eGameState.Result);
            return;
        }
        else if (Mathf.RoundToInt(curPlayerHP) <= (int)0)
        {
            GameStateSwitch.Instance.FightResult = eFightResult.Defeat;
            print("fight state: " + GameStateSwitch.Instance.FightResult);
            GameStateSwitch.Instance.SwitchState(eGameState.Result);
            return;
        }
    }
    //    if (Mathf.Round(hitpoints) <= 0)
    //{
    //    //TODO: set result screen to open up
    //    switch (gameState)
    //    {
    //        case eGameState.QTEAttack:
    //            print("enemy died");
    //            GameStateSwitch.Instance.FightResult = eFightResult.Victory;

    //            break;
    //        case eGameState.QTEBlock:
    //            print("player died");
    //            GameStateSwitch.Instance.FightResult = eFightResult.Defeat;

    //            break;
    //        default:
    //            Debug.LogError("I dunno who died, check BAEffectsHandler");
    //            GameStateSwitch.Instance.FightResult = eFightResult.None;
    //            break;
    //    }

    //    GameStateSwitch.Instance.SwitchState(eGameState.Result);
    //}
    //}
    //HACK possible needs to be rewritten later when FAs are implemented
    //public void GetAttackLists(List<Attack> Playerlist, List<Attack> Enemylist)
    //{
    //    curPlayerAttacks = Playerlist;
    //    //foreach (Attack attack in Playerlist)
    //    //{
    //    //    curPlayerAttacks.Add(attack);
    //    //}

    //    foreach (Attack attack in Enemylist)
    //    {
    //        curEnemyAttacks.Add(attack);
    //    }
    //}

    //    public void DMGModification(float dmgModifier, int RPgained)
    //{
    //    QTEResultModifier = dmgModifier;        

    //    if (Mathf.RoundToInt(EnduranceModifier) <= 0)
    //        EnduranceModifier = Mathf.Round(1);

    //    if (GameStateSwitch.Instance.Implant == eImplant.RisingRage)
    //    {            
    //        RisingRageModifier = 1 + (PlayerRPatAttackStart * (RRPercentage / 100));
    //        print("rising rage active: x" + RisingRageModifier + " dmg");
    //    }
    //    else
    //        RisingRageModifier = 1;

    //    switch (GameStateSwitch.Instance.GameState)
    //    {
    //        case eGameState.QTEAttack:
    //            float curDMG = ((curAttack.DMG * RisingRageModifier) * EnduranceModifier) * QTEResultModifier;
    //            EnemyTakesDmg(Mathf.Round(curDMG));
    //            break;
    //        case eGameState.QTEBlock:
    //            curDMG = curAttack.DMG * QTEResultModifier;
    //            PlayerTakesDmg(Mathf.Round(curDMG), RPgained);
    //            break;
    //        default:
    //            break;
    //    }        
    //}
}