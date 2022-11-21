using System;
using BallBattle;
using UnityEditor;
using UnityEngine;

namespace Editors
{
    [CustomEditor(typeof(FieldOfView))]
    public class FieldOfViewEditor : Editor
    {
        private void OnSceneGUI()
        {
            FieldOfView fov = (FieldOfView)target;
            Handles.color = Color.white;
            Handles.DrawWireArc(fov.transform.position,Vector3.up, Vector3.forward, 360,fov.radius);
            
            Vector3 viewAngleA = DirectionFromAngel(fov.transform.eulerAngles.y, -fov.angle / 2);
            Vector3 viewAngleB = DirectionFromAngel(fov.transform.eulerAngles.y, fov.angle / 2);
            
            Handles.color = Color.red;
            Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.radius);
            Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.radius);
        }

        private Vector3 DirectionFromAngel(float eulerY, float angleInDegress)
        {
            angleInDegress += eulerY;
            return new Vector3(Mathf.Sin(angleInDegress*Mathf.Deg2Rad),0,Mathf.Cos(angleInDegress*Mathf.Deg2Rad));
        }
    }
}