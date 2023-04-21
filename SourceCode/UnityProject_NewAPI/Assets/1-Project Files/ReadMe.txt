THIS BRANCH CONTAINS A REMAKE OF THE ORIGINAL PROJECT IN MASTER BRANCH
THEREFORE IT IS COMPLETELY A DIFFERENT PROJECT. DO NOT MERGE!

******************NOTES************************

Before you begin:

# Downloaded Unity 2021.1.22f1 in order to avoid version conflicts

# teslasuit_newapi branch is not compatible with the master branch. These are two
seperate projects doing the same thing.

#This project currently has
- Animate character using tesla suit
- Save rotation (quaternion) and/or rotation & position (vector3) data
- Extract and save them using a json format
- Replay the data
- Send the rotation and position data to python via python client

#This project currently lacks
- Determining the accuracy of trainings (python side)
- Give a feedback accordingly (python side)

# Steps to work with this Project

1- Git clone repository, teslasuit_newapi branch
2- git lfs fetch -all
3- git lfs pull
4- Open the project in Unity, 
check Project Settings>Player> Configuration>API Comp Level set to -NET 4.x
5- Unity shouldn't throw unknown errors now.
6- Disable auto jump detector of suit (it causes wrong behaviours)
	- ProgramData/Teslasuit/teslasuit_api.config.xml
	- Set true in the following line: <without_jumps type="bool">true</without_jumps>

# Take Note

1- All of the stuff in "1- Project Files" have been 
added and they aren't originally a part of the TeslaSuit's own API

2- However some changes have been made in other packages
as well. Those changes have been marked with   //*

For an example TsHumanAnimator.cs

3- API could get new upgrades over time. As long as the data types stay same and enums dont change their
names or their order, the project should work fine

4- "1-ProjectFiles/Scenes/MotionCaptureCaptureScene" is the scene where changes have been made.
Sometimes Unity loses references in a scene for an unknown reason. A video has been uploaded  to showcase
every reference to every gameobject in this scene

5- In order to work with Json.Net it needs to be downloaded and imported from asset store

https://assetstore.unity.com/packages/tools/input-management/json-net-for-unity-11347

6-If the console still throw errors it is either a gizmo problem or python client problem.
if it is gizmos- ignore. If it is python, disable Datagateway component and try again. If it
still doesnt work go to "Assets/1-ProjectFiles/Plugins" there should be two plugins AsyncIO
and NetMq. They are responsible for python communication as well. Check if they are there.
If not, refer to the original thesis papers. Steps to use them are explained there.

7- Due to Unity's internal 3D Object workflow, the blender version installed in local computers
have problems reading the fbx files. This causes the gameobjects to lose their mesh data and become invisible.
If that occurs to you visit https://github.com/keijiro/KinoBinary/blob/master/Assets/Standard%20Assets/Characters/ThirdPersonCharacter/Models/Ethan.fbx
and download the fbx file. Import it back to project and restore the missing Ethan mesh data.

Contact : burakhandogan93@gmail.com

***************************************************** 
