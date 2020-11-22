using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;


namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health health;

        private void Start()
        {
            health = GetComponent<Health>();
        }
        // Update is called once per frame
        void Update()
        {
            if (health.IsDead()) return;
            if(InteractWithCombat()) return;
            if(InteractWithMovement()) return;
            
        }

        private bool InteractWithMovement()
        {
            //left click laser info
            RaycastHit hit;
            //if we hit something. 
            //out allows us to return information about the location that a raycast has hit
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (hasHit)
            {
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(hit.point);
                    //go to position ray (laser) hit
                }
                return true;

            }
            return false;
        }
        private bool InteractWithCombat()
        {
            //We take every ray hits and finding object we can attack
            //so the other objects cant block the ray hits to enemy
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if (target == null) continue;

                
                if (!GetComponent<Fighter>().CanAttack(target.gameObject))
                {
                    continue;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    GetComponent<Fighter>().Attack(target.gameObject); 
                }
                return true;
            }
            return false;
        }
        private static Ray GetMouseRay()
        {
            //left click laser from cam
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}

