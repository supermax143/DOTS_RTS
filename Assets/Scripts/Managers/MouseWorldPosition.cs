using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tools
{
    public class MouseWorldPosition : MonoBehaviour
    {

        public Vector3 GetPositon()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            if (plane.Raycast(ray, out float distance))
            {
                return ray.GetPoint(distance);
            }
            else
            {
                return Vector3.zero;
            }
            
        }
        
    }
}