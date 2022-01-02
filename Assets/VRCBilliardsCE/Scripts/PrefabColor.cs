﻿
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace VRCBilliards
{
    public class PrefabColor : UdonSharpBehaviour
    {
        public colorpicker PlayerPanel;
        public Color PrefabedColor = new Color(1.00f, 1.00f, 1.00f, 1.00f);

        public string materialName = "_Color";

        //public Renderer ButtonColor;
        public Image Button;

        private void Start()
        {
            //ButtonColor.material.SetColor(materialName, PrefabedColor);
            _ButtonColors(false);
        }

        public void _ButtonPress()
        {
            float H, S, V;
            Color.RGBToHSV(PrefabedColor, out H, out S, out V);
            Debug.Log("button works");
            PlayerPanel._PrefabPicker(H, S, V);
        }

        public void _ButtonColors(bool IO)
        {
            if (IO)
            {
                Button.color = PrefabedColor;
                Debug.Log("Button turned on");
            }
            else
            {
                Button.color = PrefabedColor * Color.black;
                Debug.Log("button turned off");
            }
        }
    }
}
