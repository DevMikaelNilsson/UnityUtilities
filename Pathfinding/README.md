#Pathfinding

##FollowPathfinder.cs
Script which allows a object to follow another object with a Pathfinder script attached to it.
The object follows the Pathfinder objects position and rotation with a smooth linear interpolation.
This script can be very usefull to make the pathfinding movement more smooth and with combination with
other scripts which will alter the position/rotation of the object. This script should not be attached to a
object which already has the Pathfinder script attached to it.

##Pathfinder.cs
Pathfinder script based on Unitys build in NavMesh pathfinding.
The object finds a valid path and moves along the path until the destionation is reached.
The script has different pathfinding types to choose from, some types makes the object to move around automatic 
without any need of interaction. Other types the object needs to be manually activated in order to move along a path.
