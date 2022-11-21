using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;

namespace UnityTemplateProjects.Player
{
    public class SoldierObject : MonoBehaviour
    {
        private SoldierManager _soldierManager;
        [SerializeField] private SoldierObject _nearestSoldier;
        
        [Header("For Defender")]
        public SoldierObject targetSoldier;
        
        public Animator soldierAnimator;
        public Renderer renderer;
        public Color currentColor;
        public Color inActiveColor = Color.grey;
        
        public float yPos = 0.46f;
        public SoldierState _state;
        private GameObject _gate;
        private BallObject _ball;
        public Transform ballPosition;
        public Vector3 currentPosition;

        public bool isStandby;
        public bool isActive;
        public bool isHoldBall = false;
        public float normalAttackerSpeed = 1.5f;
        public float normalDefenderSpeed = 1f;
        public float carryingSpeed = 0.75f;
        private float _returnSpeed = 2f;

        public float spawnTime = 0.5f;
        private float _reactiveDefenderTime = 4f;

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
        void SoldierReturn()
        {
            // if soldier is not active, return to the spawn point
            if(isActive)return;
            switch (_state)
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
            
        }
        void DefenderReturn()
        {
            if(isStandby)return;
            transform.position = Vector3.MoveTowards(this.transform.position, currentPosition, _returnSpeed * Time.deltaTime);
            LookBack(currentPosition);
            StartCoroutine(ReactiveSoldier(_reactiveDefenderTime));
        }

        IEnumerator ReactiveSoldier(float delay)
        {
            yield return new WaitForSeconds(delay);
            isStandby = true;
            isActive = true;
            if (_state == SoldierState.Defender)
            {
                soldierAnimator.SetBool("Idle", false);
                soldierAnimator.SetBool("Look", true);
            }
        }
        void SoldierMovement()
        {
            if(!isActive)return;
            _nearestSoldier = NearestSoldier(_soldierManager.soldierObjects);
            switch (_state)
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
            isStandby = false;
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
        void DefenderMovement()
        {
            if(targetSoldier!=null)
            {
                isStandby = false;
                Vector3 targetPos = new Vector3(targetSoldier.transform.position.x, yPos, targetSoldier.transform.position.z);
                LookAtTarget(targetSoldier.gameObject);
                this.transform.position = Vector3.MoveTowards(transform.position, targetPos, normalDefenderSpeed * Time.deltaTime);
                MovementAnimation(normalDefenderSpeed);
            }
            else
            {
                isStandby = true;
            }
            if (isStandby)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                soldierAnimator.SetBool("Look",true);
                soldierAnimator.SetBool("Idle", false);
            }
        }

        void MovementAnimation(float speed)
        {
            soldierAnimator.SetBool("Run",true);
            soldierAnimator.SetBool("Idle", false);
            soldierAnimator.SetBool("Look", false);
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

        public void Initialize(SoldierManager manager,Vector3 Position, Material mat, SoldierState state)
        {
            this.gameObject.layer = LayerMask.NameToLayer(state+"Mask");
            gameObject.tag = state.ToString();
            _soldierManager = manager;
            this.gameObject.transform.position = new Vector3(Position.x, yPos, Position.z);
            currentPosition = new Vector3(Position.x, yPos, Position.z);
            currentColor = mat.color;
            renderer.material = mat;
            isActive = false;
            isStandby = true;
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
            TriggerSoldier(col);
        }

        void TriggerSoldier(Collider col)
        {
            if(_state == SoldierState.Attacker)
            {
                if(col.gameObject.CompareTag("Ball"))
                {
                    isHoldBall = true;
                }
                if (col.gameObject.CompareTag("Fence"))
                {
                    isActive = false;
                    soldierAnimator.SetBool("Die", true);
                    _soldierManager.soldierObjects.Remove(this);
                    float delay = AnimatorDuration("Dying Backwards");
                    Destroy(this.gameObject, delay-2);
                }
            }
            else if(_state == SoldierState.Defender)
            {
                if(col.gameObject.CompareTag("Attacker"))
                {
                    targetSoldier = null;
                    soldierAnimator.SetBool("Slide", true);
                    float delay = AnimatorDuration("Running Slide");
                    StartCoroutine(DefenderStandby(delay));
                }
            }
        }
        IEnumerator DefenderStandby(float delay)
        {
            yield return new WaitForSeconds(delay);
            isStandby = true;
            soldierAnimator.SetBool("Idle", true);
            soldierAnimator.SetBool("Slide", false);
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