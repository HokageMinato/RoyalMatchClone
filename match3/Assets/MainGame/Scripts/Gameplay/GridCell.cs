using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridCell : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    #region PRIVATE_VARIABLES
    #pragma  warning disable 0649
    [SerializeField] private int hIndex;
    [SerializeField] private int wIndex;
    [SerializeField] private BoxCollider2D inputCollider;
    public SpriteRenderer renderer;
    #pragma  warning restore 0649
    
    private Element _element;
    #endregion

    #region PUBLIC_VARIABLES

    public int HIndex
    {
        get { return wIndex; }
    }

    public bool IsEmpty
    {
        get
        {
            return _element == null;
        }
    }

   

    //public bool isMarkedForDestory = false;
    public MatchExecutionData executionData;
    #endregion

    
    #region PUBLIC_METHODS

    public void Init(int hIndex, int wIndex)
    {
        this.hIndex = hIndex;
        this.wIndex = wIndex;
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
    

    public bool IsNeighbourOf(GridCell otherCell)
    {
        return IsHorizontalNeighbourOf(otherCell) || IsVerticalNeighbourOf(otherCell);
    }

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

    

    #endregion
    
    #region PRIVATE_METHODS
    private bool IsHorizontalNeighbourOf(GridCell otherCell)
    {
        int absoluteDifference = Mathf.Abs(otherCell.wIndex - wIndex);
        bool isVerticalPositionSame = otherCell.hIndex == hIndex;
        return absoluteDifference <= 1 && otherCell != this && isVerticalPositionSame;
    }

    private bool IsVerticalNeighbourOf(GridCell otherCell)
    {
        int absoluteDifference = Mathf.Abs(otherCell.hIndex - hIndex);
        bool isHorizontalPositionSame = otherCell.wIndex == wIndex;
        return absoluteDifference <= 1 && otherCell != this && isHorizontalPositionSame;
    }
    #endregion


    public void SetExecutionData(MatchExecutionData matchExecutionData)
    {
        executionData = matchExecutionData;
    }
}

