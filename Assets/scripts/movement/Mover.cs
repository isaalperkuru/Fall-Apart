using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour
    {
        //[SerializeField] Transform target;
        void Update()
        {

            UpdateAnimator();

        }
        private void UpdateAnimator()
        {
            //we taking velocity of navmesh
            Vector3 velocity = GetComponent<NavMeshAgent>().velocity;
            //we make velocity increase by the time while moving
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            //z velocity
            float speed = localVelocity.z;
            //min velo to max velo informed
            GetComponent<Animator>().SetFloat("ForwardSpeed", speed);
        }
        private void MoveToCursor()
        {
            //left click laser from cam
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //left click laser info
            RaycastHit hit;
            //if we hit something. 
            //out allows us to return information about the location that a raycast has hit
            bool hasHit = Physics.Raycast(ray, out hit);
            if (hasHit)
            {
                //go to position ray (laser) hit
                MoveTo(hit.point);
            }
        }

        public void MoveTo(Vector3 destination)
        {
            GetComponent<NavMeshAgent>().destination = destination;
        }

    }

}
