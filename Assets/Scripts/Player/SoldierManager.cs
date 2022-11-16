using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTemplateProjects;
using UnityTemplateProjects.Player;

public enum SoldierType
{
    Player,
    Enemy
}
public enum SoldierState
{
    Attacker,
    Defender
}
public class SoldierManager : MonoBehaviour
{
    public SoldierType soldierType;
    public SoldierState soldierState;

    public float energyCost;
    private float attackEnergy = 2f;
    private float defendEnergy = 3f;

    public float currentEnergy;
    private float _maxEnergy = 6;

    private Material soldierMaterial;
    private SoldierObject soldierPrefab;
    public GameObject gateObject;
    public BallObject ballObject;
    
    private List<SoldierObject> soldierObjects = new List<SoldierObject>();

    void Start()
    {
        soldierMaterial = Resources.Load<Material>("Materials/"+soldierType+"Material");
        soldierPrefab = Resources.Load<SoldierObject>("Prefabs/Soldier");
    }
    
    void Update()
    {
        SoldierEnergy();
        InitSoldier();
    }

    private void SoldierEnergy()
    {
        if(soldierState == SoldierState.Attacker)
        {
            energyCost = attackEnergy;
        }
        else if(soldierState == SoldierState.Defender)
        {
            energyCost = defendEnergy;
        }
        
        currentEnergy += 0.5f * Time.deltaTime;
        if(currentEnergy >= _maxEnergy)
            currentEnergy = _maxEnergy;
    }

    private void InitSoldier()
    {
#if UNITY_EDITOR
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == soldierType+"Ground")
                {
                    SpawnSoldier(hit.point);
                }
            }
        }
#endif
    }

    private void SpawnSoldier(Vector3 Transform)
    {
        if(currentEnergy < energyCost) return;
        currentEnergy -= energyCost;
        SoldierObject obj = Instantiate(soldierPrefab, transform);
        soldierObjects.Add(obj);
        obj.Initialize(Transform,soldierMaterial);
        obj.State = soldierState;
        obj.Gate = gateObject;
        obj.Ball = ballObject;
        obj.name = soldierType.ToString();
    }
    public void BallHolder()
    {
        foreach (var soldier in soldierObjects)
        {
            if(soldier.State == SoldierState.Attacker)
            {
                soldier.isHoldBall = true;
            }
        }
    }
}
