# Rondo-Unity

# How to

* Open project in Unity
* Goto: File -> Build Settings
* Choose platform (iOS/Android)

### Unity player

Select desired platform from `Build Settings`, use `Switch Platform`, click run in Unity Editor.
Note: Platform specific features won't work in Editor.

### iOS Setup
Click `Build and Run`, choose folder to export XCode project, open the XCode project and run on iOS device.
In case of Leanplum.framework not found, make sure that Leanplum.framework is added to embedded libraries. 
Note: Unity app cannot be run on simulator.

![alt text](https://github.com/Leanplum/Rondo-Unity/blob/master/Screenshots/Screen%20Shot%202019-01-15%20at%2014.29.30.png "iOS Screenshot")

### Android Setup

This will export android gradle project, in case you want to run it directly on device, uncheck `Export project` and click `Build and Run`

![alt text](https://github.com/Leanplum/Rondo-Unity/blob/master/Screenshots/Screen%20Shot%202019-01-15%20at%2014.29.28.png "Android Screenshot")
