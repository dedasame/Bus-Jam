using System.Collections.Generic;
using UnityEngine;

public class PersonScript : MonoBehaviour
{
    public int color;
    public List<GridScript> path;
    public int i;
    public bool move = false;
    public Vector3 newTarget;
    public Animator anim;
    public float index;
    public GridScript grid;
    public bool moveTarget;

    public int busPos;

    public BusScript bus;

    void Start()
    {
        GetComponentInChildren<SkinnedMeshRenderer>().material.color = GameScript.Instance.colors[color];
        
    }
    
    void Update()
    {
        if(path != null  && i < path.Count)
        {
            MoveToTarget();
        }
        if(move)
        {
            MoveBusOrSlot();
        }
        if(!move && moveTarget)
        {
            GoTarget();
        }
        
    }
    public void MoveToPos(Vector3 pos)
    {
        if(moveTarget) return;
        newTarget = pos;
        moveTarget = true;
    }

    public void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, path[i].transform.position, GameScript.Instance.personSpeed * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, path[i].transform.position) <= 0.05f)
        {
            i++; // Sonraki noktaya geç

            // Eğer patika sonuna gelindiyse
            if (i >= path.Count)
            {
                anim.SetTrigger("idle"); //idle
                path = null; // Patika tamamlandı
                move = true;
            }
        }
    }


    public void MoveBusOrSlot()
    {
        if(GameScript.Instance.activeBus.color == color && GameScript.Instance.activeBus.pass != GameScript.Instance.activeBus.maxPass) //otobus ile ayni renkte ise
        {
            anim.SetTrigger("run"); //run
            if(grid)
            {
              grid.personList.Remove(gameObject);  
              grid = null;
            }
            GameScript.Instance.activeBus.personList.Add(this);
            MoveToPos(GameScript.Instance.activeBus.GetComponent<BusScript>().seats[GameScript.Instance.activeBus.personList.IndexOf(this)].transform.position + new Vector3(0,0.35f,0));
            bus = GameScript.Instance.activeBus;

            transform.SetParent(GameScript.Instance.activeBus.transform);

            GameScript.Instance.activeBus.pass++;
            GameScript.Instance.activeBus.allInPos = false;
            if(GameScript.Instance.activeBus.pass == GameScript.Instance.activeBus.maxPass) //otobus dolduysa
            {
                GameScript.Instance.activeBus.isGo = true;
            }
            move = false;
            
            return;
        }

        else
        {
            for (int i = 0; i < GameScript.Instance.slots.Count; i++)
            {
                if(GameScript.Instance.slots[i].isEmpty)
                {
                    anim.SetTrigger("run"); //run
                    if(grid) 
                    {
                        grid.personList.Remove(gameObject);  
                        grid = null;
                    }
                    MoveToPos(GameScript.Instance.slots[i].transform.position);
                    GameScript.Instance.slots[i].GetComponent<SlotScript>().personList.Add(this);
                    GameScript.Instance.slots[i].isEmpty = false;
                    move = false;
                   
                    return;
                }
            }
            //herhangi bos bir slot yok -> 
            Destroy(this.gameObject);
            GameScript.Instance.isFailed = true;
        }
    }
    public void GoTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, (Vector3)newTarget, GameScript.Instance.personSpeed * Time.deltaTime);
        if(newTarget != Vector3.zero) transform.rotation = Quaternion.LookRotation(newTarget-transform.position);
        if(Vector3.Distance(transform.position, (Vector3)newTarget) <= 0.01f)
        {
            moveTarget = false;

            if(bus)
            {
                transform.rotation = Quaternion.LookRotation(GameScript.Instance.destroyPos);
                anim.SetTrigger("sit"); //sit
            }
            else//slota gittiyse
            {
                transform.rotation = Quaternion.LookRotation(GameScript.Instance.activeBus.transform.position);
                anim.SetTrigger("idle");
            }
            
        }
    }
    

}
