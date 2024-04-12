using System;
using System.Collections.Generic;
using UnityEngine;
using VNyanInterface;

namespace LZ_TrackingDetection
{
    public class LZ_TrackingDetection : MonoBehaviour
    {
        // Set public variables to be used by script
        [Header("Output Variables")]
        public string trackVarName = "TrackingVariance";
        public string trackDetectName = "TrackingDetected";

        [Header("Number of samples to calculate")]
        public int nSamples = 10;

        [Header("Sensitivity (how small variance to detect)")]
        public float sensitivity = 1;

        [Header("Time until Tracking Lost (in ms)")]
        public float trackTime = 3000;
        private float updateSpeed = 0;  // The update speed of the script will scale based on Tracking Lost time (time / number of samples)

        // Set private variables
        private float[] lastNSamples = new float[0];
        private float timeElapsed = 0.0f;

        // Set up container variables to store the blendshape readouts
        private float CheckVal1a = 0.0f;
        private float CheckVal1b = 0.0f;
        private float CheckVal2a = 0.0f;
        private float CheckVal2b = 0.0f;
        private float CheckVal3a = 0.0f;
        private float CheckVal3b = 0.0f;
        private float CheckVal4a = 0.0f;
        private float CheckVal4b = 0.0f;

        // Will be used to combine all the blendshapes into one parameter, we dont need to track individually
        private float CheckValCombined = 0.0f;

        // List of floats to track the last 60 frames of data
        
        private int sampleIndex = 0;  // We will keep the array a constant size and instead replace items in it cyclically
        private float sumSamples = 0.0f;
        private float meanSamples = 0.0f;
        

        // Variables for final output
        private float trackVariance = 0.0f;

        void Start()
        {
            // Calculate update speed based on our sample size and Time for tracking lost
            updateSpeed = trackTime / nSamples;

            // Re-initialize our container with our sample size
            lastNSamples = new float[nSamples];
            if (!(VNyanInterface.VNyanInterface.VNyanParameter == null))
            {
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(trackVarName, (float)trackVariance);
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(trackDetectName, 0.0f);
            }
        }

        // Update is called once per frame
        void Update()
        {
            timeElapsed = timeElapsed + Time.deltaTime;
            if (timeElapsed >= updateSpeed/1000)
            {
                timeElapsed = 0.0f;
                if (!(VNyanInterface.VNyanInterface.VNyanParameter == null))
                {
                    // Get current blendshape values
                    CheckVal1a = VNyanInterface.VNyanInterface.VNyanAvatar.getBlendshapeInstant("eyeWideLeft");
                    CheckVal1a = VNyanInterface.VNyanInterface.VNyanAvatar.getBlendshapeInstant("eyeWideRight");
                    CheckVal2a = VNyanInterface.VNyanInterface.VNyanAvatar.getBlendshapeInstant("BrowDownLeft");
                    CheckVal2b = VNyanInterface.VNyanInterface.VNyanAvatar.getBlendshapeInstant("BrowDownRight");
                    CheckVal3a = VNyanInterface.VNyanInterface.VNyanAvatar.getBlendshapeInstant("MouthSmileLeft");
                    CheckVal3b = VNyanInterface.VNyanInterface.VNyanAvatar.getBlendshapeInstant("MouthSmileRight");
                    CheckVal4a = VNyanInterface.VNyanInterface.VNyanAvatar.getBlendshapeInstant("EyeLookUpLeft");
                    CheckVal4b = VNyanInterface.VNyanInterface.VNyanAvatar.getBlendshapeInstant("EyeLookUpRight");

                    // Get current frame index (mod 60 has it loop
                    sampleIndex = (sampleIndex + 1) % nSamples;

                    // Combine our check values
                    CheckValCombined = (CheckVal1a + CheckVal1b + CheckVal2a + CheckVal2b + CheckVal3a + CheckVal3b + CheckVal4a + CheckVal4b) * 100;

                    // insert into the array
                    lastNSamples[sampleIndex] = CheckValCombined;

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

                    if (trackVariance > sensitivity)
                    {
                        VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(trackDetectName, 1.0f);
                    }
                    else
                    {
                        VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(trackDetectName, 0.0f);
                    }
                }
            }
        }
    }
}
