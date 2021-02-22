using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using System;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using Game.Utils;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        Health target;

        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Weapon defaultWeapon = null;

        LazyValue<Weapon> currentWeapon;
        float timeSinceLastAttack = 0;

        private void Awake()
        {
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        private void Start()
        {
            currentWeapon.ForceInit();
        }

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
        private Weapon SetupDefaultWeapon()
        {
            AttachWeapon(defaultWeapon);
            return defaultWeapon;
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
            return Vector3.Distance(transform.position, target.transform.position) 
                < currentWeapon.value.GetRange();
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

            float damage = GetComponent<BaseStats>().GetStat(Stats.Stats.Damage);
            if (currentWeapon.value.HasProjectile())
            {
                currentWeapon.value.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            else
            {
                target.TakeDamage(gameObject, damage);
            }
        }

        void Shoot()
        {
            Hit();
        }
        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon.value = weapon;
            AttachWeapon(weapon);
        }

        private void AttachWeapon(Weapon weapon)
        {
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public Health GetTarget()
        {
            return target;
        }
        public IEnumerable<float> GetAdditiveModifiers(Stats.Stats stat)
        {
            if(stat == Stats.Stats.Damage)
            {
                yield return currentWeapon.value.GetDamage();
            }
        }
        public IEnumerable<float> GetPercentageModifiers(Stats.Stats stat)
        {
            if (stat == Stats.Stats.Damage)
            {
                yield return currentWeapon.value.GetPercentageBonus();
            }
        }

        public object CaptureState()
        {
            return currentWeapon.value.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            Weapon weapon = Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
    }
}

