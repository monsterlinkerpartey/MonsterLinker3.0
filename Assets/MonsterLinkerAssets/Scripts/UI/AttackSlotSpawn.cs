﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Spawns the attack slots for the input bars
/// </summary>
public class AttackSlotSpawn : MonoBehaviour
{
    [Tooltip("Gameobject the slots will be spawned on")]
    public GameObject PlayerInputBar;
    public GameObject EnemyInputBar;

    [SerializeField]
    private RectTransform PlayerPanelTransform;
    [SerializeField]
    private RectTransform EnemyPanelTransform;
    public GameObject Slot;
    [SerializeField]
    private GridLayoutGroup PlayerGrid;
    [SerializeField]
    private GridLayoutGroup EnemyGrid;
    public int NumberOfAttackSlotsPlayer;
    public int NumberOfAttackSlotsEnemy;

    [SerializeField] Vector3 IconScale = new Vector3(1f,1f,1f);

    GameObject ExtraSlot;

    [SerializeField]
    private float PanelHeight;
    [SerializeField]
    private float PanelWidth;

    public void Setup(int playerAttackNo, int enemyAttackNo)
    {
        NumberOfAttackSlotsPlayer = playerAttackNo;
        PlayerPanelTransform = PlayerInputBar.GetComponent<RectTransform>();
        PlayerGrid = PlayerInputBar.GetComponent<GridLayoutGroup>();

        NumberOfAttackSlotsEnemy = enemyAttackNo;
        EnemyPanelTransform = EnemyInputBar.GetComponent<RectTransform>();
        EnemyGrid = EnemyInputBar.GetComponent<GridLayoutGroup>();
    }

    public void SpawnTemporarySlot()
    {
        //Checking grid size and adjusting panel size to it
        float cellsize = PlayerGrid.cellSize.x;
        float spacing = PlayerGrid.spacing.x;
        if (spacing == 0)
            spacing = 1;

        PanelWidth = (((NumberOfAttackSlotsPlayer + 1) * cellsize) + (NumberOfAttackSlotsPlayer + 2) * (spacing));
        PanelHeight = 2 * spacing + cellsize;
        PlayerPanelTransform.sizeDelta = new Vector2(PanelWidth, PanelHeight);

        ExtraSlot = GameObject.Instantiate(Slot, transform.position, transform.rotation) as GameObject;
        ExtraSlot.transform.parent = PlayerInputBar.transform;
        ExtraSlot.transform.localScale = IconScale;
    }

    public void DestroyTemporarySlot()
    {
        Destroy(ExtraSlot);
    }

    public void SpawnPlayerSlots()
    {
        //Checking grid size and adjusting panel size to it
        float cellsize = PlayerGrid.cellSize.x;
        float spacing = PlayerGrid.spacing.x;
        if (spacing == 0)
            spacing = 1;

        PanelWidth = ((NumberOfAttackSlotsPlayer * cellsize) + (NumberOfAttackSlotsPlayer + 1) * (spacing));
        PanelHeight = 2 * spacing + cellsize;
        PlayerPanelTransform.sizeDelta = new Vector2(PanelWidth, PanelHeight);

        int i = NumberOfAttackSlotsPlayer;

        //Instantiting slots parented to the panel
        while (i > 0)
        {
            GameObject slot = GameObject.Instantiate(Slot, transform.position, transform.rotation) as GameObject;
            //HACK: Set Parent method???
            slot.transform.parent = PlayerInputBar.transform;
            slot.transform.localScale = IconScale;
            i -= 1;
        }
    }  

    public void SpawnEnemySlots()
    {
        //Checking grid size and adjusting panel size to it
        float cellsize = EnemyGrid.cellSize.x;
        float spacing = EnemyGrid.spacing.x;
        if (spacing == 0)
            spacing = 1;

        PanelWidth = ((NumberOfAttackSlotsEnemy * cellsize) + (NumberOfAttackSlotsEnemy + 1) * (spacing));
        PanelHeight = 2 * spacing + cellsize;
        EnemyPanelTransform.sizeDelta = new Vector2(PanelWidth, PanelHeight);

        int i = NumberOfAttackSlotsEnemy;

        //Instantiting slots parented to the panel
        while (i > 0)
        {
            GameObject slot = GameObject.Instantiate(Slot, transform.position, transform.rotation) as GameObject;
            //HACK: Set Parent method???
            slot.transform.parent = EnemyInputBar.transform;
            slot.transform.localScale = IconScale;
            i -= 1;
        }
    }    

}


