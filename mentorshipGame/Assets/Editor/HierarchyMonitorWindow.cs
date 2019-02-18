using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class HierarchyMonitorWindow : EditorWindow
{
    [MenuItem("Window/Hierarchy Monitor")]
    static void createWindow()
    {
        GetWindow<HierarchyMonitorWindow>();
    }
    void OnHierarchyChange()
    {
        var addedObjects = Resources.FindObjectsOfTypeAll<Object>()
                                    .Where(x => x is IGameObjectAddedToHierarchy);

        foreach (var item in addedObjects)
        {
            IGameObjectAddedToHierarchy gameObjectAddedToHierarchy = (IGameObjectAddedToHierarchy)item;
            
            //if (item.isAdded == 0) early setup

            if (gameObjectAddedToHierarchy.IsAdded() == false)
            {
                gameObjectAddedToHierarchy.AddToHierarchy();
                
            }
            
        }
    }
}
