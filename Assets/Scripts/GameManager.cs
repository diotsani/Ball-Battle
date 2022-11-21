using UnityEngine;

namespace DefaultNamespace
{
    public class GameManager : MonoBehaviour
    {
        public SoldierManager playerManager;
        public SoldierManager enemyManager;
        
        void ChangeSoldierState()
        {
            if (playerManager.soldierState == SoldierState.Attacker)
            {
                playerManager.soldierState = SoldierState.Defender;
            }
            else
            {
                playerManager.soldierState = SoldierState.Attacker;
            }
            
            if(enemyManager.soldierState == SoldierState.Attacker)
            {
                enemyManager.soldierState = SoldierState.Defender;
            }
            else
            {
                enemyManager.soldierState = SoldierState.Attacker;
            }
        }
    }
}