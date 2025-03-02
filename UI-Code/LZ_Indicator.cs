using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.EventSystems;
using LZ_TrackingDetection;
using VNyanInterface;

namespace LZ_TrackingDetection
{
    class LZ_Indicator : MonoBehaviour
    {
        public GameObject indicator_state0;
        public GameObject indicator_state1;
        public GameObject indicator_state2;
        public GameObject indicator_state3;

        public void Update()
        {
            switch(LZ_TrackingDetection.LZ_TrackDetect_Flag)
            {
                case 0:
                    indicator_state0.SetActive(true);
                    indicator_state1.SetActive(false);
                    indicator_state2.SetActive(false);
                    indicator_state3.SetActive(false);
                    break;
                case 1:
                    indicator_state0.SetActive(false);
                    indicator_state1.SetActive(true);
                    indicator_state2.SetActive(false);
                    indicator_state3.SetActive(false);
                    break;
                case 2:
                    indicator_state0.SetActive(false);
                    indicator_state1.SetActive(false);
                    indicator_state2.SetActive(true);
                    indicator_state3.SetActive(false);
                    break;
                case 3:
                    indicator_state0.SetActive(false);
                    indicator_state1.SetActive(false);
                    indicator_state2.SetActive(false);
                    indicator_state3.SetActive(true);
                    break;
                case 4:
                    indicator_state0.SetActive(false);
                    indicator_state1.SetActive(false);
                    indicator_state2.SetActive(true);
                    indicator_state3.SetActive(false);
                    break;
            }
        }
    }
}
