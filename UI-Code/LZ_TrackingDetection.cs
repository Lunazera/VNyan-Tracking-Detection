using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using VNyanInterface;
using System.Diagnostics;


namespace LZ_TrackingDetection
{
    public class LZ_TrackingDetection : MonoBehaviour
    {
        // Set public variables (with default settings in Unity) //
        [Header("Time until Tracking Lost (in ms)")]
        public string parameterNameInputTimeoutTime = "LZ_TrackDetect_TimeoutTime";
        public float trackTime = 3000f;

        [Header("Time until Tracking Found (in ms)")]
        public string parameterNameInputTimeinTime = "LZ_TrackDetect_TimeinTime";
        public float trackTimeIn = 1000f;

        [Header("Comma separated list of blendshapes to read from")]
        public string parameterNameInputBlendshapes = "LZ_TrackDetect_Blendshapes";
        public string blendshapesToRead = "";

        // Set private variables //
        private float timeElapsed = 0f;

        public static float LZ_TrackDetect_Flag = 0f;  // 0 = initial, 1 = found, 2 = first lost, 3 = lost
        private string trackFlagName = "LZ_TrackDetect_Flag";

        private float trackPause_Flag = 0f;
        private string parameterNametrackPause = "LZ_TrackDetect_Pause";

        // Will be used to combine all the blendshapes into one parameter, we dont need to track individually
        private float CheckValCombined = 0f;
        private float CheckValCombinedPrev = 0f;

        // allows for either comma-separated or semi-colon separated (VNyan's default)
        private char[] delimChars = { ';', ',' };


        // How much time in s until trackingis lost
        // trackTime
        // track lost time compared to now
        private float timeSinceLost = 0f;

        void Start()
        {
            LZ_TrackDetect_Flag = 0f;

            // Go through our dictionary of parameters, and load in the settings if they exist. These will overwrite the defaults above.
            // VNyanParameters is a dictionary set up in Sja_UICore. It loads in first (with Awake()) so by the time this runs we should already have checked
            // if there was a settings file and loaded it in
            if (Sja_UICore.VNyanParameters.ContainsKey(parameterNameInputTimeoutTime))
            {
                trackTime = Convert.ToSingle(Sja_UICore.VNyanParameters[parameterNameInputTimeoutTime]);
            }
            if (Sja_UICore.VNyanParameters.ContainsKey(parameterNameInputTimeinTime))
            {
                trackTimeIn = Convert.ToSingle(Sja_UICore.VNyanParameters[parameterNameInputTimeinTime]);
            }
            if (Sja_UICore.VNyanParameters.ContainsKey(parameterNameInputBlendshapes))
            {
                blendshapesToRead = Sja_UICore.VNyanParameters[parameterNameInputBlendshapes];
            }

            // Sets parameters into VNyan
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(parameterNameInputTimeoutTime, trackTime);
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterString(parameterNameInputBlendshapes, blendshapesToRead);
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(trackFlagName, LZ_TrackDetect_Flag);
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(parameterNametrackPause, trackPause_Flag);
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(parameterNameInputTimeinTime, trackTimeIn);
        }

        void Update()
        {
            // track how much time has elapsed since last update
            timeElapsed += Time.deltaTime;

            if (timeElapsed >= 0.25) // instead of checking every frame, check after enough time has elapsed according to our update speed
            {
                timeElapsed = 0f;
                trackPause_Flag = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(parameterNametrackPause);
                if (trackPause_Flag == 0) // Check if we're paused. Pausing freezes the plugin's tracking state
                {
                    // Get values from VNyan
                    trackTime = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(parameterNameInputTimeoutTime);
                    trackTimeIn = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterFloat(parameterNameInputTimeinTime);
                    blendshapesToRead = VNyanInterface.VNyanInterface.VNyanParameter.getVNyanParameterString(parameterNameInputBlendshapes);

                    // Read in blendshapes from blendshape list.
                    CheckValCombined = 0;
                    foreach (string blendshapeString in blendshapesToRead.Split(delimChars))
                    {
                        CheckValCombined += VNyanInterface.VNyanInterface.VNyanAvatar.getBlendshapeInstant(blendshapeString.Trim());
                    }

                    switch(LZ_TrackDetect_Flag) // 0 = initial, 1 = found, 2 = first lost, 3 = lost
                    {
                        case 0: // Initial State
                            if (CheckValCombined != CheckValCombinedPrev)
                            {
                                CheckValCombinedPrev = CheckValCombined;
                                LZ_TrackDetect_Flag = 1f; // Set to Found
                                VNyanInterface.VNyanInterface.VNyanTrigger.callTrigger("TrackingStart", 0, 0, 0, "TrackingStart", "", "");
                                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(trackFlagName, LZ_TrackDetect_Flag);
                            }
                            break;

                        case 1: // Found State
                            if (CheckValCombined != CheckValCombinedPrev)
                            {
                                CheckValCombinedPrev = CheckValCombined;
                            } 
                            else
                            {
                                LZ_TrackDetect_Flag = 2f; // Set to initial lost
                                timeSinceLost = Time.time;
                                VNyanInterface.VNyanInterface.VNyanTrigger.callTrigger("TrackingLostTimeout", 1, 0, 0, "TrackingFound", "", "");
                                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(trackFlagName, LZ_TrackDetect_Flag);
                            }
                            break;

                        case 2: // Tracking Initially Lost
                            if (CheckValCombined == CheckValCombinedPrev)
                            {
                                // If blendshape values have not changed, continue timeout wait
                                if ((Time.time - timeSinceLost) > (trackTime/1000))
                                {
                                    // At end of timout wait, go to "tracking lost"
                                    LZ_TrackDetect_Flag = 3f; // Set to lost, call trigger
                                    VNyanInterface.VNyanInterface.VNyanTrigger.callTrigger("TrackingLost", 2, 0, 0, "TrackingLostTimeout", "", "");
                                    VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(trackFlagName, LZ_TrackDetect_Flag);
                                }
                            } 
                            else
                            {
                                // If blendshape values change, end timeout wait and go back to "tracking found"
                                CheckValCombinedPrev = CheckValCombined;
                                LZ_TrackDetect_Flag = 1f; // Set to Found, don't call trigger
                                VNyanInterface.VNyanInterface.VNyanTrigger.callTrigger("TrackingFound", 2, 0, 0, "TrackingLostTimeout", "", "");
                                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(trackFlagName, LZ_TrackDetect_Flag);
                            }
                            break; 

                        case 3: 
                            // Tracking Lost
                            // if check values are the same still, then just update our 
                            if (CheckValCombined != CheckValCombinedPrev)
                            {
                                CheckValCombinedPrev = CheckValCombined;
                                // Go to Tracking Initially Found
                                LZ_TrackDetect_Flag = 4f;
                                timeSinceLost = Time.time;
                                VNyanInterface.VNyanInterface.VNyanTrigger.callTrigger("TrackingFoundTimeout", 3, 0, 0, "TrackingLost", "", "");
                                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(trackFlagName, LZ_TrackDetect_Flag);
                            }
                            break;

                        case 4: 
                            // Tracking Initially Found
                            if (CheckValCombined != CheckValCombinedPrev)
                            {
                                CheckValCombinedPrev = CheckValCombined;
                                if ((Time.time - timeSinceLost) > (trackTimeIn/1000))
                                {
                                    // Go to Tracking Found
                                    LZ_TrackDetect_Flag = 1f;
                                    VNyanInterface.VNyanInterface.VNyanTrigger.callTrigger("TrackingFound", 4, 0, 0, "TrackingFoundTimeout", "", "");
                                    VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(trackFlagName, LZ_TrackDetect_Flag);
                                }
                            } 
                            else
                            {
                                // Go to Tracking Lost
                                CheckValCombinedPrev = CheckValCombined;
                                LZ_TrackDetect_Flag = 3f;
                                VNyanInterface.VNyanInterface.VNyanTrigger.callTrigger("TrackingLost", 4, 0, 0, "TrackingFoundTimeout", "", "");
                                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterFloat(trackFlagName, LZ_TrackDetect_Flag);
                            }
                                break;
                    }
                }
            }
        }
    }
}


