#Pathfinding
Collection of scripts which handles pathfinding functionality in some way.
Normally the Pathfinding calculation(s) in these scripts are done with the Unitys built in NavMesh pathfinding.

##PathfinderBase.cs
A base component which any Pathfinder component can be based on.
The component contains all required components to move a object with the NavMeshAgent functionality.

##FollowPathfinder.cs
Script which allows a object to follow another object with a Pathfinder script attached to it.
The object follows the Pathfinder objects position and rotation with a smooth linear interpolation.
This script can be very usefull to make the pathfinding movement more smooth and with combination with
other scripts which will alter the position/rotation of the object. This script should not be attached to a
object which already has the Pathfinder script attached to it.

##PathfinderMoveTo.cs
Moves to a specific destination.
