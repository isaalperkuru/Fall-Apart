using RPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        NavMeshAgent navMeshAgent;
        [SerializeField] Transform target;
        [SerializeField] float maxSpeed = 5.7f;
        Health health;
        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }
        void Update()
        {
            navMeshAgent.enabled = !health.IsDead();
            UpdateAnimator();

        }
        private void UpdateAnimator()
        {
            //we taking velocity of navmesh
            Vector3 velocity = navMeshAgent.velocity;
            //we make velocity increase by the time while moving
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            //z velocity
            float speed = localVelocity.z;
            //min velo to max velo informed
            GetComponent<Animator>().SetFloat("ForwardSpeed", speed);
        }
        public void MoveTo(Vector3 destination, float speedFraction)
        {
            //move while you are in the right to move
            navMeshAgent.destination = destination;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;
        }

        public void Cancel()
        {
            //Stop you can't go there :)
            navMeshAgent.isStopped = true;
        }
        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            //report action to schedular we are moving
            GetComponent<ActionSchedular>().StartAction(this);
            //move to where we want
            MoveTo(destination, speedFraction);
        }
        
    }

}
