using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }

    public GameObject explosionFX;
    public Transform FXposition;

    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);

    private List<int> currentSet = new List<int>();

    private int thisPieceIndex = -1;

    private int nextPieceIndex = -1;
    [SerializeField] private GameObject[] NextMinoes;
    private bool fst = true;

    private bool holdUsed = false;
    private int holdPieceIndex = -1;
    [SerializeField] private GameObject[] holdMinos;

    


    public RectInt Bounds 
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominoes.Length; i++) 
        {
            tetrominoes[i].Initialize();
        }
        InitializeSet();
        SetNextPiece();
    }

    private void Start()
    {
        SpawnPiece();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            HoldPiece();
        }
    }


    public void SpawnPiece()
    {
        if (nextPieceIndex == -1)
        {
            SetNextPiece();
        }

        TetrominoData data = tetrominoes[nextPieceIndex];

        activePiece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(activePiece, spawnPosition))
        {
            Set(activePiece);
        }
        else
        {
            GameOver();
        }

        SetNextPiece();
    }
    private void HoldPiece()
    {
        if (holdUsed)
        {
            return;
        }

        if (holdPieceIndex == -1)
        {
            holdPieceIndex = thisPieceIndex;
            Clear(activePiece);
            SpawnPiece();
        }
        else
        {
            holdMinos[holdPieceIndex].gameObject.SetActive(false);

            TetrominoData data = tetrominoes[holdPieceIndex];
            holdPieceIndex = thisPieceIndex;
            Clear(activePiece);

            activePiece.Initialize(this, spawnPosition, data);
        }

        holdUsed = true;

        holdMinos[holdPieceIndex].gameObject.SetActive(true);
    }

    private void SetNextPiece()
    {
        if (!fst) NextMinoes[nextPieceIndex].gameObject.SetActive(false);
        else fst = false;

        thisPieceIndex = nextPieceIndex;

        if (currentSet.Count == 0)
        {
            InitializeSet();
        }

        int setIndex = Random.Range(0, currentSet.Count);
        nextPieceIndex = currentSet[setIndex];
        currentSet.RemoveAt(setIndex);

        UpdateNextPieceUI();

        holdUsed = false;
    }

    private void UpdateNextPieceUI()
    {
        NextMinoes[nextPieceIndex].gameObject.SetActive(true);
    }
    public void GameOver()
    {
        tilemap.ClearAllTiles();
        GameManager.instance.isGameover = true;
    }
    private void InitializeSet()
    {
        currentSet.Clear();
        for (int i = 0; i < tetrominoes.Length; i++)
        {
            currentSet.Add(i);
        }
    }
    public void Set(Piece piece)
    {
        if (!GameManager.instance.isGameover)
        {
            for (int i = 0; i < piece.cells.Length; i++)
            {
                Vector3Int tilePosition = piece.cells[i] + piece.position;
                tilemap.SetTile(tilePosition, piece.data.tile);
            }
        }
    }

    public void Clear(Piece piece)
    {
        if (!GameManager.instance.isGameover)
        {
            for (int i = 0; i < piece.cells.Length; i++)
            {
                Vector3Int tilePosition = piece.cells[i] + piece.position;
                tilemap.SetTile(tilePosition, null);
            }
        }
        
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        if (!GameManager.instance.isGameover)
        {
            RectInt bounds = Bounds;

            for (int i = 0; i < piece.cells.Length; i++)
            {
                Vector3Int tilePosition = piece.cells[i] + position;

                if (!bounds.Contains((Vector2Int)tilePosition))
                {
                    return false;
                }

                if (tilemap.HasTile(tilePosition))
                {
                    return false;
                }
            }
            
        }
        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;
        if (!GameManager.instance.isGameover)
        {
            while (row < bounds.yMax)
            {
                if (IsLineFull(row))
                {
                    LineClear(row);
                    Instantiate(explosionFX,FXposition);
                    GameManager.instance.Score++;
                }
                else
                {
                    row++;
                }
            }
        }
        
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            // The line is not full if a tile is missing
            if (!tilemap.HasTile(position)) {
                return false;
            }
        }

        return true;
    }

    public void LineClear(int row)
    {
        RectInt bounds = Bounds;

        // Clear all tiles in the row
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

        // Shift every row above down one
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }
    }

}
