using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : Singleton<InputManager>
{
    [SerializeField] private Camera _gameplayCamera;
    [SerializeField] private GridCell _firstCell;
    [SerializeField] private GridCell _secondCell;
    [SerializeField] private ContactFilter2D _contactFilter;

    [SerializeField] private List<EntityTuple<ElementConfig,MonoBehaviour>> _serializedSwipeHandlerLookup;
    [SerializeField] private MonoBehaviour _defaultSwipeHandlerMono;

    private Dictionary<ElementConfig,IMatchHandler> _swipeHandlerLookup = new Dictionary<ElementConfig,IMatchHandler>();
    private IMatchHandler _defaultSwipeHandler;
    
    private int swipeNumber = 0;
    private bool _inputValid;


    public void Init()
    {
        ParseInterfaces();
    }

    private void ParseInterfaces()
    {
        foreach (var item in _serializedSwipeHandlerLookup)
        {
            IMatchHandler handler = item.Value as IMatchHandler;
            _swipeHandlerLookup.Add(item.Key,handler);
            handler?.Init();
            
        }
        
        _defaultSwipeHandler = _defaultSwipeHandlerMono as IMatchHandler;
        _defaultSwipeHandler.Init();
    }

    private void Update()
    {

        if (Input.GetMouseButton(0) && _inputValid) 
        {
            Ray ray = _gameplayCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D[] hits = new RaycastHit2D[1];
            Physics2D.Raycast(ray.origin, ray.direction, _contactFilter, hits);
            Debug.DrawLine(ray.origin, ray.direction);

            if (hits[0].collider!=null) 
            { 
                GridCell cell = hits[0].collider.GetComponent<GridCell>();
                SetFirstCell(cell);
                SetSecondCell(cell);
                ValidateMove();
            }
        }

        if (Input.GetMouseButtonUp(0))
            _inputValid = true;

    }


    public void SetFirstCell(GridCell firstCell)
    {
        if(firstCell.IsEmpty)
            return;

        if (IsCellAssigned(_firstCell))
            return;
        
        _firstCell = firstCell;
    }

   

    public void SetSecondCell(GridCell secondCell)
    {
        if (!IsCellAssigned(_firstCell))
            return;

        if (_firstCell.Equals(secondCell))
            return;

        if (secondCell.IsEmpty)
        {
            _firstCell = null;
            _secondCell = null;
            return;
        }

        _secondCell = secondCell;
        
    }

   
    private void ValidateMove()           
    {
        if (IsCellAssigned(_firstCell) && IsCellAssigned(_secondCell))
        {
            if (AreNeighbours(_firstCell,_secondCell))
            {
                OnValidMove();
            }
        }
    }

    private void OnValidMove()
    {
        _inputValid = false;
        MatchExecutionData matchExecutionData = new MatchExecutionData(new List<MatchData>(),new List<GridCell>(),swipeNumber,_firstCell,_secondCell);
        swipeNumber++;
        matchExecutionData.firstCell.SetExecutionData(matchExecutionData);
        matchExecutionData.secondCell.SetExecutionData(matchExecutionData);
        _firstCell = _secondCell = null;
        HandleSwipe(matchExecutionData);
    }

    private void HandleSwipe(MatchExecutionData matchExecutionData) 
    {
        ElementConfig firstElement = matchExecutionData.FirstElement.ElementConfig;
        ElementConfig secondElement = matchExecutionData.SecondElement.ElementConfig;

        IMatchHandler swipeHandler=null;

        if(_swipeHandlerLookup.ContainsKey(firstElement))
            swipeHandler = _swipeHandlerLookup[firstElement];

        if (swipeHandler == null && _swipeHandlerLookup.ContainsKey(secondElement))
            swipeHandler = _swipeHandlerLookup[secondElement];

        if (swipeHandler == null)
            swipeHandler = _defaultSwipeHandler;

        swipeHandler.OnSwipeRecieved(matchExecutionData);
    }


    private bool IsCellAssigned(GridCell cell)
    {
        return cell != null;
    }

    private bool AreNeighbours(GridCell firstCell, GridCell secondCell)
    {

        bool isVerticalPositionSame = firstCell.HIndex == secondCell.HIndex;
        bool isHorizontalPositionSame = firstCell.WIndex == secondCell.WIndex;
        int cellDistance;

        if (isHorizontalPositionSame)
        {
            cellDistance = Mathf.Abs(firstCell.HIndex - secondCell.HIndex);
        }
        else if (isVerticalPositionSame)
        {
            cellDistance = Mathf.Abs(firstCell.WIndex - secondCell.WIndex);
        }
        else
        {
            return false;
        }

        bool areNeighbours = cellDistance <= 1 && (isVerticalPositionSame || isHorizontalPositionSame);

        return areNeighbours;
    }



    #if UNITY_EDITOR
    private void OnValidate()
    {
        foreach (var item in _serializedSwipeHandlerLookup)
        {
            if (item.Value != null && !(item.Value is IMatchHandler)) {
                item.Value = null;
                Debug.LogError($"Only implementations of {typeof(IMatchHandler)} are allowed");
            }
        }

        var item2 = _defaultSwipeHandler;

        if (item2 != null && !(item2 is IMatchHandler))
        {
            item2 = null;
            Debug.LogError($"Only implementations of {typeof(IMatchHandler)} are allowed");
        }
    }
    #endif
}
