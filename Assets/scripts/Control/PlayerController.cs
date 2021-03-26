﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using Game.Inventories;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health health;

        [SerializeField] KeyCode actionKey1 = KeyCode.Alpha1;
        [SerializeField] KeyCode actionKey2 = KeyCode.Alpha2;
        [SerializeField] KeyCode actionKey3 = KeyCode.Alpha3;
        [SerializeField] KeyCode actionKey4 = KeyCode.Alpha4;
        [SerializeField] KeyCode actionKey5 = KeyCode.Alpha5;
        [SerializeField] KeyCode actionKey6 = KeyCode.Alpha6;

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] float raycastRadius = 1f;

        bool isDraggingUI = false;

        private void Awake()
        {
            health = GetComponent<Health>();
        }
        // Update is called once per frame
        void Update()
        {
            CheckActionKeys();

            if (InteractWithUI()) return;
            if (health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }
            if (InteractWithComponent()) return;
            if(InteractWithMovement()) return;

            SetCursor(CursorType.None);
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach(IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
            float[] distances = new float[hits.Length];
            for(int i=0;i< hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits;
        }

        private bool InteractWithUI()
        {
            if (Input.GetMouseButtonUp(0))
            {
                isDraggingUI = false;
            }
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isDraggingUI = true;
                }
                SetCursor(CursorType.UI);
                return true;
            }
            if (isDraggingUI)
            {
                return true;
            }
            return false;
        }

        private bool InteractWithMovement()
        {
            
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit)
            {
                if (!GetComponent<Mover>().CanMoveTo(target)) return false;

                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(target, 1f);
                    //go to position ray (laser) hit
                }
                SetCursor(CursorType.Movement);
                return true;

            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();

            //left click laser info
            RaycastHit hit;
            //if we hit something. 
            //out allows us to return information about the location that a raycast has hit
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit) return false;
            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit,
                maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if (!hasCastToNavMesh) return false;

            target = navMeshHit.position;

            return true;
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach(CursorMapping mapping in cursorMappings)
            {
                if(mapping.type == type)
                {
                    return mapping;
                }
            }
            return cursorMappings[0];
        }

        private static Ray GetMouseRay()
        {
            //left click laser from cam
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
        private void CheckActionKeys()
        {
            var actionStore = GetComponent<ActionStore>();
            if (Input.GetKeyDown(actionKey1))
            {
                actionStore.Use(0, gameObject);
            }
            if (Input.GetKeyDown(actionKey2))
            {
                actionStore.Use(1, gameObject);
            }
            if (Input.GetKeyDown(actionKey3))
            {
                actionStore.Use(2, gameObject);
            }
            if (Input.GetKeyDown(actionKey4))
            {
                actionStore.Use(3, gameObject);
            }
            if (Input.GetKeyDown(actionKey5))
            {
                actionStore.Use(4, gameObject);
            }
            if (Input.GetKeyDown(actionKey6))
            {
                actionStore.Use(5, gameObject);
            }
        }
    }
}

