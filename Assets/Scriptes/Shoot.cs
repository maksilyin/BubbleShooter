using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    bool pressed = false;
    Vector3 mPos;
    Rigidbody2D rb;
    TargetJoint2D joint;
    Vector2 r = Vector2.zero;
    float force = 0;
    public GameObject Line;
    private LineRenderer trajectory;
    private List<GameObject> spread = new List<GameObject>(2);
    public GameObject bubbleUp;
    public float maxForce = 2;
    public GameObject Shot;
    float spreadAngle = 5;
    bool isMaxForce = false;
    int count = 0;
    bool onFloor = true;
    Vector3[] t_points;
    int coll;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        joint = GetComponent<TargetJoint2D>();
        GameManager.position = transform.position;
        if (trajectory!=null)
        {
            trajectory.transform.position = transform.position;
        }
        Shot = GameObject.Find("Shot");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && onFloor && !GameManager.manager.isWin)
        {
            mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mPos, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject==Shot)
            {
                pressed = true;
                Line = Instantiate(Line, transform.position, Quaternion.identity);
                trajectory = Line.transform.GetChild(0).GetComponent<LineRenderer>();
            }
        }

        if (pressed)
        {
            Vector3 move = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = transform.position - move;
            SetRotation(direction, 0, Line.transform);
            force = Vector2.Distance(transform.position, move);

            r = Line.transform.right;
            if (force >= maxForce)
            {
                isMaxForce = true;
                force = maxForce;
            }
            else isMaxForce = false;
            (t_points, coll) = SetTrajectory(trajectory, transform.position, Line.transform.right * force * 10, 0.1f);
            trajectory.positionCount = t_points.Length;
            trajectory.SetPositions(t_points);
            if (isMaxForce)
            {
                if (spread.Count == 0 || spread[0] == null)
                {
                    spread.Add(Instantiate(Line, transform.position, Quaternion.identity));
                    spread.Add(Instantiate(Line, transform.position, Quaternion.identity));
                }
                else
                {
                    LineSpread(spreadAngle,direction);
                    
                }
                List<Vector2> points = GetPoints(transform.position, Line.transform.right, 0);
                trajectory.positionCount = points.Count+1;
                trajectory.SetPosition(0, transform.position);
                trajectory.SetPosition(1, points[0]);
                if (points.Count>1)
                {
                    float d = 1.5f;
                    trajectory.SetPosition(2, Vector3.MoveTowards(points[0], points[1], d));
                }
            }
            else
            {
                if (spread.Count > 0)
                {
                    for (int i = 0; i < spread.Count; i++)
                    {
                        Destroy(spread[i]);
                    }
                    spread.Clear();
                }
                isMaxForce = false;
            }

            mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mPos, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject != Shot)
            {
                pressed = false;
                Shot_Bubble();
            }

        }

        if (Input.GetMouseButtonUp(0) && pressed )
        {
            pressed = false;
            Shot_Bubble();
        }
        
    }

    private void Shot_Bubble ()
    {
        GameManager.manager.isActive = false;
        if (isMaxForce)
        {
            r = SetRotation(r, Random.Range(-spreadAngle, spreadAngle), Line.transform);
        }
        force = force * 10;
        r = r * force;
        rb.velocity = r;

        Destroy(Line);
        if (spread.Count > 0)
        {
            for (int i = 0; i < spread.Count; i++)
            {
                Destroy(spread[i]);
            }
            spread.Clear();
        }
        Invoke("Air", 0.1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Destroy") && !onFloor)
        {
            Instantiate(GetComponent<BubbleUp>().Particle, transform.position, Quaternion.identity);
            GameManager.manager.Next();
            Destroy(gameObject);

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Wall") || collision.gameObject.tag.Equals("ceiling"))
        {
            r = Vector2.Reflect(r, collision.contacts[0].normal);
            r = r / 2;
            rb.velocity = r;

        }
        if (collision.gameObject.tag.Equals("BubbleUp") && !onFloor)
        {
            if (count == 0)
            {
                tag = "BubbleUp";
                count++;
                joint.enabled = true;
                rb.bodyType = RigidbodyType2D.Dynamic;
                BubbleUp bubbleUp = GetComponent<BubbleUp>();
                if (isMaxForce)
                {
                    Vector3 pos = collision.transform.position;
                    bubbleUp.row = collision.gameObject.GetComponent<BubbleUp>().row;
                    bubbleUp.isChain = collision.gameObject.GetComponent<BubbleUp>().isChain;
                    bubbleUp.StartPos = collision.gameObject.GetComponent<BubbleUp>().StartPos;
                    collision.gameObject.GetComponent<BubbleUp>().Destroy_b();
                    transform.position = pos;
                }
                else
                {
                    bubbleUp.hook = new GameObject[1];
                    bubbleUp.hook[0] = collision.gameObject;
                    bubbleUp.StartPos = Vector2.zero;
                }
                bubbleUp.enabled = true;
                transform.parent = Grid.instance.transform;
                GameManager.manager.objects.Clear();
                GameManager.manager.Check(gameObject);
                if (GameManager.manager.objects.Count >= 3) GameManager.manager.DestroyObjects();
                GameManager.manager.Next();
                Destroy(this);
            }
        }
    }

    private void Air()
    {
        onFloor = false;
    }
    private void LineSpread(float angle, Vector2 dir)
    {
        LineRenderer line1 = spread[0].transform.GetChild(0).GetComponent<LineRenderer>();
        LineRenderer line2 = spread[1].transform.GetChild(0).GetComponent<LineRenderer>();
        Vector2 direction1 = SetRotation(dir, angle, spread[0].transform);
        Vector2 direction2 = SetRotation(dir, -angle, spread[1].transform);
        if (Line.transform.right.x < 0)
            NormalizeLines(line1, line2, direction2, direction1);
  
        else 
            NormalizeLines(line1, line2, direction1, direction2);
    }


    private void NormalizeLines(LineRenderer line1, LineRenderer line2, Vector2 dir1, Vector2 dir2)
    {
        List<Vector2> points1 = GetPoints(transform.position, dir1, 1);
        List<Vector2> points2 = GetPoints(transform.position, dir2, 1);
        float dist = 0;
        if (coll!=-1) dist = Vector2.Distance(t_points[coll], t_points[t_points.Length - 1]) * 0.7f;

        line1.positionCount = 2;
        line2.positionCount = 2;
        line1.SetPosition(0, transform.position);
        if (points2.Count > 1)
        {
            Vector2 crossPoint = cross(transform.position, points1[0], points2[0], points2[1]);
            Vector2 point_1 = Vector2.MoveTowards(crossPoint, points2[1], dist);
            line1.positionCount = 3;
            line1.SetPosition(1, crossPoint);
            line1.SetPosition(2, point_1);
        } else line1.SetPosition(1, points1[0]);

        line2.SetPosition(0, transform.position);

        if (points1.Count > 1)
        {
            Vector2 crossPoint2 = cross(transform.position, points2[0], points1[1], points1[0]);
            Vector2 point_2 = Vector2.MoveTowards(points1[0], points1[1], dist);
            line2.positionCount = 3;
            line2.SetPosition(1, crossPoint2);
            line2.SetPosition(2, point_2);
        } else line2.SetPosition(1, points2[0]);
    }

    public Vector2 SetRotation(Vector2 dir, float angleOffset, Transform t)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(t.position, dir);
        Vector2 target = t.right;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.distance > 0)
            {
                target = hit.point;
            }
        }
                
        Vector3 relative = target;
        relative = relative - t.position;

        float angle = Mathf.Atan2(relative.y, relative.x) * Mathf.Rad2Deg + angleOffset;
        t.rotation = Quaternion.Euler(0f, 0f, angle);
        return t.right;
    }

    private (Vector3[], int) SetTrajectory (LineRenderer line, Vector2 pos, Vector2 dir, float step)
    {
        Vector3[] points = new Vector3[100];
        int count = points.Length;
        int hitPoint = -1;
        points[0] = pos;
        for (int i=1; i<count; i++)
        {
            float time;
            if (dir.sqrMagnitude != 0)
                time = step / dir.magnitude;
            else time = 0;
            dir= dir + Physics2D.gravity * time;
            RaycastHit2D hit = new RaycastHit2D();
            
            RaycastHit2D[] hits = Physics2D.RaycastAll(points[i-1], dir,step);
            foreach (RaycastHit2D hit_f in hits) {
                if (hit_f.distance > 0)
                {
                    hit = hit_f;
                    break;
                }
            }
            if (hit.collider != null)
            {
                points[i] = points[i - 1] + (Vector3)dir.normalized * hit.distance;
                if (hit.collider.tag.Equals("Wall"))
                {
                    dir = dir - Physics2D.gravity * (step - hit.distance) / dir.magnitude;
                    dir = Vector2.Reflect(dir, hit.normal) / 2;
                    count = i + 15;
                    hitPoint = i;
                }
                else if (hit.collider.tag.Equals("BubbleUp") || hit.collider.tag.Equals("ceiling") || hit.collider.tag.Equals("floor"))
                {
                    count = i;
                    break;
                }
            }
            else points[i] = points[i-1] + (Vector3)dir * time; 
        }
        Vector3[] p = new Vector3[count];
        for (int i=0; i<count; i++)
        {
            p[i] = points[i];
        }
        return (p,hitPoint);
    }

    Vector2 cross(Vector2 a, Vector2 b, Vector2 c, Vector2 d) 
    {
        Vector2 T;
        T.x = -((a.x * b.y - b.x * a.y) * (d.x - c.x) - (c.x * d.y - d.x * c.y) * (b.x - a.x)) / ((a.y - b.y) * (d.x - c.x) - (c.y - d.y) * (b.x - a.x));
        T.y = ((c.y - d.y) * (-T.x) - (c.x * d.y - d.x * c.y)) / (d.x - c.x);
        return T;
    }

    private List<Vector2> GetPoints (Vector2 pos, Vector2 direction, int index)
    {
        List<Vector2> points = new List<Vector2>();
        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, direction);
        foreach(RaycastHit2D hit in hits)
        {
            if (hit.distance>0)
            {
                points.Add(hit.point);
                if (hit.collider.tag.Equals("Wall"))
                {
                    if (index < 2)
                    {
                        direction = Vector2.Reflect(direction, hit.normal);
                        
                        points.Add(GetPoints(hit.point, direction, index + 1)[0]);
                    }
                }
                break;
            }
        }
        return points;
    }
}

