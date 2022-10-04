using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColor : MonoBehaviour
{
    public int type=-1;
    // Start is called before the first frame update

    private void Awake()
    {
        
    }
    void Start()
    {
        if (type == -1)
        {
            type = Random.Range(0, 4);
            GetComponent<SpriteRenderer>().color = GameManager.manager.GetColor(type);
        }
        else setColor(type);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setColor (int newType)
    {
        type = newType;
        GetComponent<SpriteRenderer>().color = GameManager.manager.GetColor(newType);
    }
}
