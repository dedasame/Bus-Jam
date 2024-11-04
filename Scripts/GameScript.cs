using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    public GameObject[] levels;
    public int levelIndex = 0;
    public Color[] colors;

    public GameObject person;
    public float personSpeed = 6f;

    public List<SlotScript> slots;
    public List<GridScript> theGrids;

    public bool isFailed;


    public GameObject bus;
    public int[] busList;
    public BusScript activeBus;
    public Vector3 firstBusPos;
    public List<BusScript> busses;
    public int busIndex = 0;
    public Vector3 destroyPos;
    public bool allInPos;
    

    public GameObject failedScreen;
    public GameObject completedScreen;

    public static GameScript Instance;
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //yeni levele gecince degisenler
        theGrids = levels[levelIndex].GetComponent<LevelScript>().TheGrids;
        slots = levels[levelIndex].GetComponent<LevelScript>().slots;
        busList = levels[levelIndex].GetComponent<LevelScript>().Bus;


        for(int i = 0;i<busList.Count();i++)
        {
            busses.Add(Instantiate(bus,firstBusPos + i*new Vector3(-6,0,0), Quaternion.identity,levels[levelIndex].transform ).GetComponent<BusScript>());
            busses[i].color = busList[i];
        }
        activeBus = busses[0];
    }

    void Update()
    {
        if(isFailed)
        {
            failedScreen.SetActive(true);
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GridScript startGrid = hit.collider.GetComponent<GridScript>();

                if(!startGrid.isAvaible && startGrid.personList.Count == 1) //ulasilabilir ve uzerinde 1 person var ise
                {
                    //ondekilere ulasilabilir bir path
                    foreach(GridScript target in theGrids)
                    {

                        if(target.personList.Count == 0 && AStarPathFinding.instance.GeneratePath(startGrid,target) != null)
                        {                     
                            startGrid.personList[0].GetComponent<PersonScript>().anim.SetInteger("anim",1);
                            startGrid.personList[0].GetComponent<PersonScript>().path = AStarPathFinding.instance.GeneratePath(startGrid,target);
                            startGrid.personList.Clear();
                            break;
                        }
                    }
                }
                else if(startGrid.personList.Count == 1)
                {
                    startGrid.personList[0].GetComponent<PersonScript>().move = true;
                }
            }
        }
        if(!allInPos)
        {
            SetBusPos();
        }
    }

    public void SendBus()
    {
        allInPos = false;
        busIndex++;
        activeBus.isGo = false;
        activeBus.move = true;
        if(busIndex == busList.Count())
        {
            completedScreen.SetActive(true);
            return;
        }
        SetBusPos();
    }

    public void SetBusPos()
    {
        for(int i = 0; i+busIndex < busList.Count(); i++) //yerlerini ayarla
        {
            busses[busIndex+i].transform.position = Vector3.MoveTowards(busses[busIndex+i].transform.position,firstBusPos + i*new Vector3(-6,0,0), personSpeed * Time.deltaTime);
        }

        if(Vector3.Distance(busses[busIndex].transform.position, firstBusPos) <= 0.05f)
        {
            activeBus = busses[busIndex]; //yeni activeBus
            allInPos = true;
            CheckSlots(); //once slotlardan esit varsa onlari otobuse yerlestir
        }
    }

    public void CheckSlots()
    {
        int i = 0;
        foreach(SlotScript slot in slots)
        {
            if(slot.isEmpty) continue;
            if(slot.personList.Count >0 && activeBus.pass < activeBus.maxPass && slot.personList[0].color == activeBus.color)
            {
                activeBus.personList.Add(slot.personList[0]);
                activeBus.pass++;
                activeBus.allInPos = false;
                slot.personList[0].anim.SetTrigger("run");
                slot.personList[0].moveTarget = false;
                slot.personList[0].transform.SetParent(activeBus.transform);
                slot.personList[0].move = false;
                slot.personList[0].bus = activeBus;
                slot.personList[0].MoveToPos(activeBus.GetComponent<BusScript>().seats[activeBus.personList.IndexOf(slot.personList[0])].transform.position + new Vector3(0,0.35f,0));
                if(activeBus.pass == activeBus.maxPass) activeBus.isGo = true;
                i++;
                slot.personList.Clear();
                slot.isEmpty =true;
            }
        }
    }

    public void Restart()
    {
        //slot sifirla
        foreach(SlotScript slot in slots)
        {
            if(slot.personList.Count != 0)
            {
                slot.personList[0].gameObject.SetActive(false);
                Destroy(slot.personList[0].gameObject);
                slot.personList.Clear();
                slot.isEmpty = true;
            }
        }

        //gridleri sifirla
        foreach(GridScript grid in levels[levelIndex].GetComponentsInChildren<GridScript>())
        {
            if(grid.personList.Count == 0) grid.CreatePerson();
        }


        //otobusleri sifirla
        foreach(BusScript bus in busses)
        {
            bus.gameObject.SetActive(false);
            Destroy(bus.gameObject);
        }

        //busList array


        
        busList = levels[levelIndex].GetComponent<LevelScript>().Bus;
        busses.Clear();
        for(int i = 0;i<busList.Count();i++)
        {
            busses.Add(Instantiate(bus,firstBusPos + i*new Vector3(-6,0,0), Quaternion.identity,levels[levelIndex].transform).GetComponent<BusScript>());
            busses[i].color = busList[i];
        }
        activeBus = busses[0];

        isFailed = false;
        failedScreen.SetActive(false);

        
    }

    public void NextLevel()
    {
        completedScreen.SetActive(false);
        levels[levelIndex].SetActive(false);
        levelIndex++;

        levels[levelIndex].SetActive(true);

        busIndex = 0;
        //yeni levele gecince degisenler
        theGrids = levels[levelIndex].GetComponent<LevelScript>().TheGrids;
        slots = levels[levelIndex].GetComponent<LevelScript>().slots;
        busList = levels[levelIndex].GetComponent<LevelScript>().Bus;
        busses.Clear();
        for(int i = 0;i<busList.Count();i++)
        {
            busses.Add(Instantiate(bus,firstBusPos + i*new Vector3(-6,0,0), Quaternion.identity,levels[levelIndex].transform).GetComponent<BusScript>());
            busses[i].color = busList[i];
        }
        activeBus = busses[0];
        
    }

    
}
