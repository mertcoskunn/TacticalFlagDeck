using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardGameProject;

public interface IGridManager
{
    event Action<global::System.Int32, global::System.Int32, Vector2> OnAddObjectToGrid;
    event Action<Vector2, Vector2> OnMakeMove;

    global::System.Boolean AddObjectToGrid(global::System.Int32 cardIndex, Vector2 gridPosition, int team);
    void CreateGrid();
    GridCell GetSelectedCell();
    void Kill(GridCell targetCell);
    void MoveObject(Vector2 initPos, Vector2 targetPos);
    void SetLocked(global::System.Boolean val);
}

public class GridManager : MonoBehaviour, IGridManager
{
    public int width = 0;
    public int height = 0;
    public GameObject gridCellPrefab;
    public List<GameObject> gridObjects = new List<GameObject>();
    public GameObject[,] gridCells;
    public GameObject selfObject;
    public CardDataBase cardDB;
    public GameObject flagPrefab;

    private GameObject flag; 


    private GridCell hoveredCell;
    private GridCell selectedCell;

    private int currentState = 0;
    private bool gridSelected = false;
    private bool mouseClicked = false;
    private bool tempMouseClicked = false;

    private List<GridCell> moveableGrids = new List<GridCell>();
    private List<GridCell> enemyGrids = new List<GridCell>();
    private List<GridCell> higlightedGrids = new List<GridCell>(); 
    
    private int spawnRange = 2; 
    
    private bool isAnimRunning = false;
    private bool isLocked = true;

    public event Action<int, int, Vector2> OnAddObjectToGrid;
    public event Action<Vector2, Vector2> OnMakeMove;
    public event Action<Vector2, Vector2> OnAttack;
    public event Action<int> OnMatchEnd;


    private bool matchEnd = false; 
    
    void Start()
    {
        //Init(0);
        //GameStart(0);
        RegisterToEvent();
    }


    void Update()
    {
        if(isAnimRunning)
        {
            return; 
        }
        
        if(matchEnd)
        {
            matchEnd = false; 
            SetLocked(true); 
            OnMatchEnd?.Invoke(GameManager.Instance.CurrentTeam);
            return;  
            
        }
        if (isLocked)
        {
            
            return;
        }
    
        mouseClicked = !tempMouseClicked && Input.GetMouseButton(0);
        tempMouseClicked = Input.GetMouseButton(0);
        switch (currentState)
        {
            case 0:
                {
                    if (mouseClicked)
                    {
                        if ((hoveredCell != null) && (hoveredCell.objectInCell != null) && (hoveredCell.objectInCell.GetComponent<GameCharacter>() != null))
                        {
                                if(hoveredCell.objectInCell.GetComponent<GameCharacter>().GetTeam() == GameManager.Instance.CurrentTeam)
                                {
                                    selectedCell = hoveredCell;
                                    DeactivateHoveredCell();
                                    currentState = 1;
                                }   
                        }
                    }
                    else
                    {
                        GridCell cell = GetSelectedCell();
                        if (cell != null && cell.objectInCell != null && cell.objectInCell.GetComponent<GameCharacter>()!=null)
                        {
                            ActivateHoveredCell(cell);
                           
                        }
                        else
                        {
                            DeactivateHoveredCell();
                        }
                    }

                    break;
                }

            case 1:
                {
                    if (!gridSelected)
                    {
                        selectedCell.SetHighlight(true, ColorCode.SELECTED);
                        ActivateMoveableGrids();
                        ActivateAttackableGrids();
                        gridSelected = true;
                        currentState = 2;
                    }
                    else
                    {
                        if (mouseClicked)
                        {
                            SetState0();
                        }

                    }
                    break;
                }

            case 2:
                {
                    if (moveableGrids.Count > 0)
                    {
                        if (mouseClicked)
                        {
                            GridCell cell = GetSelectedCell();
                            if (cell != null)
                            {

                                bool clickedEnemyObject = enemyGrids.Contains(cell);
                                bool clickedMovObject = moveableGrids.Contains(cell);

                                if (clickedEnemyObject)
                                {
                                    int enemyDist = (int)Vector2.Distance(cell.gridIndex, selectedCell.gridIndex);
                                    if (enemyDist <= selectedCell.objectInCell.GetComponent<GameCharacter>().GetAttackRange())
                                    {
                                        isAnimRunning = true; 
                                        Attack(selectedCell.gridIndex, cell.gridIndex);
                                        OnAttack?.Invoke(selectedCell.gridIndex, cell.gridIndex);
                                        
                                        
                                    }
                                    SetState0();
                                    break;

                                }

                                if (clickedMovObject)
                                {
                                    isAnimRunning = true; 
                                    MoveObject(selectedCell.gridIndex ,cell.gridIndex);
                                    OnMakeMove?.Invoke(selectedCell.gridIndex ,cell.gridIndex);
                                   
                                    SetState0();
                                    break;
                                }

                                else
                                {
                                    SetState0();
                                }
                            }
                            else
                            {
                                SetState0();
                            }

                        }

                    }
                    else
                    {
                        SetState0();
                    }
                    break;
                }

        }
    }
    
    public void CreateGrid()
    {
        gridCells = new GameObject[width, height];
        Vector2 centerOffset = new Vector2(width / 2.0f - 0.5f, height / 2.0f - 0.5f);

        centerOffset.y -= selfObject.transform.position.y;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 gridPosition = new Vector2(x, y);
                Vector2 spawnPosition = gridPosition - centerOffset;
                GameObject gridCell = Instantiate(gridCellPrefab, spawnPosition, Quaternion.identity);

                gridCell.transform.SetParent(transform);
                if (GameManager.Instance.CurrentTeam == 0)
                {
                    gridCell.GetComponent<GridCell>().gridIndex = new Vector2(x, y);
                    gridCells[x, y] = gridCell;
                }
                else if (GameManager.Instance.CurrentTeam == 1)
                {
                    gridCell.GetComponent<GridCell>().gridIndex = new Vector2(width - x - 1, y);
                    gridCells[width - x - 1, y] = gridCell;
                }
                else
                {
                    Debug.Log("Cant Found Current Team");
                }

            }
        }
    }
    public bool AddObjectToGrid(int cardIndex, Vector2 gridPosition, int team)
    {
        CardCreature cardData = cardDB.GetCardByIndex(cardIndex) as CardCreature;

        if (gridPosition.x >= 0 && gridPosition.x < width && gridPosition.y < height)
        {
            GridCell cell = gridCells[(int)gridPosition.x, (int)gridPosition.y].GetComponent<GridCell>();
            if (cell.cellFull) return false;
            else
            {
                GameObject newObj = Instantiate(cardData.prefab, cell.GetComponent<Transform>().position, Quaternion.identity);
                newObj.GetComponent<GameCharacter>().InitChar(cardData.health, cardData.attack, cardData.movementRange, cardData.attackRange, cardData.charSprite, cardData.charAnimator, team, cell);
                newObj.transform.SetParent(transform);
                gridObjects.Add(newObj);
                cell.SetObjectToCell(newObj);
                if(team == GameManager.Instance.CurrentTeam)
                    OnAddObjectToGrid?.Invoke(GameManager.Instance.CurrentTeam, cardData.GetCardIndex(), gridPosition);
                return true;
            }
        }
        else return false;

    }
    public void SetLocked(bool val)
    {
        isLocked = val;
        DeactivateHoveredCell();
    }
    public GridCell GetSelectedCell()
    {


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit.collider != null && hit.collider.GetComponent<GridCell>())
        {
            return hit.collider.GetComponent<GridCell>();

        }
        return null;
    }

    public void Attack(Vector2 initPos, Vector2 targetPos)
    {
        GridCell initCell = gridCells[(int)initPos.x, (int)initPos.y].GetComponent<GridCell>();
        GridCell targetCell = gridCells[(int)targetPos.x, (int)targetPos.y].GetComponent<GridCell>();
        
        initCell.objectInCell.GetComponent<GameCharacter>().Attack(targetCell.objectInCell.GetComponent<GameCharacter>());
        
        if (targetCell.objectInCell.GetComponent<GameCharacter>().GetHealth() <= 0)
        {

            Kill(targetCell);

        }
    }
    public void MoveObject(Vector2 initPos, Vector2 targetPos)
    {
        GridCell initCell = gridCells[(int)initPos.x, (int)initPos.y].GetComponent<GridCell>();
        GridCell targetCell = gridCells[(int)targetPos.x, (int)targetPos.y].GetComponent<GridCell>();

        if(targetCell.objectInCell == flag)
        {
            flag.SetActive(false);
            initCell.objectInCell.GetComponent<GameCharacter>().SetIsCarryingFlag(true);
        }
        else
        {
            int targetX; 
            if(GameManager.Instance.CurrentTeam == 0)
            {
                targetX = (int)(width-targetCell.gridIndex.x-1);

            }
            else
            {
                targetX = (int)(targetCell.gridIndex.x); 

            }

            if(initCell.objectInCell.GetComponent<GameCharacter>().GetIsCarryingFlag() && targetX<=0)
            {
                matchEnd = true; 
            }
        }
        initCell.objectInCell.GetComponent<GameCharacter>().Walk(targetCell);
        targetCell.SetObjectToCell(initCell.objectInCell);
        initCell.RemoveObjectFromCell();
    }
    public void Kill(GridCell targetCell)
    {
        if (targetCell != null)
        {
            if(targetCell.objectInCell.GetComponent<GameCharacter>().GetIsCarryingFlag())
            {
                flag.transform.position = targetCell.GetComponent<Transform>().position; 
                flag.SetActive(true);
                targetCell.objectInCell.GetComponent<GameCharacter>().SetIsCarryingFlag(false); 
                targetCell.objectInCell.GetComponent<GameCharacter>().Dead();
                targetCell.RemoveObjectFromCell();
                targetCell.SetObjectToCell(flag); 

            }
            else
            {
                targetCell.objectInCell.GetComponent<GameCharacter>().Dead();
                targetCell.RemoveObjectFromCell();
            }
   
        }
        else
        {
            Debug.Log("Cell empty");
        }
    }

    private void ActivateMoveableGrids(){
        
        bool blockedPosXDirection = false;
        bool blockedNegXDirection = false;
        bool blockedPosYDirection = false;
        bool blockedNegYDirection = false; 

        for(int r=1; r<=selectedCell.objectInCell.GetComponent<GameCharacter>().GetMovRange(); r++)
        {
            int tempX  = (int)selectedCell.gridIndex.x + r;
                        
            if(tempX<width)
            {
                if(!gridCells[tempX, (int)selectedCell.gridIndex.y].GetComponent<GridCell>().cellFull){
                
                    if(!blockedPosXDirection){
                    gridCells[tempX, (int)selectedCell.gridIndex.y].GetComponent<GridCell>().SetHighlight(true, ColorCode.MOVEABLE);
                    moveableGrids.Add(gridCells[tempX, (int)selectedCell.gridIndex.y].GetComponent<GridCell>());
                    higlightedGrids.Add(gridCells[tempX, (int)selectedCell.gridIndex.y].GetComponent<GridCell>());
                    } 
                }
                else
                {
                    if(gridCells[tempX, (int)selectedCell.gridIndex.y].GetComponent<GridCell>().objectInCell.GetComponent<Flag>() != null)
                    {
                       gridCells[tempX, (int)selectedCell.gridIndex.y].GetComponent<GridCell>().SetHighlight(true, ColorCode.MOVEABLE);
                       moveableGrids.Add(gridCells[tempX, (int)selectedCell.gridIndex.y].GetComponent<GridCell>());
                       higlightedGrids.Add(gridCells[tempX, (int)selectedCell.gridIndex.y].GetComponent<GridCell>()); 
                    }
                    blockedPosXDirection = true; 
                }        
            }

            tempX  = (int)selectedCell.gridIndex.x - r;

            if(tempX>=0)
            {
                if(!gridCells[tempX, (int)selectedCell.gridIndex.y].GetComponent<GridCell>().cellFull)
                {
                    if(!blockedNegXDirection){
                        gridCells[tempX, (int)selectedCell.gridIndex.y].GetComponent<GridCell>().SetHighlight(true, ColorCode.MOVEABLE);
                        moveableGrids.Add(gridCells[tempX, (int)selectedCell.gridIndex.y].GetComponent<GridCell>());
                        higlightedGrids.Add(gridCells[tempX, (int)selectedCell.gridIndex.y].GetComponent<GridCell>());
                    }
                
                 }
                 else
                 {
                    if(gridCells[tempX, (int)selectedCell.gridIndex.y].GetComponent<GridCell>().objectInCell.GetComponent<Flag>() != null)
                    {
                       gridCells[tempX, (int)selectedCell.gridIndex.y].GetComponent<GridCell>().SetHighlight(true, ColorCode.MOVEABLE);
                       moveableGrids.Add(gridCells[tempX, (int)selectedCell.gridIndex.y].GetComponent<GridCell>());
                       higlightedGrids.Add(gridCells[tempX, (int)selectedCell.gridIndex.y].GetComponent<GridCell>()); 
                    }

                    blockedNegXDirection = true; 
                   

                 }

            }

            int tempY = (int)selectedCell.gridIndex.y + r;


            if(tempY < height)
            {
                if(!gridCells[(int)selectedCell.gridIndex.x, tempY].GetComponent<GridCell>().cellFull ){
                if(!blockedPosYDirection){
                gridCells[(int)selectedCell.gridIndex.x, tempY].GetComponent<GridCell>().SetHighlight(true, ColorCode.MOVEABLE);
                moveableGrids.Add(gridCells[(int)selectedCell.gridIndex.x, tempY].GetComponent<GridCell>());
                higlightedGrids.Add(gridCells[(int)selectedCell.gridIndex.x, tempY].GetComponent<GridCell>());
                }
                }
                else
                {
                if(gridCells[(int)selectedCell.gridIndex.x, tempY].GetComponent<GridCell>().objectInCell.GetComponent<Flag>() != null)
                {
                    gridCells[(int)selectedCell.gridIndex.x, tempY].GetComponent<GridCell>().SetHighlight(true, ColorCode.MOVEABLE);
                    moveableGrids.Add(gridCells[(int)selectedCell.gridIndex.x, tempY].GetComponent<GridCell>());
                    higlightedGrids.Add(gridCells[(int)selectedCell.gridIndex.x, tempY].GetComponent<GridCell>()); 
                }
                blockedPosYDirection = true;
               
                }
            }

            tempY = (int)selectedCell.gridIndex.y - r;

            if(tempY >= 0)
            {
                if(!gridCells[(int)selectedCell.gridIndex.x, tempY].GetComponent<GridCell>().cellFull){
                    if(!blockedNegYDirection){
                    gridCells[(int)selectedCell.gridIndex.x, tempY].GetComponent<GridCell>().SetHighlight(true, ColorCode.MOVEABLE);
                    moveableGrids.Add(gridCells[(int)selectedCell.gridIndex.x, tempY].GetComponent<GridCell>());
                    higlightedGrids.Add(gridCells[(int)selectedCell.gridIndex.x, tempY].GetComponent<GridCell>());
                    }
                }
                else
                {
                
                if(gridCells[(int)selectedCell.gridIndex.x, tempY].GetComponent<GridCell>().objectInCell.GetComponent<Flag>() != null)
                {
                    gridCells[(int)selectedCell.gridIndex.x, tempY].GetComponent<GridCell>().SetHighlight(true, ColorCode.MOVEABLE);
                    moveableGrids.Add(gridCells[(int)selectedCell.gridIndex.x, tempY].GetComponent<GridCell>());
                    higlightedGrids.Add(gridCells[(int)selectedCell.gridIndex.x, tempY].GetComponent<GridCell>()); 
                }
                blockedNegYDirection = true; 
                
                }
            }
        } 
    }
    
    private void ActivateAttackableGrids(){
        
        

        for(int r=1; r<=selectedCell.objectInCell.GetComponent<GameCharacter>().GetAttackRange(); r++)
        {
            int tempX  = (int)selectedCell.gridIndex.x + r;
                        
            if(tempX<width)
            {
                GridCell cell = gridCells[tempX, (int)selectedCell.gridIndex.y].GetComponent<GridCell>();
                if(cell.cellFull && cell.objectInCell.GetComponent<GameCharacter>() != null){
                    if(cell.objectInCell.GetComponent<GameCharacter>().GetTeam() != GameManager.Instance.CurrentTeam)
                    {
                    cell.SetHighlight(true, ColorCode.ENEMY);
                    enemyGrids.Add(cell);
                    higlightedGrids.Add(cell); 
                    }
                }       
            }

            tempX  = (int)selectedCell.gridIndex.x - r;

            if(tempX>=0)
            {
                GridCell cell = gridCells[tempX, (int)selectedCell.gridIndex.y].GetComponent<GridCell>();
                if(cell.cellFull && cell.objectInCell.GetComponent<GameCharacter>() != null)
                {
                    if(cell.objectInCell.GetComponent<GameCharacter>().GetTeam() != GameManager.Instance.CurrentTeam)
                    {
                        cell.SetHighlight(true, ColorCode.ENEMY);
                        enemyGrids.Add(cell);
                        higlightedGrids.Add(cell); 
                    }
                 }
            }

            int tempY = (int)selectedCell.gridIndex.y + r;


            if(tempY < height)
            {
                GridCell cell =  gridCells[(int)selectedCell.gridIndex.x, tempY].GetComponent<GridCell>();
                if(cell.cellFull && cell.objectInCell.GetComponent<GameCharacter>() != null){
                    if(cell.objectInCell.GetComponent<GameCharacter>().GetTeam() != GameManager.Instance.CurrentTeam)
                    {
                    cell.SetHighlight(true, ColorCode.ENEMY);
                    enemyGrids.Add(cell);
                    higlightedGrids.Add(cell); 
                }
                }
                
            }

            tempY = (int)selectedCell.gridIndex.y - r;

            if(tempY >= 0)
            {
                GridCell cell = gridCells[(int)selectedCell.gridIndex.x, tempY].GetComponent<GridCell>();
                if(cell.cellFull && cell.objectInCell.GetComponent<GameCharacter>() != null){
                 if(cell.objectInCell.GetComponent<GameCharacter>().GetTeam() != GameManager.Instance.CurrentTeam)
                {
                    cell.SetHighlight(true, ColorCode.ENEMY);
                    enemyGrids.Add(cell);
                    higlightedGrids.Add(cell); 
                }
                }
            }
        } 
    }

    public void HighlightSpawnArea()
    {
        if(GameManager.Instance.CurrentTeam == 0)
        {
            for(int w = 0; w<spawnRange;w++)
            {
                for(int h = 0; h < height; h++)
                {
                    GridCell cell = gridCells[w,h].GetComponent<GridCell>(); 
                    if(cell.cellFull)
                    {
                        cell.SetHighlight(true, ColorCode.SPAWNABLE_NOT_AVALIABLE);
                    }
                    else
                    {
                        cell.SetHighlight(true, ColorCode.SPAWNABLE_AVALIABLE);
                    }
                    higlightedGrids.Add(cell); 
                }
            }
        }
        else
        {
            for(int w = 0; w<spawnRange;w++)
            {
                for(int h = 0; h < height; h++)
                {
                    GridCell cell = gridCells[width-w-1,h].GetComponent<GridCell>();
                    if(cell.cellFull)
                    {
                        cell.SetHighlight(true, ColorCode.SPAWNABLE_NOT_AVALIABLE);
                    }
                    else
                    {
                        cell.SetHighlight(true, ColorCode.SPAWNABLE_AVALIABLE);
                    }
                    higlightedGrids.Add(cell);
                }
            }

        }
    }

    private void ActivateHoveredCell(GridCell cell)
    {
        if(hoveredCell != null)
        {
            if(hoveredCell.objectInCell != null )
             {
               
                if(hoveredCell.objectInCell.GetComponent<GameCharacter>().GetTeam() == GameManager.Instance.CurrentTeam)
                {
                    hoveredCell.SetHighlight(false);
                    hoveredCell = cell; 
                    hoveredCell.SetHighlight(true, ColorCode.AVALIABLE); 
                }
                else
                {
                    hoveredCell.SetHighlight(false);
                    hoveredCell = cell; 
                    hoveredCell.SetHighlight(true, ColorCode.ENEMY);
                } 
                
            }
            else
            {
                
                    hoveredCell.SetHighlight(false);
                    hoveredCell = cell; 
                    hoveredCell.SetHighlight(true, ColorCode.AVALIABLE);
                
            }
                   

        }
        else
        {
           hoveredCell = cell; 
           hoveredCell.SetHighlight(true, ColorCode.AVALIABLE);
        }
    }
    private void DeactivateHoveredCell()
    {
        if(hoveredCell != null){
           hoveredCell.SetHighlight(false);
           hoveredCell = null; 
        }
    }
    
    public void SetState0()
    {
        DeactivateHighlightedGrids();
        if (selectedCell != null)
        {
            selectedCell.SetHighlight(false);
        }
        selectedCell = null;
        gridSelected = false;
        currentState = 0;
    }
    
    public void  DeactivateHighlightedGrids()
    {

        if (higlightedGrids.Count != 0)
        {
            for (int index = 0; index < higlightedGrids.Count; index++)
            {
                higlightedGrids[index].SetHighlight(false);
            }
        }
        higlightedGrids.Clear();
        moveableGrids.Clear();
        enemyGrids.Clear(); 
    }

    private void RegisterToEvent()
    {
        GameCharacter.OnAnimEnd += CharAnimationEnd;
    }
    private void UnregisterToEvent()
    {
        GameCharacter.OnAnimEnd -= CharAnimationEnd;
    }

    public void Init(int currentTeam)
    {   
        CreateGrid();
        CreateFlag(); 

    }


    public void Reset()
    {
        for (int i = 0; i < gridCells.GetLength(0); i++)
        {
            for (int j = 0; j < gridCells.GetLength(1); j++)
            {
                if (gridCells[i, j] != null)
                {
                    if(gridCells[i, j].GetComponent<GridCell>().objectInCell != null)
                    {
                        Destroy(gridCells[i, j].GetComponent<GridCell>().objectInCell);
                        gridCells[i, j].GetComponent<GridCell>().RemoveObjectFromCell();
                    }
                     
                    Destroy(gridCells[i, j]);
                    gridCells[i, j] = null;
                }
            }
        }

        if(flag != null)
        {
            Destroy(flag); 
        }
        enemyGrids.Clear();
        moveableGrids.Clear(); 
        higlightedGrids.Clear();
    }

    public void SetCurrentState(int val)
    {
        currentState = val; 
    }
  
    public void CreateFlag()
    {
       int midX  = (width + 1) / 2;
       int midY =  (height + 1) / 2;
       
       
       GridCell cell = gridCells[midX-1, midY-1].GetComponent<GridCell>();  
       
       GameObject newObj = Instantiate(flagPrefab, cell.GetComponent<Transform>().position, Quaternion.identity); 
       newObj.transform.SetParent(transform);

       gridObjects.Add(newObj);
       cell.SetObjectToCell(newObj);

       flag = newObj; 
    }

    void CharAnimationEnd()
    {
        isAnimRunning = false; 
    }

}
