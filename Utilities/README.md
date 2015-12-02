#mnUtilities/Utilities/
A collection of different components which can not be set under a more specific subfolder, is placed here.
All components can be found in the Unity editor menu under: Component->Utilities.

##InvokeMethod.cs
Script invokes a method attached to a GameObject.
The method and GameObject can be set directly in the Inspector window, or through a method call.
Invoking the set method and GameObject is done through a method call.

##ObjectPoolManager.cs
A script which creates a object pool for any GameObject. A object can be retrieved from/returned to the pool at runtime.
Retrieving/returning objects, using the pool, rather than creating/destroying them manually, saves up on a lot of garbage collection and performance.

##ObjectPoolObject.cs
A script which is attached to all objects which are created by the ObjectPoolManager.
This script is used to return the object to the object pool instead of destroying it.
