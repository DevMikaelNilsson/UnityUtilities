using UnityEngine;
using System.Collections;

/// <summary>
/// A class which gets added to all objects which are created through a Object Pool Manager.
/// With this script the object has fast acess to its Object Pool Manager and other usefull components, which can come in handy at times.
/// </summary>
namespace mnUtilities.Utilities
{
    public class ObjectPoolObject : MonoBehaviour
    {
  	  public ObjectPoolManager ObjectPoolManagerObject = null;
    }
}
