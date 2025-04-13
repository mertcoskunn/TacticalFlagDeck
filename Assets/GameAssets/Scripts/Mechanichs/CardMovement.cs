
using UnityEngine;
using UnityEngine.EventSystems; 

public class CardInteraction : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    
    [SerializeField] private GameObject selfObject; 
    [SerializeField] private float selectScale = 1.1f; 
    [SerializeField] private Vector2 cardPlay; 
    [SerializeField] private Vector3 playPosition; 
    [SerializeField] private GameObject glowEffect;
    [SerializeField] private GameObject playArrow; 
    [SerializeField] private float lerpFactor = 1.0f;
    

    private RectTransform rectTransform;
    private Canvas canvas; 
    private Vector2 originalLocalPointerPosition;
    private Vector3 originalPanelLocalPosition; 
    private Vector3 originalScale; 
    private int currentState = 0; 
    private Quaternion orginalRotation;
    private Vector3 orginalPosition; 
    private GridManager gridManager;
    private int siblingIndex=0;
    private bool isLocked = false; 

    private int spawnRange = 2;  


    void Awake(){
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalScale = rectTransform.localScale;
        orginalPosition = rectTransform.localPosition;
        orginalRotation = rectTransform.localRotation;
        gridManager = FindObjectOfType<GridManager>();
    }
    void Update(){

        switch (currentState)
        {
            case 1:
                HandleHoverState();
                break;
            case 2:
                HandleDragState();
                if(!Input.GetMouseButton(0))
                {
                    TransitionToState0();
                }
                break; 
            case 3:
                HandlePlayState(); 
                
                break; 
        }
    }
    
    private void TransitionToState0(){
        
        currentState = 0; 
        rectTransform.localScale = originalScale;
        HandManager handManager = FindAnyObjectByType<HandManager>();
        handManager.UpdateHandVisual();
        glowEffect.SetActive(false); 
        playArrow.SetActive(false);

        gridManager.SetState0(); 
        gridManager.DeactivateHighlightedGrids();
    }
    public void OnPointerEnter(PointerEventData eventData){
        if(currentState==0){

            originalScale = rectTransform.localScale;
            orginalPosition = rectTransform.localPosition;
            orginalRotation = rectTransform.localRotation;
            currentState = 1;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if(currentState ==1)
        {
            TransitionToState0();
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if(currentState==1 && !isLocked)
        {
            currentState = 2;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out originalLocalPointerPosition);
            originalPanelLocalPosition = rectTransform.localPosition;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {

        if(currentState == 2 && !isLocked){

            Vector2 localPointerPosition;
            if(RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), eventData.position, eventData.pressEventCamera, out  localPointerPosition))
            {
                rectTransform.position = Vector3.Lerp(rectTransform.position, Input.mousePosition, lerpFactor);

                if(rectTransform.localPosition.x < cardPlay.x)
                {
                    currentState = 3;
                    playArrow.SetActive(true);
                    rectTransform.localPosition = Vector3.Lerp(rectTransform.position, playPosition, lerpFactor); 
                    gridManager.SetCurrentState(-1); 
                    gridManager.HighlightSpawnArea();

                }
            }

        }
    }
    
    private void HandleHoverState()
    {
        selfObject.transform.SetSiblingIndex(-100);
        glowEffect.SetActive(true);
        Vector3 tempPos = rectTransform.position;
        tempPos.z = -0.5f;
        rectTransform.position = tempPos;
        rectTransform.localScale = originalScale * selectScale; 
    }
    private void HandleDragState()
    {
        rectTransform.localRotation = Quaternion.identity;
    }
    private void HandlePlayState()
    {
        rectTransform.localPosition = playPosition;
        rectTransform.localRotation = Quaternion.identity;


        if(!Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if(hit.collider != null && hit.collider.GetComponent<GridCell>())
            {
                GridCell cell = hit.collider.GetComponent<GridCell>();
                Vector2 targetPos = cell.gridIndex;

                int tempNormalizedPosX;
                if(gridManager.GetCurrentTeam() == 0)
                {
                    tempNormalizedPosX = (int)targetPos.x;  
                } 
                else
                {
                    tempNormalizedPosX = gridManager.width- (int)targetPos.x- 1;
                }

                if(tempNormalizedPosX < spawnRange)
                { 
                if(gridManager.AddObjectToGrid(GetComponent<CardDisplay>().cardData.GetCardIndex(), targetPos, gridManager.GetCurrentTeam()))
                {
                    HandManager handManager = FindAnyObjectByType<HandManager>();
                    handManager.RemoveCardFromHand(gameObject);
                    handManager.UpdateHandVisual();
                    Destroy(gameObject);
                }
                }
            }
            TransitionToState0();
        }

        if(Input.mousePosition.y<cardPlay.y){
            currentState = 2; 
            playArrow.SetActive(false);
            gridManager.SetState0(); 
            gridManager.DeactivateHighlightedGrids();
        }

    }
    
    public int GetSiblingIndex(){
        return siblingIndex;
    }
    public void SetSiblingIndex(int index){
        siblingIndex = index;
    }
    
    
    public bool GetIsLocked()
    {
        return isLocked;
    }
    public void SetIsLocked(bool var)
    {
        isLocked = var;
    }
    

}
