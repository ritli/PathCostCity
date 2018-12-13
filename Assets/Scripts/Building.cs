using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    List<Person> peopleInBuilding;
    public Door door;

    Transform peopleContainer;

    TMPro.TextMeshPro text;
    Camera camera;

    void Start()
    {
        camera = Camera.main;
        text = Instantiate(Resources.Load<GameObject>("TextTemplate"), transform.position + Vector3.up * 3, Quaternion.identity, transform).GetComponent<TMPro.TextMeshPro>();

        peopleContainer = new GameObject("PeopleInBuilding").transform;
        peopleContainer.transform.parent = transform;
        peopleContainer.transform.position = transform.position;

        if (door)
        {
            door.building = this;
        }

        Manager.RegisterBuilding(this);

        UpdateText();
    }

    void UpdateText()
    {
        string s = "People in building:\n" + peopleContainer.childCount;

        text.text = s;
    }

    private void Update()
    {
        text.transform.LookAt(-camera.transform.position, Vector3.up);
    }

    public void Enter(Person enteringPerson)
    {
        enteringPerson.transform.parent = peopleContainer;
        enteringPerson.gameObject.SetActive(false);

        UpdateText();
    }

    public void Exit(Person exitingPerson)
    {
        exitingPerson.transform.parent = null;
        exitingPerson.gameObject.SetActive(true);

        UpdateText();
    }
}
