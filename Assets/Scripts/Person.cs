using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Person : MonoBehaviour
{
    string[] vowels = { "a", "e", "i", "o", "u" };
    string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "x", "z", };

    bool isWorking;
    bool isInBuilding;

    public Building home;
    public Building work;

    [Range(0,23)]
    public int workTimeStart, workTimeEnd;

    Building currentBuilding;

    private NavMeshAgent navAgent;

    public int personnummer;

    Door destination;

    public Material lateMat;
    public Material normalMat;

    MeshRenderer renderer;

    string GenerateName(int length)
    {
        int currentConsonantCount = 0;
        int consonantCount = length / 3;
        bool lastIndexWasConsonant = false;

        string s = "";

        for (int i = 0; i < length; i++)
        {
            if (currentConsonantCount < consonantCount && !lastIndexWasConsonant && Random.value > 0.6f)
            {


                s += consonants[Random.Range(0, consonants.Length)];
                lastIndexWasConsonant = true;
            }
            else
            {
                s += vowels[Random.Range(0, vowels.Length)];
                lastIndexWasConsonant = false;
            }

            if (i == 0)
            {
                s = s.ToUpper();
            }
        }

        return s;
    }

    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        normalMat = renderer.material;

        name = GenerateName(Random.Range(5, 12));

        workTimeStart = Random.Range(0, 23);
        workTimeEnd = Random.Range(0, 23);

        Manager.RegisterPerson(this);

        work = Manager.GetRandomBuilding;
        home = Manager.GetRandomBuilding;

        navAgent = GetComponent<NavMeshAgent>();
    }

    public void TickTime(int time)
    {
        if ((time - 2) % 24 == workTimeStart)
        {
            ExitCurrentBuilding();

            destination = work.door;
            navAgent.SetDestination(work.door.transform.position);
        }
        else if (time == workTimeEnd)
        {
            if (isWorking)
            {
                ExitCurrentBuilding();

                destination = home.door;
                navAgent.SetDestination(home.door.transform.position);
            }
        }

        if (!currentBuilding && time == (workTimeStart + 1) % 24)
        {
            renderer.material = lateMat;
        }
    }

    void ExitCurrentBuilding()
    {
        if (currentBuilding)
        {
            renderer.material = normalMat;

            currentBuilding.Exit(this);
            currentBuilding = null;
        }
    }

    public void DoorCollision(Door door)
    {
        if (door == destination)
        {
            isWorking = door.building == work;
            currentBuilding = door.building;

            destination.building.Enter(this);
        }
    }
}
