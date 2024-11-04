using System.Collections.Generic;
using UnityEngine;

public class GridScript : MonoBehaviour
{
    GameObject person;
    public int color;
    public GridScript[] neighbours;
    public List<GameObject> personList = new List<GameObject>();
    public bool isAvaible;
    public bool personCreate;


    //A* 
    public float gScore,hScore;
    public GridScript cameFrom;
    public float FScore()
    {
        return gScore + hScore;
    }
    
    void Start()
    {
        //Komsulari bul -> array ekle
        RaycastHit hit;

        if(Physics.Raycast(transform.position, Vector3.forward,out hit, 1f, 1 << 6)) neighbours[0] = hit.collider.gameObject.GetComponent<GridScript>();
        if(Physics.Raycast(transform.position, Vector3.right,out hit, 1f, 1 << 6)) neighbours[1] = hit.collider.gameObject.GetComponent<GridScript>();
        if(Physics.Raycast(transform.position, Vector3.back,out hit, 1f, 1 << 6)) neighbours[2] = hit.collider.gameObject.GetComponent<GridScript>();
        if(Physics.Raycast(transform.position, Vector3.left,out hit, 1f, 1 << 6)) neighbours[3] = hit.collider.gameObject.GetComponent<GridScript>();

        if(personCreate) CreatePerson();
       // CheckNeighbours();
    }

    void Update()
    {
       // CheckNeighbours();
    }
    void CheckNeighbours()
    {
        foreach(GridScript grid in neighbours)
        {
            if(grid != null)
            {
                if(grid.isAvaible && grid.personList.Count == 0)
                {
                    isAvaible = true; //erisilebilir yap
                }   
            }       
        }
    }

    public void CreatePerson()
    {
        person = Instantiate(GameScript.Instance.person, transform.position, Quaternion.identity,transform.parent);
        person.GetComponent<PersonScript>().color = color;
        personList.Add(person);
        person.GetComponent<PersonScript>().grid = this;
    }


}
