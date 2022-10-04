using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public GameObject block;
    public float startPosX;
    public float startPosY;
    public int xMax = 10;
    public int yMax = 10;
    public float outX;
    public float outY;
    public string objName = "Bubble_";
    private List<GameObject[]> grid;
    private GameObject lastX;
    public static Grid instance;
    void Start()
    {
        instance = this;
        float startX = startPosX;
        float startY = startPosY;
        List<int[]> rows = XML.GetRows(1);
        grid = new List<GameObject[]>();



        for (int y = 0; y < rows.Count; y++)
        {
            GameObject[] gms = new GameObject[rows[y][1]];
            startPosX += outX * rows[y][0];
            for (int x = 0; x < rows[y][1]; x++)
            {
                GameObject gm = Instantiate(block, new Vector2(startPosX, startPosY), Quaternion.identity);
                gm.transform.SetParent(transform);
                gm.name = objName + y + x;
                BubbleUp bubbleUp = gm.GetComponent<BubbleUp>();
                bubbleUp.row = y;
                gms[x] = gm;
                if (y == 0) bubbleUp.noHook = true;
                startPosX += outX;
            }
            grid.Add(gms);
            startPosX = startX;
            if (y % 2 == 0) startPosX += outX / 2;
            startPosY -= outY;
        }
        startPosY = startY;
    }

    public List<GameObject[]> GetGrid
    {
        get { return grid; }
    }

    public void AllDrop()
    {
        for (int i=0; i<transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<BubbleUp>().IsDrop = true;
        }
    }
}
