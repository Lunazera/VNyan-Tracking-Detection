# LZ's Face Tracking Detection plugin for VNyan
Plugin to detect when your model loses tracking (ie when you go AFK). By default this uses ARKit blendshapes, but you can change those to be any blendshapes you want in the settings (though they'd need to be related to your face tracking in some way).

![example image of plugin](https://github.com/Lunazera/VNyan-Tracking-Detection/blob/main/example.png)

## How To Use
You can create Trigger nodes with these trigger names in your graphs to set what happens when tracking is lost or found.
- `TrackingStart` trigger will signal when tracking is first found *for the very first time*.
- `TrackingLost` trigger will signal when tracking is lost.
- `TrackingFound` trigger will signal when tracking is found.
- `TrackingLostTimeout` trigger will signal when tracking is *first* lost.
- `TrackingFoundTimeout` trigger will signal when tracking is *first* found.

The "Timeout" states will trigger when tracking is first lost or found, letting you have a period of waiting to see if tracking is actually lost/found before going to the new state. Each trigger will store the tracking state it is transitioning *from* in the Text 1 socket.

The current tracking state is also saved under the parameter `LZ_TrackDetect_Flag`. All exposed parameters can be found in the monitor with the prefix `LZ_TrackDetect_`

You can also use the `sleepyAFK` Graph included for a simple AFK setup that will have your model close their eyes and tilt their head down when tracking is lost, then reset when it's found again.

### Examples
Example 1, say your tracking is lost and you move back towards your camera, but you don't want "tracking found" to occur until you've been in view of your camera for long enough. First, `TrackingFoundTimeout` will send a signal, with "TrackingLost" in text 1. Then, `TrackingFound` will send a signal, with "TrackingFoundTimeout" in text 1. 

Example 2, say you move into view of your camera, but then move away again before the timeout is complete. First, `TrackingFoundTimeout` will send a signal, with "TrackingLost" in text 1. Then, `TrackingLost` will send a signal, with "TrackingFoundTimeout" in text 1. 


## Installation
1. Download the latest zip file from [releases](https://github.com/Lunazera/VNyan-Tracking-Detection/releases/)
2. Unzip the contents in your VNyan folder. This will put the `.dll` and `.vnobj` inside `Items/Assemblies` for you.
3. The plugin should be present when you load VNyan! (you should see it in the plugin menu.)

*Note: Remember to enable 3rd party plugins in VNyan under `Menu/Settings/Misc`*

## Plugin Menu Settings
*Make sure to hit Apply after making any changes so that the plugin can start using those changes!!*
#### Tracking Lost Timeout (ms)
Time in milliseconds that blendshapes must not change until tracking is considered "lost".
#### Tracking Found Timeout (ms)
Time in milliseconds that blendshapes must continue changing until tracking is considered "found".
#### Blendshapes
List of blendshapes to use in tracking detection. You can add or remove any blendshapes you want for this to listen to. Must be separated by comma's `,`.

Here is the list of blendshapes I use as a default. This generally works for me, but you can add/remove to what works best for your tracking situation. The important thing is that at least *some* of the blendshapes in this list are tracked (which you can double check in VNyan's Monitor). Also, blendshapes with spaces in their name wont work here.
```
eyeWideLeft; eyeWideRight; BrowDownLeft; BrowDownRight; MouthSmileLeft; MouthSmileRight; EyeLookUpLeft; EyeLookUpRight;
``` 

*VNyan is credited to [Suvidriel](https://suvidriel.itch.io/vnyan)*

*Special thanks to [Sjatar](https://github.com/Sjatar/Screen-Light) for their UI code which this UI is adapted from.*
