using System;
using UnityEngine;

namespace UnityTemplateProjects.Player
{
    public class SoldierObject : MonoBehaviour
    {
        //private Material material;
        public float yPos = 0.46f;
        private SoldierState _state;
        private GameObject _gate;
        private BallObject _ball;
        public Transform ballPosition;
        public Vector3 currentPosition;
        
        public bool isHoldBall = false;
        public float normalAttackerSpeed = 1.5f;
        public float normalDefenderSpeed = 1f;
        public float carryingSpeed = 0.75f;

        private void Start()
        {
            //material = GetComponent<Renderer>().material;
            this.transform.LookAt(_gate.transform);
        }

        private void Update()
        {
            if(!isHoldBall && _state == SoldierState.Attacker && !_ball.IsHold)
            {
                Vector3 targetPos = new Vector3(_ball.transform.position.x, yPos, _ball.transform.position.z);
                LookAtTarget(_ball.gameObject);
                this.transform.position = Vector3.MoveTowards(transform.position, targetPos, normalAttackerSpeed * Time.deltaTime);
                Debug.Log("To Ball");
            }
            else if(isHoldBall)
            {
                Vector3 targetPos = new Vector3(_gate.transform.position.x, yPos, _gate.transform.position.z);
                LookAtTarget(_gate);
                this.transform.position = Vector3.MoveTowards(transform.position, targetPos, carryingSpeed * Time.deltaTime);
                Debug.Log("To Gate");
            }
            if(_ball.IsHold && !isHoldBall)
            {
                Vector3 targetPos = new Vector3(transform.position.x, yPos, _gate.transform.position.z);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                this.transform.position = Vector3.MoveTowards(transform.position,targetPos , normalAttackerSpeed * Time.deltaTime);
                Debug.Log("To Fence");
            }
            
        }
        private void LookAtTarget(GameObject target)
        {
            Vector3 relativePos = target.transform.position - this.transform.position;
            relativePos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            this.transform.rotation = rotation;
        }

        public void Initialize(Vector3 Position, Material mat)
        {
            this.gameObject.transform.position = new Vector3(Position.x, yPos, Position.z);
            currentPosition = new Vector3(Position.x, yPos, Position.z);
            this.GetComponent<Renderer>().material.color = mat.color;
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