******************NOTES************************

Before you begin:

#Downloaded Unity 2021.1.22f1 in order to avoid version conflicts

1- Git clone repository
2- git lfs fetch -all
3- git lfs pull
4- Check Project Settings>Player> Configuration>API Comp Level set to -NET 4.x
5- Unity shouldn't throw unknown errors now.
6- Disable auto jump detector.
	- ProgramData/Teslasuit/teslasuit_api.config.xml
	- Set true in the following line: <without_jumps type="bool">true</without_jumps>


1- All of the stuff here have been added and they
aren't originally a part of the TeslaSuit's own API

2- However some changes have been made in the package
as well. Those changes have been marked with   //*
For an example TsHumanAnimator.cs

3- API could get new upgrades over time. As long as
the data types stay same and enums dont change their
names or their order, the project should work fine

4- 

Contact : burakhandogan93@gmail.com

***************************************************** 