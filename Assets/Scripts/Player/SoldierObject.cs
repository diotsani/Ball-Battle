using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace UnityTemplateProjects.Player
{
    public enum SoldierStatus
    {
        Standby,
        Active,
        Inactive,
        HoldBall,
        Wait
    }
    public class SoldierObject : MonoBehaviour
    {
        private SoldierManager _soldierManager;
        public SoldierObject nearestSoldier;
        
        [Header("For Defender")]
        public SoldierObject targetSoldier;
        
        [Header("Animation")]
        public Animator soldierAnimator;
        private string _idle = "Idle";
        private string _run = "Run";
        private string _die = "Die";
        private string _look = "Look";
        private string _slide = "Slide";
        private string _fall = "Fall";
        
        
        public Renderer renderer;
        public Color currentColor;
        public Color inActiveColor = Color.grey;
        
        public float yPos = 0.46f;
        public float yRotation;
        private GameObject _gate;
        private BallObject _ball;
        public Transform ballPosition;
        public Vector3 currentPosition;

        [Header("Soldier Status")]
        public SoldierType soldierType;
        public SoldierState state;
        public SoldierStatus soldierStatus;
        
        // public bool isStandby;
        // public bool isActive;
        // public bool isHoldBall = false;
        
        public float normalAttackerSpeed = 1.5f;
        public float normalDefenderSpeed = 1f;
        public float carryingSpeed = 0.75f;
        private float _returnSpeed = 2f;

        public float spawnTime = 0.5f;
        private float _reactiveDefenderTime = 4f;
        private float _reactiveAttackerTime = 2.5f;

        private void Start()
        {
            //material = GetComponent<Renderer>().material;
            this.transform.LookAt(_gate.transform);
        }

        private void Update()
        {
            SoldierMovement();
            SoldierReturn();
        }
        public void Initialize(SoldierManager manager,Vector3 Position, Material mat, SoldierState state)
        {
            this.gameObject.layer = LayerMask.NameToLayer(state+"Mask");
            gameObject.tag = state.ToString();
            _soldierManager = manager;
            soldierType = manager.soldierType;
            yRotation = manager._yRotation;
            
            this.gameObject.transform.position = new Vector3(Position.x, yPos, Position.z);
            currentPosition = new Vector3(Position.x, yPos, Position.z);
            currentColor = mat.color;
            renderer.material = mat;
            // isActive = false;
            // isStandby = true;
            soldierStatus = SoldierStatus.Standby;
            StartCoroutine(SoldierSpawn());
        }
        IEnumerator SoldierSpawn()
        {
            renderer.material.color = inActiveColor;
            yield return new WaitForSeconds(spawnTime);
            // isActive = true;
            renderer.material.color = currentColor;
            switch (state)
            {
                case SoldierState.Attacker:
                    soldierStatus = SoldierStatus.Active;
                    break;
                case SoldierState.Defender:
                    soldierStatus = SoldierStatus.Standby;
                    break;
            }
        }
        void SoldierReturn()
        {
            // if soldier is not active, return to the spawn point
            if(soldierStatus != SoldierStatus.Inactive)return;
            switch (state)
            {
                case SoldierState.Attacker:
                    AttackerReturn();
                    break;
                case SoldierState.Defender:
                    DefenderReturn();
                    break;
            }
        }

        void AttackerReturn()
        {
            renderer.material.color = inActiveColor;
            transform.rotation = Quaternion.Euler(0, yRotation, 0);
            
            soldierAnimator.SetBool(_idle, true);
            
            StartCoroutine(ReactiveSoldier(_reactiveAttackerTime));
        }
        void DefenderReturn()
        {
            // if(isStandby)return;
            Debug.Log("Defender Return");
            renderer.material.color = inActiveColor;
            transform.position = Vector3.MoveTowards(this.transform.position, currentPosition, _returnSpeed * Time.deltaTime);
            LookBack(currentPosition);
            MovementAnimation(_returnSpeed);
            if(Vector3.Distance(transform.position, currentPosition) < 0.1f)
            {
                soldierAnimator.SetBool(_idle, true);
                soldierAnimator.SetBool(_run, false); 
                
                transform.rotation = Quaternion.Euler(0, yRotation, 0);
            }
            StartCoroutine(ReactiveSoldier(_reactiveDefenderTime));
        }

        IEnumerator ReactiveSoldier(float delay)
        {
            yield return new WaitForSeconds(delay);
            // isStandby = true;
            // isActive = true;
            renderer.material.color = currentColor;
            if (state == SoldierState.Attacker)
            {
                soldierStatus = SoldierStatus.Active;
            }
            else if(state == SoldierState.Defender)
            {
                soldierStatus = SoldierStatus.Standby;
            }
        }
        void SoldierMovement()
        {
            // if(!isActive)return;
            nearestSoldier = NearestSoldier(_soldierManager.soldierObjects);
            switch (state)
            {
                case SoldierState.Attacker:
                    AttackerMovement();
                    break;
                case SoldierState.Defender:
                    DefenderMovement();
                    break;
            }
        }

        void AttackerMovement()
        {
            switch (soldierStatus)
            {
                case SoldierStatus.Active:
                    if (_ball.IsHold)
                    {
                        Vector3 targetFence = new Vector3(transform.position.x, yPos, _gate.transform.position.z);
                        transform.rotation = Quaternion.Euler(0, yRotation, 0);
                        this.transform.position = Vector3.MoveTowards(transform.position,targetFence , normalAttackerSpeed * Time.deltaTime);
                        MovementAnimation(normalAttackerSpeed);
                        Debug.Log("To Fence");
                    }
                    else
                    {
                        Vector3 targetBall = new Vector3(_ball.transform.position.x, yPos, _ball.transform.position.z);
                        LookAtTarget(_ball.gameObject);
                        this.transform.position = Vector3.MoveTowards(transform.position, targetBall, normalAttackerSpeed * Time.deltaTime);
                        MovementAnimation(normalAttackerSpeed);
                        Debug.Log("To Ball");
                    }
                    break;
                
                case SoldierStatus.HoldBall:
                    Vector3 targetGate = new Vector3(_gate.transform.position.x, yPos, _gate.transform.position.z);
                    LookAtTarget(_gate);
                    this.transform.position = Vector3.MoveTowards(transform.position, targetGate, carryingSpeed * Time.deltaTime);
                    MovementAnimation(carryingSpeed);
                    Debug.Log("To Gate");
                    break;
            }
        }
        void DefenderMovement()
        {
            switch (soldierStatus)
            {
                case SoldierStatus.Standby:
                    transform.rotation = Quaternion.Euler(0, yRotation, 0);
                    
                    soldierAnimator.SetBool(_idle, false);
                    soldierAnimator.SetBool(_look,true);
                    break;
                case SoldierStatus.Active:
                    Vector3 targetPos = new Vector3(targetSoldier.transform.position.x, yPos, targetSoldier.transform.position.z);
                    LookAtTarget(targetSoldier.gameObject);
                    this.transform.position = Vector3.MoveTowards(transform.position, targetPos, normalDefenderSpeed * Time.deltaTime);
                    MovementAnimation(normalDefenderSpeed);
                    if (targetSoldier == null)
                    {
                        soldierStatus = SoldierStatus.Standby;
                    }
                    break;
            }
        }

        void MovementAnimation(float speed)
        {
            soldierAnimator.SetBool(_idle, false);
            soldierAnimator.SetBool(_look, false);
            soldierAnimator.SetBool(_fall, false);
            soldierAnimator.SetBool(_run,true);
            soldierAnimator.speed = speed;
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
        private void LookBack(Vector3 target)
        {
            Vector3 relativePos = target - this.transform.position;
            relativePos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            this.transform.rotation = rotation;
        }
        public void OnTriggerEnter(Collider col)
        {
            TriggerSoldier(col);
        }

        void TriggerSoldier(Collider col)
        {
            if(state == SoldierState.Attacker)
            {
                if(col.gameObject.CompareTag("Ball"))
                {
                    // isHoldBall = true;
                    //if(_ball.IsHold)return;
                    //_ball.gameObject.transform.parent = ballPosition;
                    if(soldierStatus != SoldierStatus.Active)return;
                    //soldierStatus = SoldierStatus.HoldBall;
                }
                if (col.gameObject.CompareTag("Fence"))
                {
                    // isActive = false;
                    soldierStatus = SoldierStatus.Inactive;
                    
                    soldierAnimator.SetBool(_die, true);
                    
                    _soldierManager.soldierObjects.Remove(this);
                    float delay = AnimatorDuration("Dying Backwards");
                    Destroy(this.gameObject, delay-2);
                }
                if(col.gameObject.CompareTag("Gate"))
                {
                    // Goal
                }
                if(col.gameObject.CompareTag("Defender"))
                {
                    if(soldierStatus != SoldierStatus.HoldBall)return;
                    soldierStatus = SoldierStatus.Wait; // bug
                    soldierAnimator.SetBool(_run, false);
                    soldierAnimator.SetBool(_fall, true);
                    StartCoroutine(AttackerInactive(0.45f));

                }
            }
            else if(state == SoldierState.Defender)
            {
                if(col.gameObject.CompareTag("Attacker"))
                {
                    var attacker = col.gameObject.GetComponent<SoldierObject>();
                    Debug.Log("Attacker status is "+attacker.soldierStatus);
                    if(attacker.soldierStatus == SoldierStatus.HoldBall)
                    {
                        Debug.Log("Sliding Attacker");
                        soldierStatus = SoldierStatus.Wait; // bug with attacker>compare tag defender
                    
                        soldierAnimator.SetBool(_run, false);
                        soldierAnimator.SetBool(_slide, true);
                    
                        StartCoroutine(DefenderInactive(0.45f));
                    }
                    
                }
            }
        }

        IEnumerator AttackerInactive(float delay)
        {
            yield return new WaitForSeconds(delay);
            soldierAnimator.SetBool(_fall, false);
            soldierAnimator.SetBool(_idle, true);
            yield return new WaitForSeconds(delay);
            soldierStatus = SoldierStatus.Inactive;
        }
        IEnumerator DefenderInactive(float delay)
        {
            yield return new WaitForSeconds(delay);
            targetSoldier = null;
            soldierAnimator.SetBool(_run, false);
            soldierAnimator.SetBool(_slide, false);
            soldierAnimator.SetBool(_idle, true);
            yield return new WaitForSeconds(delay);
            soldierStatus = SoldierStatus.Inactive;
        }
        float AnimatorDuration(string name)
        {
            RuntimeAnimatorController controller = soldierAnimator.runtimeAnimatorController;
            foreach (var clip in controller.animationClips)
            {
                if(clip.name == name)
                {
                    return clip.length;
                }
            }
            return 0;
        }
        public SoldierState State
        {
            get => state;
            set => state = value;
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