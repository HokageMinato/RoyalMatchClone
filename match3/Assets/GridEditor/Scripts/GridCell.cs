using UnityEngine;
using UnityEngine.EventSystems;

public class GridCell : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    #region PRIVATE_VARIABLES
    [SerializeField] private int hIndex;
    [SerializeField] private int wIndex;
    #endregion

    #region PUBLIC_VARIABLES
    public Element element;
    public bool IsBoundaryCell = false;

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


    private bool IsHorizontalNeighbourOf(GridCell otherCell)
    {
        int absoluteDifference = Mathf.Abs(otherCell.wIndex - wIndex);
        bool isVerticalPositionSame = otherCell.hIndex == hIndex;
        return absoluteDifference < 2 && otherCell != this && isVerticalPositionSame;
    }

    private bool IsVerticalNeighbourOf(GridCell otherCell)
    {
        int absoluteDifference = Mathf.Abs(otherCell.hIndex - hIndex);
        bool isHorizontalPositionSame = otherCell.wIndex == wIndex;
        return absoluteDifference < 2 && otherCell != this && isHorizontalPositionSame;
    }

    public bool IsNeighbourOf(GridCell otherCell)
    {
        return IsHorizontalNeighbourOf(otherCell) || IsVerticalNeighbourOf(otherCell);
    }

   
    #endregion
}