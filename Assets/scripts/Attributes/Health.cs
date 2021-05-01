using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Saving;
using RPG.Stats;
using RPG.Core;
using System;
using Game.Utils;
using UnityEngine.Events;
using RPG.SceneManagement;
using RPG.Quests;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDie;

        GameObject boss;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {
        }

        LazyValue<float> healthPoints;
        bool isDead = false;

        GameObject player;
        public GameObject LoadScene;
        private void Awake()
        {
            healthPoints = new LazyValue<float>(GetInitialHealth);
            player = GameObject.FindWithTag("Player");
            boss = GameObject.FindWithTag("Boss");
        }
      
        private void GetPortal(GameObject portal)
        {

        }
        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stats.Stats.Health);
        }
        private void Start()
        {
            healthPoints.ForceInit();
            LoadScene = GameObject.Find("Saving");
        }
        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        }
        private void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;
        }

        public bool IsDead()
        {
            return isDead;
        }
        public void TakeDamage(GameObject instigator, float damage)
        {
            print(gameObject.name + "took damage: " + damage);

            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            takeDamage.Invoke(damage);
            if (healthPoints.value == 0)
            {
                onDie.Invoke();
                Die();
                AwardExperience(instigator);
                
            }
        }
        public void Heal(float healthToRestore)
        {
            healthPoints.value = Mathf.Min(healthPoints.value + healthToRestore, GetMaxHealthPoints());
        }

        public float GetHealthPoints()
        {
            return healthPoints.value;
        }
        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stats.Stats.Health);
        }

        public float GetPercentage()
        {
            return 100 * (GetFraction());
        }

        public float GetFraction()
        {
            return healthPoints.value / GetComponent<BaseStats>().GetStat(Stats.Stats.Health);
        }
        private void Die()
        {
            if (isDead) return;

            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionSchedular>().CancelCurrentAction();
            if (player.GetComponent<Health>().IsDead())
            {
                Invoke("Respawn", 3);
            }
        }

        private void Respawn()
        {
            LoadScene.GetComponent<SavingWrapper>().LoadFromLastSave();
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;
            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stats.Stats.ExperienceReward));
        }
        private void RegenerateHealth()
        {
            float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stats.Stats.Health);
            healthPoints.value = Mathf.Max(healthPoints.value, regenHealthPoints);
        }

        public object CaptureState()
        {
            return healthPoints.value;
        }

        public void RestoreState(object state)
        {
            healthPoints.value = (float) state;
            if (healthPoints.value == 0)
            {
                Die();
            }
        }

        public bool? Evaluate(string predicate, string[] parameters)
        {
            switch (predicate)
            {
                case "IsBossDied":
                    return IsBossDied(parameters);                
            }

            return null;
        }

        private bool IsBossDied(string[] parameters)
        {
            return boss.GetComponent<Health>().IsDead();
        }
    }
}


