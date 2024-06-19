# LZ's Face Tracking Detection plugin for VNyan
Plugin to detect when your model loses tracking (ie when you go AFK). This uses some of your tracked blendshapes and calculates the total variance over a small window of time to determine when VNyan loses tracking. By default this uses ARKit blendshapes, but you can change those to be any blendshapes you want in the settings (though they'd need to be related to your face tracking in some way).

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
*You will also need at least v1.4.0*

## Plugin Menu Settings
*Make sure to hit Apply after making any changes so that the plugin can start using those changes!!*
#### Sensitivity
Minimum blendshape variance until tracking is considered "lost".
#### Tracking Timout (ms)
Time in milliseconds that blendshape variance must stay below the *Sensitivity* value until tracking is considered "lost".
#### Blendshapes
List of blendshapes to use in tracking detection. You can add or remove any blendshapes you want for this to listen to. Must be separated by comma's `,`.

Here is the list of blendshapes I use as a default. This generally works for me, but you can add/remove to what works best for your tracking situation. The important thing is that at least *some* of the blendshapes in this list are tracked (which you can double check in VNyan's Monitor).
```
eyeWideLeft, eyeWideRight, BrowDownLeft, BrowDownRight, MouthSmileLeft, MouthSmileRight, EyeLookUpLeft, EyeLookUpRight
``` 

*VNyan is credited to [Suvidriel](https://suvidriel.itch.io/vnyan)*
*Credit to [Sjatar](https://github.com/Sjatar/Screen-Light) for their UI code which this UI is adapted from.*
