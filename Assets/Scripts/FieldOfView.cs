using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTemplateProjects.Player;

namespace BallBattle
{
    public class FieldOfView : MonoBehaviour
    {
        [Header("Dependencies")]
        public SoldierObject soldierObject;

        [Header("Settings")]
        public float radius;
        [Range(0, 360)] public float angle;

        [Header("Layer Mask")]
        private LayerMask _targetMask;
        private LayerMask _obstacleMask;

        private void Start()
        {
            soldierObject = GetComponent<SoldierObject>();
            StartCoroutine(FOVRoutine());
            _targetMask = LayerMask.GetMask("AttackerMask");
            _obstacleMask = LayerMask.GetMask("ObstacleMask");
        }
        private IEnumerator FOVRoutine()
        {
            WaitForSeconds wait = new WaitForSeconds(0.2f);

            while (true)
            {
                yield return wait;
                CheckFOV();
            }
        }

        private void CheckFOV()
        {
            if(soldierObject.state == SoldierState.Attacker)return;
            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, radius, _targetMask);
            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                Transform target = targetsInViewRadius[i].transform;
                SoldierObject targetSoldierObject = target.GetComponent<SoldierObject>();
                if (targetSoldierObject.soldierStatus == SoldierStatus.HoldBall)
                {
                    if(soldierObject.soldierStatus != SoldierStatus.Standby)return;
                    soldierObject.targetSoldier = targetSoldierObject;
                    soldierObject.soldierStatus = SoldierStatus.Active;
                }

                Vector3 dirToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) < angle / 2)
                {
                    float dstToTarget = Vector3.Distance(transform.position, target.position);
                    if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, _obstacleMask))
                    {
                        
                    }
                }
            }
        }
    }
}
