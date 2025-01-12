﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMenu : MonoBehaviour
{
    [Tooltip("Initiate Link Button at Game Start")]
    public Animator MonsterLinkerLogo;
    public GameObject TitleButton;
    public GameObject SaveLoadWindow;
    public GameObject InputPlayerNameWindow;
    public GameObject Save1;
    public GameObject Save2;
    public GameObject Save3;
    public InputField PlayerName;
    public Button curSaveButton;
    private PreLoadScript preloadscript;
    private TitleTutorial titletutorial;

    public void Start()
    {
        TitleButton.SetActive(true);
        SaveLoadWindow.SetActive(false);
        preloadscript = FindObjectOfType<PreLoadScript>();
        titletutorial = FindObjectOfType<TitleTutorial>();
        //WriteSaveData();
        SoundController.Instance.StartMenuMusic();
    }

    //Button of 1. screen
    public void InitiatingLink()
    {
        if (!DPadButtons.disabled)
        {
            SoundController.Instance.StartSFX(SoundController.SFX.ui_titlescreen);
            StartCoroutine(WaitForGlitchyButton());
        }
    }

    public IEnumerator WaitForGlitchyButton()
    {
        DPadButtons.disabled = true;
        TitleButton.GetComponentInChildren<Animator>().SetTrigger("Pressed");
        MonsterLinkerLogo.SetTrigger("fadeout");
        yield return new WaitForSeconds(1f);
        TitleButton.SetActive(false);
        DPadButtons.disabled = false;
        //call name prompt
        titletutorial.TriggerDialogue(0);
        //SaveLoadWindow.SetActive(true);
    }

    public void ConfirmPlayerName()
    {
        SoundController.Instance.StartSFX(SoundController.SFX.ui_select);
        if (string.IsNullOrWhiteSpace(PlayerName.text))
        {
            preloadscript.curSave.LinkerName = "Hoody";
        }
        else
        {
            preloadscript.curSave.LinkerName = PlayerName.text;
        }
        InputPlayerNameWindow.SetActive(false);
        titletutorial.TriggerDialogue(4);
        preloadscript.curSave.Empty = false;
    }



    //public void PressLoadButton()
    //{
    //    SoundController.Instance.StartSFX(SoundController.SFX.ui_loadoutEquip);
    //    SelectedSaveSlot();
    //    LoadSave(SelectedSaveSlot());
    //}

    //public void PressDeleteButton()
    //{
    //    SoundController.Instance.StartSFX(SoundController.SFX.ui_cancel);
    //    SelectedSaveSlot();
    //    DeleteSave(SelectedSaveSlot());
    //}

    //public void WriteSaveData()
    //{
    //    if (preloadscript.Save1.Empty)
    //    {
    //        Save1.GetComponentInChildren<Text>().text = "Save 1: New Game";
    //    }
    //    else
    //    {
    //        Save1.GetComponentInChildren<Text>().text = "" + preloadscript.Save1.LinkerName;
    //    }

    //    if (preloadscript.Save2.Empty)
    //    {
    //        Save2.GetComponentInChildren<Text>().text = "Save 2: New Game";
    //    }
    //    else
    //    {
    //        Save2.GetComponentInChildren<Text>().text = "" + preloadscript.Save2.LinkerName;
    //    }

    //    if (preloadscript.Save3.Empty)
    //    {
    //        Save3.GetComponentInChildren<Text>().text = "Save 3: New Game";
    //    }
    //    else
    //    {
    //        Save3.GetComponentInChildren<Text>().text = "" + preloadscript.Save3.LinkerName;
    //    }
    //}

    //public int SelectedSaveSlot()
    //{
    //    int SlotNo = 0;
    //    if (Save1 == EventSystem.current.currentSelectedGameObject.gameObject)
    //    {
    //        curSaveButton = Save1.GetComponentInChildren<Button>();
    //        SlotNo = 1;
    //    }
    //    if (Save2 == EventSystem.current.currentSelectedGameObject.gameObject)
    //    {
    //        curSaveButton = Save2.GetComponentInChildren<Button>();
    //        SlotNo = 2;
    //    }
    //    if (Save3 == EventSystem.current.currentSelectedGameObject.gameObject)
    //    {
    //        curSaveButton = Save3.GetComponentInChildren<Button>();
    //        SlotNo = 3;
    //    }
    //    print("slotno " + SlotNo);
    //    return SlotNo;
    //}

    //public void LoadSave(int slotNo)
    //{
    //    switch (slotNo)
    //    {
    //        case 1:
    //            preloadscript.curSave = preloadscript.Save1;                
    //            break;
    //        case 2:
    //            preloadscript.curSave = preloadscript.Save2;
    //            break;
    //        case 3:
    //            preloadscript.curSave = preloadscript.Save3;
    //            break;
    //        default:
    //            Debug.LogError("could not find save, loading default 1");
    //            preloadscript.curSave = preloadscript.Save1;
    //            break;
    //    }        

    //    SaveLoadWindow.SetActive(false);
    //    if (preloadscript.curSave.Empty)
    //    {
    //        titletutorial.TriggerDialogue(0);
    //    }
    //    else
    //    {
    //        titletutorial.TriggerDialogue(6);
    //    }
    //}

    //public void DeleteSave(int slotNo)
    //{
    //    switch (slotNo)
    //    {
    //        case 1:
    //            preloadscript.Save1.ResetSave();
    //            break;
    //        case 2:
    //            preloadscript.Save2.ResetSave();
    //            break;
    //        case 3:
    //            preloadscript.Save3.ResetSave();
    //            break;
    //        default:
    //            Debug.LogError("could not find save to delete");
    //            break;
    //    }
    //    WriteSaveData();
    //    curSaveButton.Select();
    //}
}
