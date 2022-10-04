using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int lastRowCount =-1;
    public GameObject Bubble;
    public static GameManager manager;
    public Color[] color = new Color[4];
    public List<GameObject> objects = new List<GameObject>();
    public int Bubbles = 5;
    public GameObject UIcountBubble;
    public Text UIscore;
    public GameObject RestartPanel;
    public GameObject WinPanel;
    public bool isWin = false;
    private int[] typeNext;
    public static Vector2 position;
    public bool isActive = true;
    private int score = 0;
    private void Awake()
    {
        manager = this;
        SetTypeNext();
        RestartUI();
    }
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        if (UIscore != null) UIscore.text = Score.ToString();
    }

    public int Score
    {
        get { return score; }
        set { score = value; }
    }


    public Color GetColor(int index)
    {
        return color[index];
    }

    public void Check(GameObject gm)
    {
        int type = gm.GetComponent<SetColor>().type;
        objects.Add(gm);
        Collider2D[] hits = Physics2D.OverlapCircleAll(gm.transform.position, 0.3f);
        foreach (Collider2D hit in hits)
        {
            if (hit.tag.Equals("BubbleUp") || hit.tag.Equals("Bubble"))
            {
                int typeObject = hit.GetComponent<SetColor>().type;
                if (type == typeObject)
                {
                    bool add = true;
                    for (int j = 0; j < objects.Count; j++)
                    {
                        if (objects[j] == hit.gameObject)
                        {
                            add = false;
                            break;
                        }
                    }
                    if (add)
                    {
                        Check(hit.gameObject);
                    }
                }
            }
        }
    }

    public void DestroyObjects ()
    {
        int addScore = 0;
        for (int i = 0; i < objects.Count; i++)
        {
            addScore += i * 50;
            objects[i].GetComponent<BubbleUp>().Destroy_b();
        }
        objects.Clear();
       Score += addScore;
    }

    public void RestartUI ()
    {
        if (UIcountBubble != null)
        {
            UIcountBubble.GetComponent<Text>().text = Bubbles.ToString();
            if (Bubbles>0)
            {
                Image image = UIcountBubble.transform.parent.GetComponent<Image>();
                image.color = color[typeNext[Bubbles - 1]];
            }
        }
    }

    private void SetTypeNext()
    {
        typeNext = new int[Bubbles];
        for (int i=0; i<Bubbles; i++)
        {
            typeNext[i] = Random.Range(0, 4);
        }
    }


    public bool CheckWin()
    {
        float p_win = 30;
        int count = 0;
        GameObject[] gms = Grid.instance.GetGrid[0];
        foreach (GameObject gm in gms)
        {
            if (gm != null) count++;
        }
        if ((float)gms.Length * p_win / 100 >= count) return true;
        return false;
    }

    public void NextInvoke()
    {
        if (!CheckWin())
        {
            if (Bubbles > 0)
            {
                isActive = true;
                GameObject newBubble = Instantiate(Bubble, position, Quaternion.identity);
                newBubble.GetComponent<SetColor>().type = typeNext[Bubbles - 1];
                Bubbles--;
                RestartUI();
            }
            else
            {
                if (RestartPanel!=null)
                    Instantiate(RestartPanel, GameObject.Find("UICanvas").transform);
            }
        }
        else
        {
            Grid.instance.AllDrop();
            Invoke("Win", 2);
        }
    }

    public void Win ()
    {
        if (WinPanel!=null)
            Instantiate(WinPanel, GameObject.Find("UICanvas").transform);
    }

    public void Next()
    {
        if (!isActive)
            Invoke("NextInvoke", 0.5f);
    }
}
