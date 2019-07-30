﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImplantHandler : MonoBehaviour
{
    [Header("For Rising Rage")]
    [Tooltip("Save of the RP count at the start of each attack round")]
    public float PlayerRPatAttackStart;
    [Tooltip("% more BaseDmg for one RP")]
    public float OneRP_Multiplier;

    [Header("For Unleashed Mode")]
    public int UMrounds;
    public bool UMbuttonpressed = false;
    [Tooltip("% of Player max HP threshold for activation of UM")]
    public float UMPercent = 0.25f;
    public float UM_oneTimeHeal_Value;
    public float UM_DMGDealt_Multiplier;
    public float UM_DMGTaken_Reduction;

    public eUnleashedMode Unleashed = eUnleashedMode.sleeping;

    [Header("For Super Feral Art")]
    public FeralArt SuperFeralArt;
    public eSuperFeralArt SuperFA = eSuperFeralArt.sleeping;

    [Header("For Temporary Input Slot")]
    public bool NoExtraSlot;

    //check which implant is currently on
    //check if SFA or UM have been used
    //check the rounds UM has been active
    //check modifiers of the implants
    //one dmg modifier in baeffects only, it is set by the current implant?    

    public void ResetCounters()
    {
        PlayerRPatAttackStart = 0f;
        UMrounds = 0;
        NoExtraSlot = false;
    }

    public float RPMultiplier()
    {
        float value = (OneRP_Multiplier * PlayerRPatAttackStart);
        return value;
    }

    public IEnumerator UMHeal()
    {
        print("healing player by" + UM_oneTimeHeal_Value);
        float animationTime = 1.5f;
        GameStateSwitch.Instance.animationhandler.PlayerUMActivation();
        GameStateSwitch.Instance.baeffectshandler.HealPlayer(UM_oneTimeHeal_Value);
        yield return new WaitForSeconds(animationTime);
        GameStateSwitch.Instance.initiativecheck.UMIni();
    }

    public void UMRoundCounter()
    {
        UMrounds += 1;
    }

    //called in nextround state
    public void ImplantCheck()
    {
        print("current Implant: " + GameStateSwitch.Instance.Implant);

        switch (GameStateSwitch.Instance.Implant)
        {
            case eImplant.UnleashedMode:
                switch (Unleashed)
                {
                    case eUnleashedMode.sleeping:
                        if ((GameStateSwitch.Instance.baeffectshandler.curPlayerHP <= (GameStateSwitch.Instance.baeffectshandler.maxPlayerHP * UMPercent)))
                        {
                            print("UM available now");
                            GameStateSwitch.Instance.arenaui.UMButton.SetActive(true);
                            Unleashed = eUnleashedMode.available;
                        }
                        break;
                    case eUnleashedMode.available:                        
                        break;
                    case eUnleashedMode.active:
                        //show UM symbol
                        GameStateSwitch.Instance.arenaui.UMIcon.SetActive(true);
                        print("UM round " + UMrounds);                        
                        //update UM counter
                        GameStateSwitch.Instance.arenaui.UMCounter.text = ""+UMrounds;
                        if (UMrounds > 3)
                        {
                            //disable UM symbol
                            GameStateSwitch.Instance.arenaui.UMIcon.SetActive(false);
                            Unleashed = eUnleashedMode.done;
                        }
                        break;
                    case eUnleashedMode.done:
                        break;
                    default:
                        Debug.LogError("UM state not found");
                        break;
                }
                break;

            case eImplant.SuperFA:

                switch (SuperFA)
                {
                    case eSuperFeralArt.sleeping:
                        if ((GameStateSwitch.Instance.baeffectshandler.curPlayerHP <= (GameStateSwitch.Instance.baeffectshandler.maxPlayerHP * UMPercent)))
                        {
                            SuperFA = eSuperFeralArt.available;
                        }
                        break;
                    case eSuperFeralArt.available:
                        //show SFA in feral art list
                        print("Super FA is available for use");

                        break;
                    case eSuperFeralArt.used:
                        //disable SFA from feral art list
                        SuperFA = eSuperFeralArt.done;
                        break;
                    case eSuperFeralArt.done:
                        break;
                    default:
                        break;
                }
                break;

            case eImplant.TempInputSlot:
                //If there are six slots and no QTEs have been failed
                if (GameStateSwitch.Instance.arenaui.playerSlots.Count == 6 && !GameStateSwitch.Instance.attackroundhandler.NoExtraSlot)
                    return;              
                
                switch (GameStateSwitch.Instance.arenaui.playerSlots.Count)
                {
                    case 5:
                        //QTE(s) have been failed
                        if (GameStateSwitch.Instance.attackroundhandler.NoExtraSlot)
                        {
                            //do nothing, everything is as it should be
                        }
                        else
                        {
                            //Spawn the sixth slot and update UI
                            print("spawning extra slot");
                            GameStateSwitch.Instance.attackslotspawn.SpawnTemporarySlot();
                            GameStateSwitch.Instance.arenaui.GetAttackSlots();
                        }
                        break;

                    case 6:
                        //QTE(s) have been failed
                        if (GameStateSwitch.Instance.attackroundhandler.NoExtraSlot)
                        {
                            //Delete the sixth slot and update UI
                            print("deleting extra slot");
                            GameStateSwitch.Instance.attackslotspawn.DestroyTemporarySlot();
                            GameStateSwitch.Instance.arenaui.GetAttackSlots();
                        }
                        else
                        {
                            //do nothing, everything is as it should be
                        }
                        break;

                    default:
                        Debug.LogError("Something went wrong with the input slot number");
                        break;
                }
                break;

            case eImplant.RisingRage:
                break;

            default:
                Debug.LogWarning("Implant not found: " + GameStateSwitch.Instance.Implant);
                break;
        }
    }

}
