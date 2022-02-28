using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class GridCell : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    #region PRIVATE_VARIABLES
    [SerializeField] private int hIndex;
    [SerializeField] private int wIndex;
    [SerializeField] private BoxCollider2D inputCollider;
    [SerializeField] private RenderLayer renderLayer;


public bool lockedInAnimation;
    public new SpriteRenderer renderer;
    [SerializeField]private Element _element;
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
            if (_blocker == null)
                return false;

            return _blocker.DoesBlockCell();
        }
    }


    public RenderLayer RenderLayer{
        get{
            return renderLayer;
        }
    }

    public MatchExecutionData executionData { get; private set; }

    public GridCell bottomCell, bottomLeftCell, bottomRightCell;
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
      


    public void SetElement(Element newElement) {
        
        if (!IsEmpty)
        {
            throw new Exception($"Write before read at cell {gameObject.name}");
        }


        _element = newElement;
    }


    public void LockCell(MatchExecutionData executionData) {
        renderer.color = Color.red;
        SetExecutionData(executionData);
        ToggleInputInteractibility(false);
    }

    public void UnlockCell() {

        ToggleInputInteractibility(true);
        SetExecutionData(null);
        renderer.color = Color.gray;
    }


    public void SetBlocker(CellBlocker blocker) {

        _blocker = blocker;
        inputCollider.enabled = (blocker==null);
    }

    public void SetExecutionData(MatchExecutionData executionData) {
        this.executionData = executionData;
    }

   

    public override string ToString()
    {
        return gameObject.name;
    }

    #endregion




}

