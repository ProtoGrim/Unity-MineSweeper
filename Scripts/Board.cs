
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using VectorExt;
using TMPro;

public class Board : MonoBehaviour
{
    [Header("Board Size")]
    public int boardSizeX = 10;
    public int boardSizeY = 10;
    public float scale = 1;

    [Header("Board Position")]
    public float boardPosX = 0;
    public float boardPosY = 0;

    [Header("Mine Count")]
    public int mineCount = 10;

    [Header("Tile Prefabs and Sprites")]
    public GameObject tilePiece;
    public Sprite numberSprite;
    public Sprite mineSprite;
    public Sprite baseTileSprite;
    public Sprite markedSprite;

    [Header("Buttons")]
    public GameObject resetButton;
    public TextMeshProUGUI gameStateText;

    [Header("Board")]
    public GameObject[,] board;

    private int numbersLeft;
    private bool started = false;

    public void StartGame()
    {
        
        started = false;

        Debug.Log("I have read and agreed to the terms of service.");
        if (mineCount > boardSizeX * boardSizeY)
        {
            Debug.LogErrorFormat("Initial Mine Count was greater than maximum amount of tiles. Mine Count : {0}, tile count: {1}\nBoard.cs", mineCount, boardSizeX * boardSizeY);
            mineCount = (boardSizeX * boardSizeY);
        }
        else if (mineCount < 0)
        {
            Debug.LogErrorFormat("Initial Mine Count was less than zero. Mine Count : {0}\nBoard.cs", mineCount);
            mineCount = 0;
        }
        board = new GameObject[boardSizeX, boardSizeY];

        numbersLeft = boardSizeX * boardSizeY - mineCount;

        gameStateText.text = "";

        FillBoard(boardSizeX / 2 * scale, boardSizeY / 2 * scale);
    }

    private void FillBoard(float offsetX = 0, float offsetY = 0)
    {
        for (int y = 0; y < boardSizeY; ++y)
        {
            for (int x = 0; x < boardSizeX; ++x)
            {
                Vector2 tilePosition = new Vector2(
                                                    -offsetX + (scale / 2 * ((boardSizeX + 1) % 2)) + (x * scale) + boardPosX,
                                                    -offsetY + (scale / 2) + (y * scale) + boardPosY
                                                  );

                GameObject obj;
                Tile script;

                obj = board[x, y] = Instantiate(tilePiece, tilePosition, Quaternion.identity, transform);
                script = obj.AddComponent<NumberTile>();
                script.Count = 0;
                script.pos = new Vector2(x, y);
                script.transform.localScale = new Vector2(scale, scale);
                script.name = "Tile";
                script.changeTo = numberSprite;
            }
        }
    }

    private void PlaceMines(Vector2 safeZone)
    {
        List<int> placements = new List<int>();
        for (int i = 0; i < mineCount; ++i)
        {
            int pos = Random.Range(0, boardSizeX * boardSizeY - 1);
            int x = pos % boardSizeX;
            int y = pos / boardSizeX;
            if (placements.Contains(pos) || ((safeZone.x >= x - 1) && (safeZone.x <= x + 1) && (safeZone.y >= y - 1) && (safeZone.y <= y + 1)) )
            {
                do
                {
                    pos = Random.Range(0, boardSizeX * boardSizeY - 1);
                    x = pos % boardSizeX;
                    y = pos / boardSizeX;
                } while (placements.Contains(pos) || ( (safeZone.x >= x - 1)  &&  (safeZone.x <= x + 1)  &&  (safeZone.y >= y - 1)  &&  (safeZone.y <= y + 1) ) );
            }
            placements.Add(pos);
        }

        foreach (int pos in placements)
        {
            Vector2 position = new Vector2(pos % boardSizeX, pos / boardSizeX);
            MineTile script;
            script = board[(int)position.x, (int)position.y].AddComponent<MineTile>();
            script.changeTo = mineSprite;
            script.pos = new Vector2(pos % boardSizeX, pos / boardSizeX);
            for (int x = -1; x <= 1; ++x)
            {
                for (int y = -1; y <= 1; ++y)
                {
                    Vector2 newPos = new Vector2(x, y);
                    if ((position + newPos).inRange(Vector2.zero, new Vector2(boardSizeX, boardSizeY)))
                        board[(int)(position.x + newPos.x), (int)(position.y + newPos.y)].GetComponent<Tile>().Count += 1;
                }
            }
        }
    }

    public bool TileClicked(Vector2 pos)
    {
        if (!started)
        {
            PlaceMines(pos);
            started = true;
        }
        if (pos.inRange(Vector2.zero, new Vector2(boardSizeX, boardSizeY)))
        {
            Tile script = board[(int)pos.x, (int)pos.y].GetComponent<Tile>();
            if (!script.Visible && !script.Marked)
            {
                if (script.Count != -1)
                    --numbersLeft;
                if (numbersLeft == 0)
                {
                    Debug.Log("You Wins!");
                    GameOver(true);
                    return true;
                }
                if (script.Show())
                {
                    for (int x = -1; x <= 1; ++x)
                    {
                        for (int y = -1; y <= 1; ++y)
                        {
                            Vector2 newPos = new Vector2(x, y);
                            if (TileClicked(new Vector2(pos.x + newPos.x, pos.y + newPos.y)))
                            {
                                return true;
                            }
                            
                        }
                    }
                    return false;
                }
            }
        }
        return false;
    }

    public void MarkTile(Vector2 pos)
    {
        GameObject obj = board[(int)pos.x, (int)pos.y];
        if (obj.GetComponent<Tile>().Marked)
        {
            obj.GetComponent<SpriteRenderer>().sprite = markedSprite;
        }
        else
        {
            obj.GetComponent<SpriteRenderer>().sprite = baseTileSprite;
        }
    }

    public void ShowSurrounding(Vector2 pos)
    {
        GameObject obj = board[(int)pos.x, (int)pos.y];

        int count = 0;

        for (int x = -1; x <= 1; ++x)
        {
            for (int y = -1; y <= 1; ++y)
            {
                if ((new Vector2(pos.x + x, pos.y + y)).inRange(Vector2.zero, new Vector2(boardSizeX, boardSizeY)))
                {
                    Tile script = board[(int)(pos.x + x), (int)(pos.y + y)].GetComponent<Tile>();
                    if (script.Marked)
                    {
                        ++count;
                    }
                }
            }
        }
        if (count == obj.GetComponent<NumberTile>().Count)
        {
            for (int x = -1; x <= 1; ++x)
            {
                for (int y = -1; y <= 1; ++y)
                {
                    if ((new Vector2(pos.x + x, pos.y + y)).inRange(Vector2.zero, new Vector2(boardSizeX, boardSizeY)))
                    {
                        Vector2 newPos = new Vector2(x, y);
                        TileClicked(new Vector2(pos.x + newPos.x, pos.y + newPos.y));
                    }
                }
            }
        }
    }

    private void ShowAll(bool keepFlags = false)
    {
        for (int x = 0; x < boardSizeX; ++x)
        {
            for (int y = 0; y < boardSizeY; ++y)
            {
                Tile script = board[x, y].GetComponent<Tile>();
                if (!keepFlags || !script.Marked)
                    script.ShowRaw();
                Destroy(script);
            }
        }
    }

    public void GameOver(bool win)
    {
        string text;
        if (win)
        {
            ShowAll(true);
            text = "You Win!";
        }
        else
        {
            ShowAll();
            text = "Game Over!";
        }
        gameStateText.gameObject.SetActive(true);
        gameStateText.text = text;
        resetButton.SetActive(true);
    }

    public void ResetScreen()
    {
        gameStateText.gameObject.SetActive(false);
        foreach (GameObject obj in board)
        {
            Destroy(obj);
        }
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().ResetGame();
    }
}
