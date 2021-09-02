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
    #pragma  warning restore 0649
    
    private Element _element;
    #endregion

    #region PUBLIC_VARIABLES

    public bool IsAnimating()
    {
        if (IsEmpty)
        {
            return false;
        }

        return (_element.IsAnimating);
    }


    public bool IsEmpty
    {
        get
        {
            return _element == null;
        }
    }

    public int HIndex
    {
        get { return hIndex; }
    }

    public int WIndex
    {
        get { return wIndex; }
    }

    #endregion

    #region PUBLIC_METHODS

    public void Init(int hIndex, int wIndex)
    {
        this.hIndex = hIndex;
        this.wIndex = wIndex;
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

    public bool HasSameElementAs(GridCell otherCell)
    {
        return _element.IsSame(otherCell._element);
    }

    public void ToggleInputInteractibility(bool isActive)
    {
        inputCollider.enabled = isActive;
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

    public void EmptyCell()
    {
        Destroy(_element.gameObject);
        _element = null;
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

   

}

