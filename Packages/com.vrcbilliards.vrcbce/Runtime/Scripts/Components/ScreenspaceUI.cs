﻿using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace VRCBilliardsCE.Packages.com.vrcbilliards.vrcbce.Runtime.Scripts.Components
{
    /// <summary>
    /// Handles the pancake-mode (desktop) UI.
    /// TODO for 1.7.0: migrate all PoolStateManager desktop UI code in here
    /// </summary>
    public class ScreenspaceUI : UdonSharpBehaviour
    {
        [Tooltip("Animates the camera when entering the game in desktop")]
        public bool animateCamera;
        
        
        public GameObject escImage;
        public string startPlaying;
        public string quitPlaying;
        [SerializeField] public string androidMobileNotProperlySupported;
        public TextMeshProUGUI text;
        
        // Camera animation
        private Camera cam;
        private Quaternion headRot;
        private Vector3 headPos;
        private Quaternion camRot;
        private Vector3 camPos;
        [Range(0, 3f)]
        public float animationLen;

        private float animTime;

        public bool isAnimating;

        public void OnEnable()
        {
#if UNITY_ANDROID
            if (!Networking.LocalPlayer.IsUserInVR()) {
                text.text = androidMobileNotProperlySupported;
            }
#endif
        }

        public void _EnterDesktopTopDownView(Camera newCam)
        {
            text.text = quitPlaying;
            escImage.SetActive(true);

            if (!animateCamera)
            {
                return;
            }
            
            cam = newCam;
            camPos = cam.transform.position;
            camRot = cam.transform.rotation;

            VRCPlayerApi.TrackingData head = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            headPos = head.position;
            headRot = head.rotation;
            cam.transform.SetPositionAndRotation(headPos, headRot);
            isAnimating = true;
        }

        public void _ExitDesktopTopDownView()
        {
            text.text = startPlaying;
            escImage.SetActive(false);
            
            EndAnimation();
        }

        public void Update()
        {
            if (!animateCamera || !isAnimating)
            {
                return;
            }

            animTime += Time.deltaTime;
            var iLerp = Mathf.InverseLerp(0, animationLen, animTime);

            if (cam)
            {
                cam.transform.SetPositionAndRotation(Vector3.Lerp(headPos, camPos, iLerp), Quaternion.Lerp(headRot, camRot, iLerp));
            }
            
            if (iLerp >= 1)
            {
                EndAnimation();
            }
        }

        private void EndAnimation()
        {
            isAnimating = false;
            
            animTime = 0;

            if (!cam)
            {
                return;
            }
            
            cam.transform.SetPositionAndRotation(camPos, camRot);
        }
    }
}
