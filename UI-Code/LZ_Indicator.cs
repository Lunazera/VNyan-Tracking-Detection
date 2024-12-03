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
        public GameObject indicator_on;
        public GameObject indicator_off;
        public void Update()
        {
            if(LZ_TrackingDetection.LZ_TrackDetect_Flag == 0)
            {
                indicator_on.SetActive(false);
                indicator_off.SetActive(true);
            } else
            {
                indicator_on.SetActive(true);
                indicator_off.SetActive(false);
            }
        }
    }

    
}
