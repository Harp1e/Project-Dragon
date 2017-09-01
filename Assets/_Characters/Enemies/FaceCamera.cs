﻿using UnityEngine;

namespace RPG.Characters
{
    public class FaceCamera : MonoBehaviour
    {
        void LateUpdate ()
        {
            transform.LookAt (Camera.main.transform);
        }
    }
}
