using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Manager : MonoBehaviour
{
    public Light sun;

    public int populationCount = 100;
    public float worldSize = 100;

    public int time = 0;
    float elapsedTime = 0;

    List<Person> peopleInWorld;
    public List<Building> buildingsInWorld;

    public static Manager instance;

    private void Start()
    {
        //Singleton check, destroys itself if more instances of manager are found.
        if (FindObjectsOfType<Manager>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;

            buildingsInWorld = new List<Building>();
            peopleInWorld = new List<Person>();

            for (int i = 0; i < populationCount; i++)
            {
                Vector3 pos = Random.insideUnitSphere * worldSize * 0.5f;
                NavMeshHit hit;

                Debug.DrawRay(pos, pos + Vector3.up, Color.red, 10);

                NavMesh.SamplePosition(pos, out hit, 100, NavMesh.AllAreas);

                Instantiate(Resources.Load("Person"), hit.position, Quaternion.identity);
            }
        }
    }

    public static void RegisterPerson(Person person)
    {
        instance.peopleInWorld.Add(person);
    }


    public static void RegisterBuilding(Building building)
    {
        instance.buildingsInWorld.Add(building);
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime > 1)
        {
            elapsedTime -= 1;

            time = (time + 1) % 24;

            TickTime();
        }
    }

    void TickTime()
    {
        print(time);

        foreach (Person person in peopleInWorld)
        {
            person.TickTime(time);
        }
    }

    public static Building GetRandomBuilding
    {
        get
        {
            return instance.buildingsInWorld[Random.Range(0, instance.buildingsInWorld.Count)];
        }
    }
}
