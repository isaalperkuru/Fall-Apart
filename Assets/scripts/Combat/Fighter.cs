using Game.Utils;
using RPG.Attributes;
using RPG.Core;
using RPG.Movement;
using Game.Saving;
using RPG.Stats;
using System.Collections.Generic;
using UnityEngine;
using Game.Inventories;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        Health target;
        Equipment equipment;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;

        WeaponConfig currentWeaponConfig;
        float timeSinceLastAttack = 0;
        LazyValue<Weapon> currentWeapon;

        private void Awake()
        {
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
            equipment = GetComponent<Equipment>();
            if (equipment)
            {
                equipment.equipmentUpdated += UpdateWeapon;
            }
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
            if (!GetIsInRange(target.transform))
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
            return AttachWeapon(defaultWeapon);
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
            if (!GetComponent<Mover>().CanMoveTo(combatTarget.transform.position) &&
                !GetIsInRange(combatTarget.transform)) 
            { 
                return false;
            }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }
        private bool GetIsInRange(Transform targetTransform)
        {
            //is distance with player and enemy is less than 2f
            return Vector3.Distance(transform.position, targetTransform.position) 
                < currentWeaponConfig.GetRange();
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

            if(currentWeapon.value != null)
            {
                currentWeapon.value.OnHit();
            }
            if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
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
        public void EquipWeapon(WeaponConfig weapon)
        {
            if (GetComponent<Health>().IsDead()) return;
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private void UpdateWeapon()
        {
            var weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            if (weapon == null) EquipWeapon(defaultWeapon);
            else EquipWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public Health GetTarget()
        {
            return target;
        }

        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }
    }
}

