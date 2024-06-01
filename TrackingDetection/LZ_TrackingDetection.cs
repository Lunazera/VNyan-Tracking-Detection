using System;
using System.Collections.Generic;
using UnityEngine;
using VNyanInterface;

namespace LZ_TrackingDetection
{
    public class LZ_TrackingDetection : MonoBehaviour
    {
        // Set public variables
        [Header("Number of samples to calculate")]
        public string parameterNameInputSamples = "LZ_TrackDetect_Samples";
        public int nSamples = 30;

        [Header("Sensitivity (how small variance to detect)")]
        public string parameterNameInputSensitivity = "LZ_TrackDetect_Sensitivity";
        public float sensitivity = 0.1f;

        [Header("Time until Tracking Lost (in ms)")]
        public string parameterNameInputTimeoutTime = "LZ_TrackDetect_TimeoutTime";
        public float trackTime = 3000;

        [Header("Comma separated list of blendshapes to read from")]
        public string parameterNameInputBlendshapes = "LZ_TrackDetect_Blendshapes";
        public string blendshapesToRead = "";

        // Set private variables
        private float[] lastNSamples = new float[0];
        private float timeElapsed = 0.0f;

        // The update speed of the script will scale based on Tracking Lost time (time / number of samples)
        private float updateSpeed = 0;

        private string trackVarName = "LZ_TrackDetect_Variance";
        private string trackFlagName = "LZ_TrackDetect_Flag";
        private int LZ_TrackDetect_Flag = 0;  // 1 = detected, 0 = lost

        // Will be used to combine all the blendshapes into one parameter, we dont need to track individually
        private float CheckValCombined = 0.0f;

        // List of floats to track the last 60 frames of data
        private int sampleIndex = 0;  // We will keep the array a constant size and instead replace items in it cyclically
        private float sumSamples = 0.0f;
        private float meanSamples = 0.0f;


        // Variables for final output
        private float trackVariance = 0.0f;

        // Initial run of script
        void Start()
        {
            // Re-initialize our container with our sample size
            lastNSamples = new float[nSamples];

            // Set parameters in VNyan according to parameters from script settings (later pulled from/saved in settings json)
            if (!(VNyanInterface.VNyanInterface.VNyanParameter == null))
            {
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(trackVarName, trackVariance);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(parameterNameInputSensitivity, sensitivity);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(parameterNameInputTimeoutTime, trackTime);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterString(parameterNameInputBlendshapes, blendshapesToRead);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(trackFlagName, LZ_TrackDetect_Flag);
            }

            // Calculate update speed based on our sample size and Time for tracking lost
            updateSpeed = trackTime / nSamples;
        }

        // Update is called once per frame
        void Update()
        {
            timeElapsed = timeElapsed + Time.deltaTime;
            if (timeElapsed >= updateSpeed / 1000)
            {
                timeElapsed = 0.0f;
                if (!(VNyanInterface.VNyanInterface.VNyanParameter == null))
                {
                    // Get New values for our parameters
                    sensitivity = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(parameterNameInputSensitivity);
                    trackTime = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(parameterNameInputTimeoutTime);
                    blendshapesToRead = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterString(parameterNameInputBlendshapes);

                    // Calculate update speed based on our sample size and Time for tracking lost
                    updateSpeed = trackTime / nSamples;


                    CheckValCombined = 0;
                    foreach (string blendshapeString in blendshapesToRead.Split(','))
                    {
                        CheckValCombined = CheckValCombined + VNyanInterface.VNyanInterface.VNyanAvatar.getBlendshapeInstant(blendshapeString.Trim());
                    }

                    // Get current frame index (mod 60 has it loop)
                    sampleIndex = (sampleIndex + 1) % nSamples;

                    // insert into the array to keep our samples across frames. We scale by 100 just to keep the numbers from getting to small.
                    lastNSamples[sampleIndex] = CheckValCombined * 100;

                    // Calculate sum of samples
                    sumSamples = 0;
                    for (int i = 0; i < nSamples; i++)
                    {
                        sumSamples += lastNSamples[i];
                    }

                    // Calculate Mean of samples
                    meanSamples = sumSamples / nSamples;

                    // Calculate Variance of samples
                    trackVariance = 0;
                    for (int i = 0; i < nSamples; i++)
                    {
                        trackVariance = trackVariance + (lastNSamples[i] - meanSamples) * (lastNSamples[i] - meanSamples) / (nSamples - 1);
                    }

                    // Output into parameter
                    VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(trackVarName, (float)trackVariance);

                    // Signal tracking state through parameter and trigger
                    if (trackVariance > sensitivity)
                    {
                        if (LZ_TrackDetect_Flag == 0)
                        {
                            LZ_TrackDetect_Flag = 1;
                            VNyanInterface.VNyanInterface.VNyanTrigger.callTrigger("TrackingFound");
                            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(trackFlagName, LZ_TrackDetect_Flag);
                        }
                    }
                    else
                    {
                        if (LZ_TrackDetect_Flag == 1)
                        {
                            LZ_TrackDetect_Flag = 0;
                            VNyanInterface.VNyanInterface.VNyanTrigger.callTrigger("TrackingLost");
                            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(trackFlagName, LZ_TrackDetect_Flag);
                        }
                    }
                }
            }
        }
    }
}


