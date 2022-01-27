/*******************************************************
 * Copyright (C) 2017 Doron Weiss  - All Rights Reserved
 * You may use, distribute and modify this code under the
 * terms of unity license.
 * 
 * See https://abnormalcreativity.wixsite.com/home for more info
 *******************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Dweiss {
	[System.Serializable]
	public class Settings : ASettings {
        public float simSpeed=3;
        public float stopL=0;
        public int simLength=500;
        public int simTimes = 6;
        public string output = "";
        /*
        [Header("--Simple primitive example--")]
        public bool show = true;

        public float colorChangeCycle = 1.5f;
        public int qualitySettings = 2;


        [Header("--Lists and arrays--")]
        public float[] arrayExample;

        public List<float> listExample;


        [Header("--Enum and Class--")]
        public EnumExample enumExample;
        public MySpecialClassExample classExample;



        #region Enums and classes for serialization

        public enum EnumExample {
            Enum1,Enum2
        }


        [System.Serializable]
        public class MySpecialClassExample
        {
            public string txt = "abcd";
        }
        #endregion


        private new void Awake() {
			base.Awake ();
            SetupSingelton();
        }


        #region  Singelton
        public static Settings _instance;
        public static Settings Instance { get { return _instance; } }
        private void SetupSingelton()
        {
            if (_instance != null)
            {
                Debug.LogError("Error in settings. Multiple singeltons exists: " + _instance.name + " and now " + this.name);
            }
            else
            {
                _instance = this;
            }
        }
        #endregion


        */
    }
}