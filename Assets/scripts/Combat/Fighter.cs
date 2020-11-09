using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using System;
using RPG.Core;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        Transform target;

        [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float weaponDamage = 5f;
        float timeSinceLastAttack = 0;


        // Update is called once per frame
        void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            //if we have target
            if (target == null) return;
            //if we in range combat target in this case 2f (weaponRange)
            if (!GetIsInRange())
            {
                //move to target
                GetComponent<Mover>().MoveTo(target.position);
            }
            else
            {
                //Stop movement
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }

        }

        private void AttackBehaviour()
        {
            if(timeSinceLastAttack > timeBetweenAttacks)
            {
                //this will trigger the Hit() event
                GetComponent<Animator>().SetTrigger("attack");
                timeSinceLastAttack = 0;
            }     
        }

        private bool GetIsInRange()
        {
            //is distance with player and enemy is less than 2f
            return Vector3.Distance(transform.position, target.position) < weaponRange;
        }

        public void Attack(CombatTarget combatTarget)
        {
            //send report to schedular we change the action
            GetComponent<ActionSchedular>().StartAction(this);
            //changing our target to our enemy's position
            target = combatTarget.transform;
        }
        public void Cancel()
        {
            //Cancelling combat
            target = null;
        }
        //Animation event
        void Hit()
        {
            Health healthComponent = target.GetComponent<Health>();
            healthComponent.TakeDamage(weaponDamage);
        }
    }
}

