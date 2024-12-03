using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using VNyanInterface;


namespace LZ_TrackingDetection
{
    public class LZ_TrackingDetection : MonoBehaviour
    {
        static float sampleVar(float[] lastNSamples, float nSamples, float meanSamples)
        {
            // calculates variance of the last set of samples
            float var_collector = 0;
            for (int i = 0; i < nSamples; i++)
            {
                var_collector += (lastNSamples[i] - meanSamples) * (lastNSamples[i] - meanSamples);
            }

            return var_collector / (nSamples - 1);
        }

        // Set public variables (with default settings in Unity)
        [Header("Number of samples to calculate")]
        public string parameterNameInputSamples = "LZ_TrackDetect_Samples";
        public int nSamples = 30;

        [Header("Sensitivity (how small variance to detect)")]
        public string parameterNameInputSensitivity = "LZ_TrackDetect_Sensitivity";
        public float sensitivity = 0.1f;

        [Header("Time until Tracking Lost (in ms)")]
        public string parameterNameInputTimeoutTime = "LZ_TrackDetect_TimeoutTime";
        public float trackTime = 3000f;

        [Header("Comma separated list of blendshapes to read from")]
        public string parameterNameInputBlendshapes = "LZ_TrackDetect_Blendshapes";
        public string blendshapesToRead = "";


        // Set private variables
        private float[] lastNSamples = new float[0];
        private float timeElapsed = 0f;

        // The update speed of the script will scale automatically based on Tracking Lost time (time / number of samples)
        private float updateSpeed = 0f;

        private string trackVarName = "LZ_TrackDetect_Variance";
        private string trackFlagName = "LZ_TrackDetect_Flag";
        public static float LZ_TrackDetect_Flag = 0f;  // 1 = detected, 0 = lost

        private float trackPause_Flag = 0f;
        private string parameterNametrackPause = "LZ_TrackDetect_Pause";

        // This flag is just to handle the beginning of the app differently. If you start VNyan and your tracking is off, you don't want to go AFK immediately.
        // It'll get set to false once tracking is found for the first time.
        private bool trackingStartFlag = true;

        // Will be used to combine all the blendshapes into one parameter, we dont need to track individually
        private float CheckValCombined = 0f;

        // List of floats to track the last 60 frames of data
        private int sampleIndex = 0;  // We will keep the array a constant size and instead replace items in it cyclically
        private float sumSamples = 0f;
        private float meanSamples = 0f;

        // Variables for final output
        private float trackVariance = 0f;

        // allows for either comma-separated or semi-colon separated (VNyan's default)
        private char[] delimChars = { ';', ',' };

        void Start()
        {
            // Re-initialize our container with our sample size at start
            lastNSamples = new float[nSamples];

            // Calculate update speed based on our sample size and Time for tracking lost
            updateSpeed = trackTime / nSamples;

            // Go through our dictionary of parameters, and load in the settings if they exist. These will overwrite the defaults above.
            // VNyanParameters is a dictionary set up in Sja_UICore. It loads in first (with Awake()) so by the time this runs we should already have checked
            // if there was a settings file and loaded it in
            if (Sja_UICore.VNyanParameters.ContainsKey(parameterNameInputSensitivity))
            {
                sensitivity = Convert.ToSingle(Sja_UICore.VNyanParameters[parameterNameInputSensitivity]);
            }
            if (Sja_UICore.VNyanParameters.ContainsKey(parameterNameInputTimeoutTime))
            {
                trackTime = Convert.ToSingle(Sja_UICore.VNyanParameters[parameterNameInputTimeoutTime]);
            }
            if (Sja_UICore.VNyanParameters.ContainsKey(parameterNameInputBlendshapes))
            {
                blendshapesToRead = Sja_UICore.VNyanParameters[parameterNameInputBlendshapes];
            }

            // Sets parameters into VNyan
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(parameterNameInputSensitivity, sensitivity);
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(parameterNameInputTimeoutTime, trackTime);
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterString(parameterNameInputBlendshapes, blendshapesToRead);
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(trackFlagName, LZ_TrackDetect_Flag);
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(parameterNametrackPause, trackPause_Flag);
        }

        void Update()
        {
            // track how much time has elapsed since last update
            timeElapsed += Time.deltaTime;

            if (timeElapsed >= updateSpeed / 1000) // instead of checking every frame, check after enough time has elapsed according to our update speed
            {
                timeElapsed = 0f;
                trackPause_Flag = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(parameterNametrackPause);
                if (trackPause_Flag == 0) // Check if we're paused. Pausing freezes the plugin's tracking state
                {
                    // Get current values from VNyan parameters
                    sensitivity = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(parameterNameInputSensitivity);
                    trackTime = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(parameterNameInputTimeoutTime);
                    blendshapesToRead = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterString(parameterNameInputBlendshapes);

                    // Calculate update speed based on our sample size and Time for tracking lost
                    updateSpeed = trackTime / nSamples;

                    // Read in blendshapes from blendshape list.
                    CheckValCombined = 0;
                    foreach (string blendshapeString in blendshapesToRead.Split(delimChars))
                    {
                        CheckValCombined += VNyanInterface.VNyanInterface.VNyanAvatar.getBlendshapeInstant(blendshapeString.Trim());
                    }

                    // Get current frame index (mod 60 makes this loop every 60 checks)
                    sampleIndex = (sampleIndex + 1) % nSamples;

                    // insert into the array to keep our samples across checks. We scale by 100 just to keep the numbers from getting to small.
                    lastNSamples[sampleIndex] = CheckValCombined * 100;

                    sumSamples = lastNSamples.Sum();
                    meanSamples = sumSamples / nSamples;

                    // Calculate Variance of samples, as long as we actually had samples above 0 
                    if(meanSamples > 0)
                    {
                        trackVariance = sampleVar(lastNSamples, nSamples, meanSamples);
                    } 
                    else
                    {
                        trackVariance = 0;
                    }


                    // Output into parameter
                    // VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(trackVarName, trackVariance);

                    // Signal tracking state through parameter and trigger
                    // The if/else here with the flag just ensures we only send the triggers once
                    if (trackVariance > sensitivity)
                    {
                        if (LZ_TrackDetect_Flag == 0f)
                        {
                            LZ_TrackDetect_Flag = 1f;
                            VNyanInterface.VNyanInterface.VNyanTrigger.callTrigger("TrackingFound", 0, 0, 0, "", "", "");
                            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(trackFlagName, LZ_TrackDetect_Flag);
                            trackingStartFlag = false;
                        }
                    }
                    else
                    {
                        if (LZ_TrackDetect_Flag == 1f && trackingStartFlag is false)
                        {
                            LZ_TrackDetect_Flag = 0f;
                            VNyanInterface.VNyanInterface.VNyanTrigger.callTrigger("TrackingLost", 0, 0, 0, "", "", "");
                            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(trackFlagName, LZ_TrackDetect_Flag);
                        }
                    }
                }
            }
        }
    }
}


