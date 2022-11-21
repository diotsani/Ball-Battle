using System;
using UnityEngine;
using UnityTemplateProjects.Player;

namespace UnityTemplateProjects
{
    public class BallObject : MonoBehaviour
    {
        public SoldierManager soldierManager;
        public SoldierObject currentSoldier;
        public bool IsHold;
        private void OnTriggerEnter(Collider col)
        {
            if(col.gameObject.CompareTag("Player"))
            {
                //this.transform.position = collision.gameObject.GetComponent<SoldierObject>().ballPosition.position;
                if(currentSoldier == null)
                {
                    currentSoldier = col.gameObject.GetComponent<SoldierObject>();
                    IsHold = true;
                }
                else if (!IsHold)
                {
                    currentSoldier = col.gameObject.GetComponent<SoldierObject>();
                    IsHold = true;
                }
                //soldierManager.BallHolder();
            }
        }
    }
}