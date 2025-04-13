using System.Collections.Generic; 
using UnityEngine;

public class ArcRenderer : MonoBehaviour
{
   public GameObject arrowPrefab;
   public GameObject dotPrefab; 
   public int poolSize = 50;
   private List<GameObject> dotPool = new List<GameObject>();
   private GameObject arrowInstance; 

    public float spacing = 40;
    public float arrowAngleAdjustment = 0; 
    public int dotsToSkip = 2;
    private Vector3 arrowDirection; 

    void Start()
    {
        arrowInstance = Instantiate(arrowPrefab, transform);
        arrowInstance.transform.localPosition = Vector3.zero; 
        InitializeDotPool(poolSize);

        
    }
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 0;

        Vector3 startPos = transform.position;
        Vector3 midPoint = CalculateMidPoint(startPos, mousePos);
        UpdateArc(startPos, midPoint, mousePos);
        PositionAndRotateArrow(mousePos);
        
    }

    void InitializeDotPool(int count){
        for(int i= 0; i<count; i++){
            GameObject dot = Instantiate(dotPrefab, Vector3.zero, Quaternion.identity, transform);
            dot.SetActive(false);
            dotPool.Add(dot); 
        }
    }

    Vector3 CalculateMidPoint(Vector3 start, Vector3 end)
    {   
        Vector3 midPoint = (start + end)/2;
        float arcHeight = Vector3.Distance(start, end)/3f; 
        midPoint.y += arcHeight;
        return midPoint;
    }


    Vector3 QuadraticBezierPoint(Vector3 start, Vector3 control, Vector3 end, float t){
        float u = 1-t;
        float tt = t*t;
        float uu = u*u;

        Vector3 point = uu*start;
        point += 2 * u * t * control;
        point += tt * end; 
        return point; 
    }

    void UpdateArc(Vector3 start, Vector3 mid, Vector3 end){
        int numDots = Mathf.CeilToInt(Vector3.Distance(start, end)/spacing);

        for(int i = 0; i<numDots && i<dotPool.Count; i++)
        {
            float t = i/(float)numDots;
            t = Mathf.Clamp(t, 0f, 1f);
            Vector3 position = QuadraticBezierPoint(start, mid, end, t);

            if(i != numDots - dotsToSkip){
                dotPool[i].transform.position = position;
                dotPool[i].SetActive(true);

            }
            if(i== numDots -(dotsToSkip + 1) && i-dotsToSkip + 1 >=0){
                arrowDirection = dotPool[i].transform.position;

            }

        }

        for(int i=numDots-dotsToSkip; i<dotPool.Count; i++){
            if(i>0){
                dotPool[i].SetActive(false); 
            }
        }
    }


    void PositionAndRotateArrow(Vector3 position){
        arrowInstance.transform.position = position;
        Vector3 direction = arrowDirection-position;
        direction = -1* direction;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; 
        angle += arrowAngleAdjustment;
        arrowInstance.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);     
    }


}
