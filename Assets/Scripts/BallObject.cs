using System;
using UnityEngine;
using UnityTemplateProjects.Player;

namespace UnityTemplateProjects
{
    public class BallObject : MonoBehaviour
    {
        public SoldierManager soldierManager;
        public bool IsHold;
        private void OnTriggerEnter(Collider col)
        {
            if(col.gameObject.CompareTag("Player"))
            {
                //this.transform.position = collision.gameObject.GetComponent<SoldierObject>().ballPosition.position;
                IsHold = true;
                //soldierManager.BallHolder();
            }
        }
    }
}