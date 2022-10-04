using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleUp : MonoBehaviour
{
    public GameObject Particle;
    public GameObject[] hook;
    public bool noHook = false;
    private bool drop = false;
    public int type;
    public Vector2 startPos;
    public int row = -1;
    public bool isChain=false;
    public bool upChain;
    public bool IsDrop
    {
        get { return drop; }
        set { drop = value; }
    }

    public Vector2 StartPos
    {
        get { return startPos; }
        set { startPos = value; }
    }

    void Awake()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsDrop)
        {
            if (hook.Length>0) 
            {
                Check();
            }
            else CheckChain();
            if (StartPos != Vector2.zero)
            {
                float x = 0;
                float y = 0;
                x = Mathf.Lerp(transform.position.x, startPos.x, Time.deltaTime * 3);
                y = Mathf.Lerp(transform.position.y, startPos.y, Time.deltaTime * 3);
                transform.position = new Vector2(x, y);
            }
        }
        else
        {
            GetComponent<TargetJoint2D>().enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Destroy") && GetComponent<BubbleUp>().enabled)
        {
            Destroy_b();
        }
    }

    public void Destroy_b ()
    {
        Instantiate(Particle, transform.position, Quaternion.identity);
        GameManager.manager.Score += 100;
        GameManager.manager.CheckWin();
        Destroy(gameObject);
    }

    public void Check()
    {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);
            bool n = true;
            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject != gameObject)
                {
                    if (noHook) n = false;
                    for (int j = 0; j < hook.Length; j++)
                    {
                        if (hook[j] == hit.gameObject && !hit.GetComponent<BubbleUp>().IsDrop)
                        {
                            n = false;
                            break;
                        }
                    }
                }
                if (!n) break;
            }
            IsDrop = n;
    }

    private void CheckChain()
    {
        isChain = false;
        if (row > 0)
        {
            upChain = CheckUpChein();

            if (upChain) isChain = true;

            else isChain = CheckRow(row);

            if (!isChain)
            {
               IsDrop = true;
            }
        }
        else if(row!=-1) isChain = true;
    }


    private bool CheckRow(int numRow)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject != gameObject)
            {
                BubbleUp bubbleUp = hit.GetComponent<BubbleUp>();
                if (bubbleUp.row == numRow)
                {
                    if (bubbleUp.upChain) return true;
                    else return bubbleUp.CheckRow(numRow,hit.gameObject,gameObject);
                }
            }
        }
        return false;
    }

    private bool CheckRow(int numRow, GameObject gm, GameObject ignor)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.5f);
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject !=gm && hit.gameObject!=ignor)
            {
                BubbleUp bubbleUp = hit.GetComponent<BubbleUp>();
                if (bubbleUp.row == numRow)
                {
                    if (bubbleUp.upChain) return true;
                    else return bubbleUp.CheckRow(numRow, hit.gameObject, gm);
                }
            }
        }
        return false;
    }

    public bool CheckUpChein()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.3f);
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject != gameObject)
            {
                BubbleUp bubbleUp = hit.GetComponent<BubbleUp>();
                if (bubbleUp.row != row - 1) continue;
                else
                {
                    if (bubbleUp.isChain || bubbleUp.noHook) return true;
                }
            }
        }
        return false;
    }
}
