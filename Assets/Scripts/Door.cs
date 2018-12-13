using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Door : MonoBehaviour
{
    public Building building;

#if UNITY_EDITOR
    private void OnValidate()
    {
        UnityEditor.Undo.RecordObject(building, "Auto Set Building");
        building = transform.parent.GetComponent<Building>();

        UnityEditor.Undo.RecordObject(building.door, "Auto Set Buildings Door");
        building.door = this;
    }
#endif

    private void OnTriggerEnter(Collider other)
    {
        var person = other.GetComponent<Person>();

        if (person)
        {
            person.DoorCollision(this);
        }
    }

}
