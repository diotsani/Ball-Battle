using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTemplateProjects.Player
{
    public class SoldierObject : MonoBehaviour
    {
        private SoldierManager _soldierManager;
        [SerializeField] private SoldierObject _nearestSoldier;
        public Animator soldierAnimator;
        public Renderer renderer;
        public Color currentColor;
        public Color inActiveColor = Color.grey;
        
        public float yPos = 0.46f;
        private SoldierState _state;
        private GameObject _gate;
        private BallObject _ball;
        public Transform ballPosition;
        public Vector3 currentPosition;

        public bool isActive;
        public bool isHoldBall = false;
        public float normalAttackerSpeed = 1.5f;
        public float normalDefenderSpeed = 1f;
        public float carryingSpeed = 0.75f;

        public float spawnTime = 0.5f;

        private void Start()
        {
            //material = GetComponent<Renderer>().material;
            this.transform.LookAt(_gate.transform);
        }

        private void Update()
        {
            SoldierMovement();
        }
        void SoldierMovement()
        {
            if(!isActive)return;
            _nearestSoldier = NearestSoldier(_soldierManager.soldierObjects);
            if(!isHoldBall && _state == SoldierState.Attacker && !_ball.IsHold)
            {
                Vector3 targetPos = new Vector3(_ball.transform.position.x, yPos, _ball.transform.position.z);
                LookAtTarget(_ball.gameObject);
                this.transform.position = Vector3.MoveTowards(transform.position, targetPos, normalAttackerSpeed * Time.deltaTime);
                MovementAnimation(normalAttackerSpeed);
                Debug.Log("To Ball");
            }
            else if(isHoldBall)
            {
                Vector3 targetPos = new Vector3(_gate.transform.position.x, yPos, _gate.transform.position.z);
                LookAtTarget(_gate);
                this.transform.position = Vector3.MoveTowards(transform.position, targetPos, carryingSpeed * Time.deltaTime);
                MovementAnimation(carryingSpeed);
                Debug.Log("To Gate");
            }
            if(_ball.IsHold && !isHoldBall)
            {
                Vector3 targetPos = new Vector3(transform.position.x, yPos, _gate.transform.position.z);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                this.transform.position = Vector3.MoveTowards(transform.position,targetPos , normalAttackerSpeed * Time.deltaTime);
                MovementAnimation(normalAttackerSpeed);
                Debug.Log("To Fence");
            }
        }

        void MovementAnimation(float speed)
        {
            soldierAnimator.SetBool("Run",true);
            soldierAnimator.SetBool("Idle", false);
        }
        private SoldierObject NearestSoldier(List<SoldierObject> listSoldiers)
        {
            SoldierObject nerestSoldier = null;
            float minDist = Mathf.Infinity;
            Vector3 currentPos = transform.position;
            foreach (SoldierObject soldier in listSoldiers)
            {
                float dist = Vector3.Distance(soldier.transform.position, currentPos);
                if (dist < minDist && soldier != this)
                {
                    nerestSoldier = soldier;
                    minDist = dist;
                }
            }
            return nerestSoldier;
        }
        private void LookAtTarget(GameObject target)
        {
            Vector3 relativePos = target.transform.position - this.transform.position;
            relativePos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            this.transform.rotation = rotation;
        }

        public void Initialize(SoldierManager manager,Vector3 Position, Material mat)
        {
            _soldierManager = manager;
            this.gameObject.transform.position = new Vector3(Position.x, yPos, Position.z);
            currentPosition = new Vector3(Position.x, yPos, Position.z);
            currentColor = mat.color;
            renderer.material = mat;
            isActive = false;
            StartCoroutine(SoldierSpawn());
        }

        IEnumerator SoldierSpawn()
        {
            renderer.material.color = inActiveColor;
            yield return new WaitForSeconds(spawnTime);
            isActive = true;
            renderer.material.color = currentColor;
        }
        public void OnTriggerEnter(Collider col)
        {
            if(col.gameObject.CompareTag("Ball"))
            {
                isHoldBall = true;
            }
        }

        public SoldierState State
        {
            get => _state;
            set => _state = value;
        }
        public GameObject Gate
        {
            get => _gate;
            set => _gate = value;
        }
        public BallObject Ball
        {
            get => _ball;
            set => _ball = value;
        }
    }
}