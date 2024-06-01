# LZ's Face Tracking Detection plugin for VNyan
Plugin to detect when your model loses tracking (ie when you go AFK). This uses some of your tracked blendshapes and calculates the total variance over a small window of time to determine when VNyan loses tracking.
![Cover image for face tracking plugin](./FaceTrackingDetection_Image.png)

## How To Use
You can create Trigger nodes with these trigger names in your graphs to set what happens when tracking is lost or found.
- The `TrackingLost` trigger will signal when tracking is lost.
- The `TrackingFound` trigger will signal when tracking is found again.

The current tracking state is also saved under the parameter `LZ_TrackDetect_Flag`. All exposed parameters can be found in the monitor with the prefix `LZ_TrackDetect_`

You can also use the `sleepyAFK` Graph included for a simple AFK setup that will have your model close their eyes and tilt their head down when tracking is lost, then reset when it's found again.

## Installation
1. Download the latest zip file from [releases](https://github.com/Lunazera/VNyan-Tracking-Detection/releases/)
2. Unzip the contents in your VNyan folder. This will put the `.dll` and `.vnobj` inside `Items/Assemblies` for you.
3. The plugin should be present when you load VNyan! (you should see it in the plugin menu.

*Note: Remember to enable 3rd party plugins in VNyan under `Menu/Settings/Misc`*

## Plugin Menu Settings
#### Sensitivity
Minimum blendshape variance until tracking is considered "lost".
#### Tracking Timout (ms)
Time in milliseconds that blendshape variance must stay below the *Sensitivity* value until tracking is considered "lost".
#### Blendshapes
List of blendshapes to use in tracking detection. You can add or remove any blendshapes you want for this to listen to. Must be separated by comma's `,`.

*Credit to Sjatar for their UI code which this UI is adapted from.*
