using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridCell : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    #region PRIVATE_VARIABLES
    [SerializeField] private int hIndex;
    [SerializeField] private int wIndex;
    [SerializeField] private BoxCollider2D inputCollider;
    public new SpriteRenderer renderer;
    
    private Element _element;
    private CellBlocker _blocker;
    #endregion

    #region PUBLIC_VARIABLES
    public int WIndex
    {
        get { return wIndex; }
    }

    public int HIndex
    {
        get { return hIndex; }
    }

    public bool IsEmpty
    {
        get
        {
            return _element == null;
        }
    }

    public bool IsBlocked 
    {
        get 
        {
            return _blocker != null && _blocker.DoesBlockCell();
        }
    }

   public MatchExecutionData executionData { get; private set; }
   #endregion

    
    #region PUBLIC_METHODS

    public void Init(int hIndex, int wIndex)
    {
        this.hIndex = hIndex;
        this.wIndex = wIndex;
        renderer.sortingOrder = (int)GameLayer.BASE_LAYER;
        executionData = MatchExecutionData.GetDefaultExecutionData();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        InputManager inputManager = InputManager.instance;
        inputManager.SetFirstCell(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        InputManager inputManager = InputManager.instance;
        inputManager.SetSecondCell(this);
    }
    

    //public bool IsNeighbourOf(GridCell otherCell)
    //{
    //    return IsHorizontalNeighbourOf(otherCell) || IsVerticalNeighbourOf(otherCell);
    //}

    public void ToggleInputInteractibility(bool isActive)
    {
        inputCollider.enabled = isActive;
    }

    public Element ReadElement()
    {
        return _element;
    }

    public Element GetElement()
    {
        Element lastElement = _element;
        _element = null;
        return lastElement;
    }

    public void SetElement(Element newElement)
    {
        if (!IsEmpty)
        {
            Debug.LogWarning($"Override at cell {gameObject.name}");
        }

        _element = newElement;
        _element.SetHolder(this);
    }

    public void SetBlocker(CellBlocker blocker) {

        _blocker = blocker;
    }

    public void SetExecutionData(MatchExecutionData executionData) {
        this.executionData = executionData;
    }

    public void ClearExecutionData() {
        SetExecutionData(null);
    }
    #endregion
    
    

   
}

