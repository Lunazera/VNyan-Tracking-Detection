# VNyan Face Tracking Detection
Plugin to detect when your model loses tracking (ie when you go AFK). This uses some of your tracked blendshapes and calculates the variance to determine when VNyan loses tracking.

When tracking is lost, the `TrackingLost` trigger will signal.
When tracking is found again, the `TrackingDetected` trigger will signal.

You can create Trigger nodes with these trigger names in your graphs to set what happens when tracking is lost or found. The current tracking state is also saved under the parameter `LZ_TrackDetect_Flag`.

All exposed parameters can be found in the monitor with the prefix `LZ_TrackDetect_`

## Installation
1. Download the latest zip file
2. Unzip the contents in your VNyan folder. This will put the `.dll` and `.vnobj` inside `Items/Assemblies` for you.
3. The plugin should be present when you load VNyan! (you should see it in the plugin menu.

*Note: Remember to enable 3rd party plugins in VNyan under `Menu/Settings/Misc`*
