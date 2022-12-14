using System;
using System.Collections;
using UnityEngine;
using UnityTemplateProjects.Player;
using Random = UnityEngine.Random;

namespace UnityTemplateProjects
{
    public enum BallStatus
    {
        None,
        Hold,
        Pass
    }
    public class BallObject : MonoBehaviour
    {
        public SoldierManager playerManager;
        public SoldierManager enemyManager;
        
        public SoldierObject currentSoldier;
        public SoldierObject _passTarget;
        
        public Transform defaultParent;
        public float ballSpeed = 2f;
        public BallStatus ballStatus;
        public bool IsHold;

        [Header("Area")]
        public float minY = 0.2f;
        public float areaMinX = -2f;
        public float areaMaxX = 2f;
        
        public float playerMinZ = -0.5f;
        public float playerMaxZ = -6f;
        
        public float enemyMinZ = 0.5f;
        public float enemyMaxZ = 6f;

        private void Start()
        {
            ballStatus = BallStatus.None;
            BallPosition();
        }

        private void Update()
        {
            switch (ballStatus)
            {
                case BallStatus.Hold:
                    if(currentSoldier.soldierStatus != SoldierStatus.Active)return;
                    _passTarget = currentSoldier.nearestSoldier;
                    transform.parent = currentSoldier.ballPosition.transform;
                    transform.localPosition = new Vector3(0,0,0);
                    currentSoldier.soldierStatus = SoldierStatus.HoldBall;
                    break;
                case BallStatus.Pass:
                    if (_passTarget == null)
                    {
                        // Lose
                        Debug.Log("Lose");
                        return;
                    }
                    _passTarget = currentSoldier.nearestSoldier;
                    var speed = ballSpeed * Time.deltaTime;
                    transform.position = Vector3.MoveTowards(transform.position, _passTarget.transform.position,speed);
                    if(Vector3.Distance(transform.position, _passTarget.transform.position) < 0.1f)
                    {
                        currentSoldier = _passTarget;
                        transform.parent = _passTarget.ballPosition.transform;
                        
                        ballStatus = BallStatus.Hold;
                    }
                    break;
            }
        }
        IEnumerator WaitPass(float delay)
        {
            yield return new WaitForSeconds(delay);
            transform.parent = _passTarget.ballPosition.transform;
            ballStatus = BallStatus.Hold;
            
        }
        public void BallPosition()
        {
            ballStatus = BallStatus.None;
            if(playerManager.soldierState == SoldierState.Attacker)
            {
                transform.position = new Vector3(Random.Range(areaMinX, areaMaxX), minY, Random.Range(playerMinZ, playerMaxZ));
            }
            else if(enemyManager.soldierState == SoldierState.Attacker)
            {
                transform.position = new Vector3(Random.Range(areaMinX, areaMaxX), minY, Random.Range(enemyMinZ, enemyMaxZ));
            }
            // RandomPosition(new Vector3(Random.Range(areaMinX, areaMaxX), 0, Random.Range(playerMinZ, playerMaxZ)),playerMinZ,playerMaxZ);
            // RandomPosition(new Vector3(Random.Range(areaMinX, areaMaxX), 0, Random.Range(enemyMinZ, enemyMaxZ)),enemyMinZ,enemyMaxZ);
        }
        private void OnTriggerEnter(Collider col)
        {
            if(col.gameObject.CompareTag("Attacker"))
            {
                //this.transform.position = collision.gameObject.GetComponent<SoldierObject>().ballPosition.position;
                var soldier = col.gameObject.GetComponent<SoldierObject>();
                if(currentSoldier == null)
                {
                    ballStatus = BallStatus.Hold;
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
            if(col.gameObject.CompareTag("Defender"))
            {
                var soldier = col.gameObject.GetComponent<SoldierObject>();
                if(soldier.soldierStatus != SoldierStatus.Active)return;
                ballStatus = BallStatus.Pass;
            }
        }
    }
}