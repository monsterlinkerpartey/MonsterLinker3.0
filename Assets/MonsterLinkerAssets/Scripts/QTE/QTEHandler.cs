﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// QTE Type enum
/// different ui objects for the different types
/// different animations for the different lengths
/// 
public class QTEHandler : MonoBehaviour
{
    [Header("The different Types of QTE")]
    public Animator AttackQTEAnim;
    public Animator BlockQTEAnim;
    public Animator EnduranceQTEAnim;
    public Animator FAQTEAnim;

    [Header("Length of the different attack kuh-teh-eh animations")]
    public float Attack1Length;
    public float Attack2Length;
    public float Attack3Length;

    [Header("Length of the different block kuh-teh-eh animations")]
    public float Block1Length;
    public float Block2Length;
    public float Block3Length;

    [Header("Length of the different FA kuh-teh-eh animations")]
    public float FA1Length;

    [Header("For the Button Input Randomizer")]
    public List<ButtonInput> Buttons = new List<ButtonInput>();
    public List<Vector2> ButtonPosVectors;

    public GameObject EnduranceText;
    public GameObject QTEButton;
    public RectTransform QTEButtonTransform;
    public Image ButtonImage;

    public GameObject EnduranceButton;
    public Animator EnduranceButtonAnim;
    public RectTransform EnduranceButtonTransform;
    public Image EnduranceButtonImage;

    public Text EnduranceCounter;
    public Text EnduranceCounter2;
    public Animator EnduranceTextAnim;

    [Tooltip("Random generated no. for button")]
    [SerializeField] int ran;
    [Tooltip("Random generated no. for animation")]
    [SerializeField] int ranAnim;

    public eQTEState QTEState;
    public eQTEType curQTEType;

    [Header("QTE Times for Attack")]
    public QTE Attack;
    [Header("QTE Times for Block")]
    public QTE Block;
    [Header("QTE Times for FA")]
    public QTE FA;

    [Header("Other stuff")]
    [Tooltip("Time til impact/startup of Monster animation, default 1.5s")]
    public float AnimStartup = 1.5f;
    //TODO get animation events for monster animations to check when they end/start
    [Tooltip("Time til QTE starts")]
    public float WaitingTime;
    //[SerializeField] float QTETimer;
    //[SerializeField] float QTEGood;
    //[SerializeField] float QTEPerfect;

    [SerializeField] Animator curQTEAnim;
    [HideInInspector] public QTE curQTE;

    [SerializeField] int MaxSlots;

    public eQTEInput QTEInput;

    //[Tooltip("Bool to start QTE Timer")]
    //[SerializeField] bool running;
    //[Tooltip("Bool to start Endurance QTE")]
    //[SerializeField] bool mashing;

    [Tooltip("Counter for the Endurance button mashing")]
    [SerializeField] int mashCounter = 0;
    [Tooltip("How long the Endurance QTE lasts")]
    [SerializeField] float EnduranceTime;
    public string AnimString;

    public bool EnduranceDone;

    public AttackRoundHandler attackroundhandler;
    public BAEffectsHandler baeffectshandler;
    public TurnChanger turnchanger;

    float QTESFXvolume = 0.8f;

    public void GetAnimClipTimes()
    {
        AnimationClip[] attackClips = AttackQTEAnim.runtimeAnimatorController.animationClips;
        AnimationClip[] blockClips = BlockQTEAnim.runtimeAnimatorController.animationClips;
        AnimationClip[] FAClips = FAQTEAnim.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in FAClips)
        {
            switch (clip.name)
            {
                case "FA1":
                    FA1Length = clip.length;
                    break;
                default:
                    //print("not saving time of " + clip.name);
                    break;
            }
        }

        foreach (AnimationClip clip in attackClips)
        {
            switch (clip.name)
            {
                case "Attack1":
                    Attack1Length = clip.length;
                    break;
                case "Attack2":
                    Attack2Length = clip.length;
                    break;
                case "Attack3":
                    Attack3Length = clip.length;
                    break;
                default:
                    //print("not saving time of " + clip.name);
                    break;
            }
        }

        foreach (AnimationClip clip in blockClips)
        {
            switch (clip.name)
            {
                case "Block1":
                    Block1Length = clip.length;
                    break;
                case "Block2":
                    Block2Length = clip.length;
                    break;
                case "Block3":
                    Block3Length = clip.length;
                    break;
                default:
                    //print("not saving time of " + clip.name);
                    break;
            }
        }
    }

    void Update()
    {
        switch (QTEInput)
        {
            case eQTEInput.None:
                break;
            case eQTEInput.QTE:
                CheckForInput();
                break;
            case eQTEInput.Endurance:
                CountButtonMash();
                break;
        }
    }

    public void SetType(eQTEType QTEType)
    {
        //GlobalVars.AttackRound = 1;
        //MaxSlots = maxSlots;
        curQTEType = QTEType;
        //print("QTE Type: " + QTEType);

        switch (QTEType)
        {
            case eQTEType.Attack:
                curQTEAnim = AttackQTEAnim;
                curQTE = Attack;
                break;
            case eQTEType.Block:
                curQTEAnim = BlockQTEAnim;
                curQTE = Block;
                break;
            case eQTEType.FAEndurance:
                curQTEAnim = EnduranceQTEAnim;
                break;
            case eQTEType.FA:
                EnduranceDone = false;
                curQTEAnim = FAQTEAnim;
                curQTE = FA;
                break;
            default:
                print("ERROR: QTEType not found, check QTEHandler");
                break;
        }
        RandomButtonGenerator();

        if (curQTEType != eQTEType.FAEndurance && curQTEType != eQTEType.FA)
        {            
            GetAnimClipTimes();
            QTEStateSwitch(eQTEState.Waiting);
            SetQTEAnim(curQTE.Type);            
        }        
        //else
        //{
        //    StartCoroutine(ButtonMash());
        //}
    }

    public IEnumerator ButtonMash()
    {
        QTEStateSwitch(eQTEState.Endurance);
        print("qte will last " + EnduranceTime + " secs");
        yield return new WaitForSeconds(EnduranceTime);
        QTEStateSwitch(eQTEState.Waiting);
        baeffectshandler.SetEnduranceModifier(mashCounter);
        EnduranceText.SetActive(false);
        EnduranceDone = true;
        attackroundhandler.StartAttack();
    }

    ///Randomizes the QTE Button Image and Input
    public void RandomButtonGenerator()
    {
        ran = Random.Range(0, Buttons.Count);
        //print("Chosen Button: " + Buttons[ran].name);

        if (Buttons.Count <= 0)
        {
            Debug.LogError("No buttons in list, check QTE Handler");
        }
        else
        {
            //set QTE Button position
            switch (curQTEType)
            {
                case eQTEType.Attack:
                    QTEButtonTransform.anchoredPosition = ButtonPosVectors[1];
                    break;
                case eQTEType.Block:
                    QTEButtonTransform.anchoredPosition = ButtonPosVectors[2];
                    break;
                case eQTEType.FAEndurance:
                    EnduranceButtonTransform.anchoredPosition = ButtonPosVectors[0];
                    break;
                case eQTEType.FA:
                    QTEButtonTransform.anchoredPosition = ButtonPosVectors[1];
                    break;
                default:
                    QTEButtonTransform.anchoredPosition = ButtonPosVectors[0];
                    break;
            }
            ButtonImage.sprite = Buttons[ran].buttonSprite;
            EnduranceButtonImage.sprite = Buttons[ran].buttonSprite;
        }
    }

    public void CountButtonMash()
    {
        float duration = Random.Range(0.15f, 0.25f);
        float magnitude = Random.Range(2.0f, 3.0f);

        if (Input.GetButtonDown(Buttons[ran].inputString))
        {
            SoundController.Instance.StartSFX(SoundController.SFX.endurance_mash2, 0.7f);
            //GameStateSwitch.Instance.camshake.Shake(0.10f, 1.75f);
            print("button " + Buttons[ran].name + " pressed");
            mashCounter += 1;
            MashCounterAnim();
            baeffectshandler.SetEnduranceModifier(mashCounter);
        }
    }

    public void MashCounterAnim()
    {
        float curDMG = Mathf.RoundToInt(baeffectshandler.curAttack.DMG + (baeffectshandler.curAttack.DMG * baeffectshandler.EnduranceModifier));
        EnduranceTextAnim.Play("mash");
        EnduranceButtonAnim.SetTrigger("Pressed");
        EnduranceCounter.text = ("" + curDMG);
        EnduranceCounter2.text = EnduranceCounter.text;
    }

    void CheckForInput()
    {
        if (Input.anyKey)
        {
            curQTEAnim.speed = 0.0f;

            if (Input.GetButtonDown(Buttons[ran].inputString))
            {
                print("kuhteheh button pressed");
                //ButtonAnim.Play("Highlighted");
                StartCoroutine(CheckQTEZone());
            }
            else
            {
                QTEAnimEvents.QTEZone = eQTEZone.Fail;
                StartCoroutine(CheckQTEZone());
            }
        }
    }

    //Called if player input happened
    //Or by QTEZoneReader if QTE has gone through without input
    public IEnumerator CheckQTEZone()
    {
        QTEInput = eQTEInput.None;
        curQTEAnim.speed = 1.0f;

        if (curQTEType == eQTEType.FA)
        {
            print("curQTE is FA, checking zones");
            switch (QTEAnimEvents.QTEZone)
            {
                case eQTEZone.None:
                    Debug.LogError("QTE Zone auf None, check Animation Events");
                    break;
                case eQTEZone.Fail:
                    SoundController.Instance.StartSFX(SoundController.SFX.qte_timing_fail, QTESFXvolume);
                    curQTEAnim.Play(curQTE.Type + "_Fail");
                    attackroundhandler.NoExtraSlot = true;
                    baeffectshandler.SetFA_QTEResultModifier(curQTE.ModifierFail);

                    break;
                case eQTEZone.Good:
                    curQTEAnim.Play(curQTE.Type + "_Good");
                    SoundController.Instance.StartSFX(SoundController.SFX.qte_newgood, 1f);
                    baeffectshandler.SetFA_QTEResultModifier(curQTE.ModifierGood);

                    break;
                case eQTEZone.Perfect:
                    curQTEAnim.Play(curQTE.Type + "_Perfect");
                    SoundController.Instance.StartSFX(SoundController.SFX.qte_timing_good, QTESFXvolume);
                    baeffectshandler.SetFA_QTEResultModifier(curQTE.ModifierPerfect);

                    break;
            }
        }
        else
        {
            print("curQTE is NOT FA, checking zones");
            switch (QTEAnimEvents.QTEZone)
            {
                case eQTEZone.None:
                    Debug.LogError("QTE Zone auf None, check Animation Events");
                    break;
                case eQTEZone.Fail:
                    //trigger fail anim
                    //do dmg stuff etc
                    SoundController.Instance.StartSFX(SoundController.SFX.qte_timing_fail, QTESFXvolume);
                    curQTEAnim.Play(curQTE.Type + "_Fail");
                    attackroundhandler.NoExtraSlot = true;
                    GameStateSwitch.Instance.playerCreatureanimevents.qteResult = 1;
                    GameStateSwitch.Instance.enemyCreatureanimevents.qteResult = 1;
                    baeffectshandler.SetQTEResultModifier(curQTE.ModifierFail, curQTE.RPGainFail);
                    print("fail QTE result");
                    break;
                case eQTEZone.Good:
                    //trigger good anim
                    //do dmg stuff etc
                    SoundController.Instance.StartSFX(SoundController.SFX.qte_newgood, 1f);
                    //curQTEAnim.speed = 1.0f;
                    curQTEAnim.Play(curQTE.Type + "_Good");
                    GameStateSwitch.Instance.playerCreatureanimevents.qteResult = 2;
                    GameStateSwitch.Instance.enemyCreatureanimevents.qteResult = 2;
                    baeffectshandler.SetQTEResultModifier(curQTE.ModifierGood, curQTE.RPGainGood);
                    print("good QTE result");
                    break;
                case eQTEZone.Perfect:
                    //trigger perfect anim
                    //do dmg stuff etc
                    SoundController.Instance.StartSFX(SoundController.SFX.qte_timing_good, QTESFXvolume);
                    //curQTEAnim.speed = 1.0f;
                    curQTEAnim.Play(curQTE.Type + "_Perfect");
                    GameStateSwitch.Instance.playerCreatureanimevents.qteResult = 3;
                    GameStateSwitch.Instance.enemyCreatureanimevents.qteResult = 3;
                    baeffectshandler.SetQTEResultModifier(curQTE.ModifierPerfect, curQTE.RPGainPerfect);
                    print("perfect QTE result");
                    break;
                default:
                    print("ERROR: Could not find QTEZone, check QTEHandler");
                    baeffectshandler.SetQTEResultModifier(curQTE.ModifierFail, curQTE.RPGainFail);
                    break;
            }
        }
        //wait for result animation to play
        yield return new WaitForSeconds(0.4f);
        //start new qte or set qte done
        QTEStateSwitch(eQTEState.Done);
            //GlobalVars.AttackRound += 1;
            //print("attackround: " + GlobalVars.AttackRound);
            //SetQTEAnim(curQTE.Type);
        }

        public void SetQTEAnim(string type)
    {
        switch (GameStateSwitch.Instance.GameState)
        {
            case eGameState.QTEAttack:

                //TODO animations randomizen
                ranAnim = Random.Range(1, 3);

                switch (ranAnim)
                {
                    case 1:
                        WaitingTime = AnimStartup - Attack1Length;
                        break;
                    case 2:
                        WaitingTime = AnimStartup - Attack2Length;
                        break;
                    case 3:
                        WaitingTime = AnimStartup - Attack3Length;
                        break;
                    default:
                        WaitingTime = 0f;
                        Debug.LogError("Could not set Wait Time, check QTEHandler");
                        break;
                }
                break;
            case eGameState.QTEBlock:

                //TODO animations randomizen
                ranAnim = Random.Range(1, 3);

                switch (ranAnim)
                {
                    case 1:
                        WaitingTime = AnimStartup - Block1Length;
                        break;
                    case 2:
                        WaitingTime = AnimStartup - Block2Length;
                        break;
                    case 3:
                        WaitingTime = AnimStartup - Block3Length;
                        break;
                    default:
                        WaitingTime = 0f;
                        Debug.LogError("Could not set Wait Time, check QTEHandler");
                        break;
                }
                break;
        }
        AnimString = type + ranAnim;
    }

    public void CallFAQTE(string animString)
    {
        print("calling fa qte " + animString);
        RandomButtonGenerator();
        AnimString = animString;
        QTEStateSwitch(eQTEState.Running);
    }

    public IEnumerator WaitForStart()
    {
        //print("kuhteheh waiting for " + WaitingTime + " s");
        yield return new WaitForSeconds(WaitingTime);
        QTEStateSwitch(eQTEState.Running);
    }

    public void QTEStateSwitch(eQTEState qteState)
    {
        QTEState = qteState;

        switch (qteState)
        {
            case eQTEState.Waiting:
                EnduranceCounter.text = "";
                EnduranceCounter2.text = "";
                curQTEAnim.Play("Wait");
                QTEButton.SetActive(false);
                EnduranceButton.SetActive(false);
                QTEInput = eQTEInput.None;
                break;
            case eQTEState.Running:
                print("kuhteheh running");
                //QTEInput = eQTEInput.QTE;                
                QTEButton.SetActive(true);
                curQTEAnim.SetTrigger(AnimString);
                break;
            case eQTEState.Endurance:
                print("endurance kuhteheh running");
                QTEInput = eQTEInput.Endurance;
                EnduranceButton.SetActive(true);
                EnduranceText.SetActive(true);
                curQTEAnim.SetTrigger("Endurance");
                break;
            case eQTEState.Done:
                EnduranceCounter.text = "";
                EnduranceCounter2.text = "";
                print("kuhteheh done");
                QTEInput = eQTEInput.None; 
                curQTEAnim.Play("Wait");
                QTEButton.SetActive(false);                
                EnduranceButton.SetActive(false);
                //RandomButtonGenerator();
                mashCounter = 0;
                break;
            default:
                Debug.LogError("QTE state not found, check QTEHandler");
                break;
        }
    }


        ///=============================================================

        //public eQTEState QTEState;

        //[Tooltip("Current attack for animation length")]
        //public Attack curAttack;
        //public float AnimSpeedModifier;
        //public Animator QTEAnim;

        //[Tooltip("Image of the centered Button")]
        //public Image QTEButtonSprite;
        //public Animator QTEButtonAnim;

        //[Header("Time til the QTE starts")]
        //public float WaitForQTEStart;
        //[Tooltip("The running timer for the QTE length, influenced by AnimationSpeedModifier")]
        //public float QTETimer;
        //[Header("QTE Result times")]
        //public float QTEFullTime = 120f;
        //public float QTEGoodTime = 60f;
        //public float QTEPerfectTime = 11f;

        //[Header("QTE Button List, drag n drop")]
        //[Tooltip("List of buttons to use randomly for this QTE")]
        //public List<ButtonInput> Buttons = new List<ButtonInput>();

        //[Tooltip("For the button randomizer")]
        //int ran;
        //public int AttackRound;
        //bool runTimer = false;

        /////QTETimer -= delta time
        /////if QTETimer > QTEFullTime-QTEGoodTime = early fail
        /////if QTETimer <= QTEFullTime-QTEGoodTime && > QTEPerfectTime = good result
        /////if QTETimer <=QTEPerfectTime && != 0 = perfect result

        //void Update()
        //{
        //    if (runTimer)
        //    {
        //        QTETimer -= Time.deltaTime;
        //        GetButtonInput();

        //        if (QTETimer <= 0.0f)
        //        {
        //            print("QTE late fail");
        //            QTEStateSwitch(eQTEState.Fail);
        //        }
        //    }
        //}

        //public void QTEStateSwitch(eQTEState QTEState)
        //{
        //    switch (QTEState)
        //    {
        //        case eQTEState.Waiting:
        //            SetButton();
        //            SetAnimationTimes();
        //            //run coroutine that waits for the specified amount of time
        //            StartCoroutine(Wait());        
        //            break;
        //        case eQTEState.Running:
        //            //start animation
        //            QTEAnim.Play("runQTE");
        //            runTimer = true;
        //            break;
        //        case eQTEState.Fail:
        //            QTEAnim.speed = 0;
        //            //do dmg stuff?
        //            QTEStateSwitch(eQTEState.Done);
        //            break;
        //        case eQTEState.Good:
        //            QTEAnim.speed = 0;
        //            //do dmg stuff?
        //            QTEStateSwitch(eQTEState.Done);
        //            break;
        //        case eQTEState.Perfect:
        //            QTEAnim.speed = 0;
        //            //do dmg stuff?
        //            QTEStateSwitch(eQTEState.Done);
        //            break;
        //        case eQTEState.Done:
        //            runTimer = false;
        //            //finishes qte, resets all values
        //            break;
        //        default:
        //            print("QTE state not found, check QTEHandler");
        //            break;           
        //    }
        //}        

        //public IEnumerator Wait()
        //{
        //    print("waiting for " + WaitForQTEStart+" s");
        //    yield return new WaitForSeconds(WaitForQTEStart);
        //    QTEStateSwitch(eQTEState.Running);
        //}

        //////called by gamestateswitch
        ////public void GetQTETimes(float fullLength, float goodTime, float PerfectTime)
        ////{
        ////    QTEFullTime = fullLength;
        ////    QTEGoodTime = goodTime;
        ////    QTEPerfectTime = PerfectTime;
        ////}

        //public void SetAnimationTimes()
        //{
        //    //read out which attackround -> switch
        //    switch(AttackRound)
        //    {
        //        case 1:
        //            AnimSpeedModifier = 1.1f;
        //            break;
        //        case 2:
        //            AnimSpeedModifier = 1.2f;
        //            break;
        //        case 3:
        //            AnimSpeedModifier = 1.3f;
        //            break;
        //        case 4:
        //            AnimSpeedModifier = 1.4f;
        //            break;
        //        case 5:
        //            AnimSpeedModifier = 1.5f;
        //            break;
        //        default:
        //            AnimSpeedModifier = 1.0f;
        //            print("AttackRound not correctly set, check QTE Handler");
        //            break;
        //    }

        //    print("AnimationSpeed = " + AnimSpeedModifier);
        //    //get time of full length and animation speed
        //    QTEAnim.SetFloat("animSpeed", AnimSpeedModifier);

        //    //get times of good and perfect result

        //    WaitForQTEStart = 2.5f - QTEFullTime;
        //    //get waiting time => curAttack.TimeTilStartup - QTEFullTime 
        //}

        //public void SetButton()
        //{
        //    ran = 0;

        //    if (Buttons.Count <= 0)
        //    {            
        //        Debug.LogError("ERROR: No buttons in list, check QTE Handler");
        //    }
        //    else
        //    {
        //        QTEButtonSprite.sprite = Buttons[ran].buttonSprite;
        //    }
        //}

        //bool GetButtonInput()
        //{
        //    if (Input.GetButtonDown(Buttons[ran].inputString))
        //    {
        //        print("qte button pressed");
        //        QTEButtonAnim.Play("Highlighted");
        //        QTEResult(QTETimer);
        //        return true;
        //    }
        //    else
        //        return false;
        //}

        //void QTEResult(float curTime)
        //{
        //    print("curTime :"+curTime);

        //    if (curTime > (QTEFullTime - QTEGoodTime))
        //    {
        //        print("QTE too early");
        //        QTEStateSwitch(eQTEState.Fail);
        //    }
        //    else if (curTime <= (QTETimer-QTEGoodTime) && curTime > (QTETimer - QTEPerfectTime))
        //    {
        //        print("QTE good");
        //        QTEStateSwitch(eQTEState.Good);
        //    }
        //    else if (curTime <= (QTETimer - QTEPerfectTime))
        //    {
        //        print("QTE perfect");
        //        QTEStateSwitch(eQTEState.Perfect);
        //    }
        //}
        //}
    //}
}