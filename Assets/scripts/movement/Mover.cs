using RPG.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        NavMeshAgent navMeshAgent;
        [SerializeField] Transform target;
        [SerializeField] float maxSpeed = 5.7f;
        [SerializeField] float maxNavPathLength = 40f;
        Health health;
        private void Awake()
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

        public bool CanMoveTo(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(path) > maxNavPathLength) return false;

            return true;
        }
        private float GetPathLength(NavMeshPath path)
        {
            float total = 0;
            if (path.corners.Length < 2) return total;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return total;
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
        [System.Serializable]
        struct MoverSaveData
        {
            public SerializableVector3 position;
            public SerializableVector3 rotation;
        }
        public object CaptureState()
        {
            MoverSaveData data = new MoverSaveData();
            data.position= new SerializableVector3(transform.position);
            data.rotation = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        public void RestoreState(object state)
        {
            MoverSaveData data = (MoverSaveData)state;
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = data.position.ToVector();
            transform.eulerAngles = data.rotation.ToVector();
            GetComponent<NavMeshAgent>().enabled = true;
        }
    }

}
