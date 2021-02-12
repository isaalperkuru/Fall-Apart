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
        Health target;

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
            if (target.IsDead()) return;
            //if we in range combat target in this case 2f (weaponRange)
            if (!GetIsInRange())
            {
                //move to target
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
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
            //looking face to enemy while attacking
            transform.LookAt(target.transform);
            if(timeSinceLastAttack > timeBetweenAttacks)
            {
                TriggerAttack();
                timeSinceLastAttack = 0;
            }
        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            //this will trigger the Hit() event
            GetComponent<Animator>().SetTrigger("attack");
        }

        //if object attackable or attackable object death
        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }
        private bool GetIsInRange()
        {
            //is distance with player and enemy is less than 2f
            return Vector3.Distance(transform.position, target.transform.position) < weaponRange;
        }

        public void Attack(GameObject combatTarget)
        {
            //send report to schedular we change the action
            GetComponent<ActionSchedular>().StartAction(this);
            //changing our target to our enemy's position
            target = combatTarget.GetComponent<Health>();
        }
        public void Cancel()
        {
            StopAttack();
            //Cancelling combat
            target = null;
            GetComponent<Mover>().Cancel();
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        //Animation event
        void Hit()
        {
            if (target == null) return;
            target.TakeDamage(weaponDamage);
        }
    }
}

