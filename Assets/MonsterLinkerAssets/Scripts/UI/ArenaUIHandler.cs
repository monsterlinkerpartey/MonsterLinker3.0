﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArenaUIHandler : MonoBehaviour
{
    [Header("Initiative UI")]
    public GameObject IniArrow;
    public GameObject IniBG;
    public Text EnemySpeed;
    public Text PlayerSpeed;

    [Header("Drag n Drop Menus")]
    public GameObject CheatWindow;
    public GameObject FALoadout;
    public GameObject InputPanel;
    public GameObject PlayerInputBar;
    public GameObject EnemyInputBar;
    public GameObject StatusBars;
    public GameObject InitiativeCheck;
    public GameObject QTEPanel;
    public GameObject EndDemoText;
    public List<Button> BAInputButtons = new List<Button>(3);

    [Header("Health and Rage")]
    public Text PlayerHPtxt;
    public Text PlayerRPtxt;
    public Text EnemyHPtxt;
    public Text EnemyRPtxt;

    public Text EnemyName;
    public Text PlayerName;

    public Text DebugPlayerDMG;
    public Text DebugEnemyDMG;

    public Text PlayerDmgTaken;
    public Text PlayerDmgTaken2;
    public Text EnemyDmgTaken;
    public Text EnemyDmgTaken2;
    public Animator PlayerDmgTextAnim;    
    public Animator EnemyDmgTextAnim;    

    [Header("Buttons for BA Input")]
    public Button HeavyAttack;
    public Button NormalAttack;
    public Button LightAttack;
    public Button UM;

    [Header("Unleashede Mode")]
    public GameObject UM_Button;
    public GameObject UM_BuffIcon;
    public Image UM_IconImage;
    public List<Sprite> BuffIcons;
    public GameObject UM_Text;
    public Animator UM_healAnim;

    [Header("Super Feral Art")]
    public GameObject SFASlot;
    public Text SFA_Name;

    [Header("Result Screen")]
    public GameObject ResultPanel;
    public GameObject HomeButton;
    public GameObject RetryButton;
    public GameObject NextButton;
    public Text ResultText;
    public Text ResultNameText;

    [Header("Other")]
    public GameObject BaseAttackInfoPanel;
    public Text InfoButtonText;
    public Button ConfirmBAsButton;
    //public GameObject DmgCounter;

    [HideInInspector]
    //public AttackSlot[] playerSlots;
    public List<AttackSlot> playerSlots;
    [HideInInspector]
    public AttackSlot[] enemySlots;
    [HideInInspector]
    public InputBarHandler inputbarhandler;

    public bool hidden;

    public void Update()
    {
        switch (GameStateSwitch.Instance.GameState)
        {
            case eGameState.PlayerInput:
                if (inputbarhandler.PlayerAttackInput.Count < inputbarhandler.maxBaseAttackInputSlots)
                    CheckForPlayerBAInput();
                    CheckForPause();
                break;
            default:
                break;
        }
    }

    public void UseCheatWindow()
    {
        if (!DPadButtons.disabled)
        {
            SoundController.Instance.StartSFX(SoundController.SFX.ui_select);
            DPadButtons.disabled = true;
            CheatWindow.SetActive(true);
        }
        else
        {
            SoundController.Instance.StartSFX(SoundController.SFX.ui_cancel);
            DPadButtons.disabled = false;
            CheatWindow.SetActive(false);
        }      
    }

    public void UpdateUMBuff(int round)
    {
        SoundController.Instance.StartSFX(SoundController.SFX.ui_loadoutEquip);
        UM_BuffIcon.GetComponentInChildren<Animator>().SetTrigger("update");
        UM_IconImage.sprite = BuffIcons[round - 1];
    }

    public IEnumerator ShowDmgCounters(int DmgTaken)
    {
        if (GameStateSwitch.Instance.GameState == eGameState.QTEAttack)
        {
            EnemyDmgTaken.text = "-" + DmgTaken;
            EnemyDmgTaken2.text = "-" + DmgTaken;
            PlayerDmgTaken.text = "";
            PlayerDmgTaken2.text = "";
            EnemyDmgTextAnim.SetTrigger("dmg");
        }
        else if (GameStateSwitch.Instance.GameState == eGameState.QTEBlock)
        {
            PlayerDmgTaken.text = "-" + DmgTaken;
            PlayerDmgTaken2.text = "-" + DmgTaken;
            EnemyDmgTaken.text = "";
            EnemyDmgTaken2.text = "";
            PlayerDmgTextAnim.SetTrigger("dmg");
        }

        //DmgCounter.SetActive(true);
        yield return new WaitForSeconds(1f);
        //DmgCounter.SetActive(false);
    }

    public void SetSpeedValues(int eSpeed, int pSpeed)
    {
        EnemySpeed.text = ""+eSpeed;
        PlayerSpeed.text = ""+pSpeed;
    }

    public void SetIniArrow(string who)
    {
        IniArrow.GetComponentInChildren<Animator>().Play("play"+who);
    }

    public void SetIni_UM()
    {
        IniArrow.GetComponentInChildren<Animator>().Play("playp");        
    }

    public void SetPlayerHPandRP(int HP, int RP)
    {
        if (HP < 0)
        PlayerHPtxt.text = "0";
        else
        PlayerHPtxt.text = ""+HP;

        PlayerRPtxt.text = ""+RP;
    }

    public void SetEnemyHPandRP(int HP, int RP)
    {
        if (HP < 0)
            EnemyHPtxt.text = "0";
        else
            EnemyHPtxt.text = "" + HP;
        
        EnemyRPtxt.text = "" + RP;
    }

    public void CheckForPause()
    {
        if (Input.GetButtonDown("start"))
        {
            //TODO: write pause screen stuff
        }
        if (Input.GetButtonDown("select"))
        {

        }
    }

    public void WriteSFA_Info()
    {
        SoundController.Instance.StartSFX(SoundController.SFX.ui_loadoutEquip);
        SFA_Name.text = GameStateSwitch.Instance.implanthandler.SuperFeralArt.FAName;
        SFASlot.SetActive(true);
    }

    public void CheckForPlayerBAInput()
    {             
        if (DPadButtons.Down || Input.GetKeyDown(KeyCode.DownArrow))
        {
            SoundController.Instance.StartSFX(SoundController.SFX.ui_select);
            LightAttack.animator.SetTrigger("Pressed");
            LightAttack.onClick.Invoke();
        }

        if (DPadButtons.Right || Input.GetKeyDown(KeyCode.RightArrow))
        {
            SoundController.Instance.StartSFX(SoundController.SFX.ui_select);
            NormalAttack.animator.SetTrigger("Pressed");
            NormalAttack.onClick.Invoke();
        }

        if (DPadButtons.Up || Input.GetKeyDown(KeyCode.UpArrow))
        {
            SoundController.Instance.StartSFX(SoundController.SFX.ui_select);
            HeavyAttack.animator.SetTrigger("Pressed");
            HeavyAttack.onClick.Invoke();
        }
        if (DPadButtons.Left || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (GameStateSwitch.Instance.arenaui.UM_Button.GetComponentInChildren<Button>().interactable)
            {
                SoundController.Instance.StartSFX(SoundController.SFX.ui_select);
                UM.animator.SetTrigger("Pressed");
                UM.onClick.Invoke();
            }       
        }
    }

    public void GetAttackSlots()
    {
        print("getting attack slots");
        PlayerInputBar.GetComponentsInChildren<AttackSlot>(playerSlots);
        enemySlots = EnemyInputBar.GetComponentsInChildren<AttackSlot>();
    }

    public void SetConfirmButtonStatus(bool enabled)
    {
        //TODO: disable button anims somehow
        //ConfirmBAsButton.GetComponent<ButtonControl>().Bdisabled = enabled;
    }

    public void SetInputButtonsStatus(bool enabled)
    {
        print("input buttons enabled: " + enabled);
        HeavyAttack.enabled = enabled;
        LightAttack.enabled = enabled;
        NormalAttack.enabled = enabled;        
    }

    //TODO Show/Hide FA Info and Input Buttons
    public void ShowHideInputInfo()
    {
        //TODO: animate FA info and input buttons
        if (hidden)
        {
            //FAInfoAndInputButtons.SetActive(true);
        }
        else
        {
            //FAInfoAndInputButtons.SetActive(false);
        }
    }

    public void SlideInputBarIn(GameObject inputbar)
    {
        //TODO: animate inputbars
        inputbar.SetActive(true);
    }

    public void SlideInputBarOut(GameObject inputbar)
    {
        inputbar.SetActive(false);
    }

    //player input shown in ui
    public void UpdatePlayerInput(List<BaseAttack> attacks)
    {
        //playerSlots[position].AddCombo(attack);

        print("updating icon slots");

        for (int i = 0; i < playerSlots.Count; i++)
        {
            if (i < attacks.Count)
            {
                playerSlots[i].AddCombo(attacks[i]);
            }
            else
            {
                playerSlots[i].ClearSlot();
            }
        }
    }

    //player input shown in ui
    public void UpdateEnemyInput(List<BaseAttack> attacks)
    {
        //playerSlots[position].AddCombo(attack);

        print("updating icon slots");

        for (int i = 0; i < enemySlots.Length; i++)
        {
            if (i < attacks.Count)
            {
                enemySlots[i].AddCombo(attacks[i]);
            }
            else
            {
                enemySlots[i].ClearSlot();
            }
        }
    }

    public void VisializeFAs(List<int> Positions, Color thiscolor)
    {
        foreach (int i in Positions)
        {
            AttackSlot slot = playerSlots[i].GetComponentInChildren<AttackSlot>();
            slot.icon.color = thiscolor;
        }
    }

    public void ResetBAColours(Color thiscolor)
    {
        foreach (AttackSlot slot in playerSlots)
        {
            slot.icon.color = thiscolor;
        }
    }
}
