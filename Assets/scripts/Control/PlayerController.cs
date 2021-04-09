using System.Collections;
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
            //checking every frame if there is an interaction with movement
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
            //target is a vector 3 will be store transformation data
            Vector3 target;
            //has ray laser hit to something in the scene. Doing it with RaycastNavMesh function written.
            bool hasHit = RaycastNavMesh(out target);
            //if there is hit
            if (hasHit)
            {
                //if we can move to target position
                if (!GetComponent<Mover>().CanMoveTo(target)) return false;
                //if mouse right click is clicked
                if (Input.GetMouseButton(0))
                {
                    //start the move action.
                    GetComponent<Mover>().StartMoveAction(target, 1f);
                    //go to position ray (laser) hit
                }
                //if mouse on moveable place update the cursor
                SetCursor(CursorType.Movement);
                return true;

            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            //giving a default x=0,y=0,z=0 data for initialization.
            target = new Vector3();

            //left click laser info
            RaycastHit hit;
            //if we hit something. 
            //out allows us to return information about the location that a raycast has hit
            //this function also updating hit because we use out for hit
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            //if there is no hit
            if (!hasHit) return false;
            //collecting navmesh hit data that is about navigation system for terrains in the scene. Unity gives you this navigation system.
            NavMeshHit navMeshHit;
            //Finds the nearest point based on the NavMesh within a specified range.
            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit,
                maxNavMeshProjectionDistance, NavMesh.AllAreas);
            //if there is no path.
            if (!hasCastToNavMesh) return false;
            //update target position nearest position to our target.
            target = navMeshHit.position;
            //return true
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
            //I return exact position from the camera because in the game there is no mouse cursor gameObject
            //and returning exact position of the mouse ray from the camera is tricky but I used Unity method for solving this problem easily
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

