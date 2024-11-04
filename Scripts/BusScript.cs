using System.Collections.Generic;
using UnityEngine;


public class BusScript : MonoBehaviour
{
    public int color;
    public int maxPass;
    public int pass = 0;
    public bool allInPos = false;
    public bool isGo = false;
    public bool move;
    public List<Transform> seats;
    public List<Transform> seatsPos;


    public List<PersonScript> personList = new List<PersonScript>();

    
    void Start()
    {
        GetComponentInChildren<MeshRenderer>().material.color = GameScript.Instance.colors[color];
    }

    void Update()
    {
        if(!allInPos)
        {
            foreach(PersonScript person in personList)
            {
                if(person.move || person.moveTarget) return;
            }
            allInPos = true;
        }

        if(isGo && allInPos)
        {
            GameScript.Instance.SendBus();
        } 

        if(move) GoAndDelete(GameScript.Instance.destroyPos);
    }

    public void GoAndDelete(Vector3 targetPos)
    {
        transform.position = Vector3.MoveTowards(transform.position,targetPos,GameScript.Instance.personSpeed*Time.deltaTime);

        if(Vector3.Distance(transform.position,targetPos) < .5f ) move = false;
        
    }




}
