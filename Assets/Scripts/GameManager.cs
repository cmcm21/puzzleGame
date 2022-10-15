using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private AssetReference gameplayConfigRef;
    [SerializeField] private Transform puzzleContainer;
    [SerializeField] private RectTransform rightSidePosition;
    [SerializeField] private RectTransform leftSidePosition;
    [SerializeField] private Image piecePrefab;
    [SerializeField] private Image pieceHolderPrefab;
    [SerializeField] private float waitTime = 1.5f;

    private GameplayConfiguration _gameplayConfiguration;
    private List<Transform> _placeHoldersTransforms;
    private List<Piece> _pieces;
    private List<SpritePlaceHolder> _spritesPlaceHolders;

    public struct SpritePlaceHolder
    {
        public Sprite sprite;
        public Transform placeHolder;
    }

    private void Awake()
    {
        DataManager.OnGameplayDataLoaded += DataManager_OnGameplayDataLoaded;
        _placeHoldersTransforms = new List<Transform>();
        _pieces = new List<Piece>();
        _spritesPlaceHolders = new List<SpritePlaceHolder>();
    }

    private void Start()
    {
       DataManager.Init(gameplayConfigRef); 
    }
    
    private void DataManager_OnGameplayDataLoaded(GameplayConfiguration gameplayConfig)
    {
        _gameplayConfiguration = gameplayConfig;
        CreatePuzzle();
        StartCoroutine(WaitThen(RearrangePieces, waitTime));
    }

    private void RearrangePieces()
    {
        var random = new Random();
        _spritesPlaceHolders = _spritesPlaceHolders.OrderBy(sprite => random.Next()).ToList();
        for (int i = 0; i < _pieces.Count; i++)
        {
            var spritePlaceHolder = _spritesPlaceHolders[i];
            _pieces[i].Set(spritePlaceHolder.sprite,spritePlaceHolder.placeHolder);
        }
    }

    private void CreatePuzzle()
    {
        CreatePlaceHolders();
        CreatePieces();
    }

    private void CreatePlaceHolders()
    {
        for(int i = 0; i < _gameplayConfiguration.Rows * _gameplayConfiguration.Columns; i++)
        {
            var placeHolder = CreatePuzzleObject(i, false);
            _placeHoldersTransforms.Add(placeHolder.transform);
            _spritesPlaceHolders.Add(new SpritePlaceHolder()
            {
                sprite = _gameplayConfiguration.Sprites[i],
                placeHolder = placeHolder.transform
            });
        }
    }

    private void CreatePieces()
    {
        for(int i = 0; i < _gameplayConfiguration.Rows * _gameplayConfiguration.Columns; i++)
        {
            var piece = CreatePuzzleObject(i, true);
            CreatePiece(piece, _placeHoldersTransforms[i]);
        }
    }

    private GameObject CreatePuzzleObject(int index,bool piece)
    {
        var Width = _gameplayConfiguration.PieceWidth;
        var Height = _gameplayConfiguration.PieceHeight;
        var row = index % _gameplayConfiguration.Rows;
        var column = index / _gameplayConfiguration.Columns;
        Vector3 centerPosition = piece ? leftSidePosition.transform.position : rightSidePosition.transform.position;
        
        Vector3 position = new Vector3(
            centerPosition.x + Width * (row),
            centerPosition.y - Height * (column),
            centerPosition.z);

        Sprite sprite = piece ? _gameplayConfiguration.Sprites[index] : null;
        Image puzzleObj = piece ?  piecePrefab: pieceHolderPrefab;

        return CreateImage(puzzleObj, position,sprite);
    }

    private GameObject CreateImage(Image go,Vector3 position, Sprite sprite = null)
    {
        var puzzleObject = Instantiate(go, position, Quaternion.identity);
        if (sprite != null)
        {
            if (puzzleObject.TryGetComponent(out Image image))
                image.sprite = sprite;
            else
                Debug.LogError($"[{GetType()}]:: Error trying to set sprite, Object: {puzzleObject.name}");
        }
        puzzleObject.transform.SetParent(puzzleContainer);
        return puzzleObject.gameObject;
    }

    private void CreatePiece(GameObject pieceImage, Transform target)
    {
        var piece = pieceImage.GetComponent<Piece>();
        if (piece != null)
            _pieces.Add(piece);
        else
        {
            Debug.LogError($"[{GetType()}]:: Error trying to create piece, " +
                           $"{pieceImage.name} doesn't have piece component");
        }
    }

    private IEnumerator WaitThen(Action callback, float time)
    {
        yield return new WaitForSeconds(time);
        callback?.Invoke();
    }

    private void OnDestroy()
    {
        DataManager.Dispose();
        DataManager.OnGameplayDataLoaded -= DataManager_OnGameplayDataLoaded;
    }
}
