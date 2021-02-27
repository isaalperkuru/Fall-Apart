﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using RPG.Attributes;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        Health target = null;
        [SerializeField] float speed = 1f;
        [SerializeField] bool isHoming = true;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 10;
        [SerializeField] GameObject[] destroyOnHit = null;
        [SerializeField] float lifeAfterImpact = 2;
        [SerializeField] UnityEvent onHit;
        private Collider targetCollider;
        GameObject instigator = null;

        float damage = 0;

        private void Start()
        {
            transform.LookAt(GetVector());
        }
        // Update is called once per frame
        void Update()
        {
            if (target == null) return;
            if(isHoming && !target.IsDead()) transform.LookAt(GetVector());
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;

            Destroy(gameObject, maxLifeTime);
        }

        private Vector3 GetVector()
        {
            if (!targetCollider) {
                return target.transform.position;
            }
            else
            {
                targetCollider = target.GetComponent<Collider>();
            }
            return targetCollider.bounds.center;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != target) return;
            if (target.IsDead()) return;
            target.TakeDamage(instigator, damage);

            speed = 0;

            onHit.Invoke();

            if(hitEffect != null)
            {
                Instantiate(hitEffect, GetVector(), transform.rotation);
            }

            foreach (GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }

            Destroy(gameObject, lifeAfterImpact);
        }
    }
}

