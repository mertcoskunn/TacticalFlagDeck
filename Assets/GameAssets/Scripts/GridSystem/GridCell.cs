using UnityEngine;



public enum ColorCode
{
    NONE ,
    AVALIABLE ,
    SELECTED, 
    MOVEABLE,
    ENEMY, 
    EMPTY,
    SPAWNABLE_AVALIABLE,
    SPAWNABLE_NOT_AVALIABLE
}


public class GridCell : MonoBehaviour
{

    public Vector2 gridIndex;
    public bool cellFull = false;
    public GameObject objectInCell; 
    public GameObject higlightSquare;
    
    
    

    public void SetHighlight(bool val, ColorCode color = ColorCode.NONE){
            higlightSquare.GetComponent<SpriteRenderer>().color = GetColor(color);
            higlightSquare.SetActive(val);
    }

    public void SetObjectToCell(GameObject newObj)
    {
        objectInCell  =newObj;
        cellFull = true; 
    }
    public void RemoveObjectFromCell()
    {
        objectInCell = null;
        cellFull = false; 
    }

    private Color GetColor(ColorCode color)
    {
        switch (color)
        {
                case ColorCode.NONE:
                        return new Color(0.0f, 0.0f, 0.0f, 0f);
                case ColorCode.AVALIABLE:
                        return new Color(0.5f, 0.5f, 0.5f, 1f);
                case ColorCode.SELECTED:
                        return new Color(0.0f, 0.1f, 0.1f, 1f);
                case ColorCode.MOVEABLE:
                        return new Color(0.2f, 1.0f, 0.2f, 1f);
                case ColorCode.ENEMY:
                        return new Color(0.7f, 0.2f, 0.2f, 1f);
                case ColorCode.EMPTY:
                        return new Color(0.0f, 0.0f, 0.0f, 1f);
                case ColorCode.SPAWNABLE_AVALIABLE:
                        return new Color(0.2f, 1f, 0.2f, 0.5f);
                case ColorCode.SPAWNABLE_NOT_AVALIABLE:
                        return new Color(1f, 0.2f, 0.2f, 0.5f);

        }
        return new Color(0.0f, 0.0f, 0.0f, 0f);
    }

}

   