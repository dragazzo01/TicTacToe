using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class Main : MonoBehaviour
{
    public int numberOfDimensions;
    public int gridSize;
    public int numberOfPlayers;
    public int turnOrder;
    public bool clockRunning;
    public Player[] players;
    public Sprite[] sprites;
    GameObject[] grid;
    public GameObject prefab;
    public Transform allTiles;
    public Canvas mainOverlay;
    public TextMeshProUGUI timerText;
    public float[] timesLeft;
    


    void Start()
    {
        numberOfDimensions = TransferData.dimNum;
        gridSize = TransferData.gridNum;
        numberOfPlayers = TransferData.playerNum;
        turnOrder = 0;
        players = new Player[numberOfPlayers];
        timesLeft = new float[numberOfPlayers];
        for (int i = 0; i < numberOfPlayers; i++)
        {
            players[i] = new Player(TransferData.playerNames[i], TransferData.playerTimes[i], sprites[0], sprites[i + 1]);
            timesLeft[i] = TransferData.playerTimes[i];
        }
        CreateGame();
    }
    /// <summary>
    /// Checks for Mouse Click on Object
    /// </summary>
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject tile = ObjClick();
            if (tile != null && tile.GetComponent<Data>().clickable)
            {
                PlayerClick(tile);



            }
        }

        if (Input.GetKeyDown("p"))
        {
            int playerCount = 0;
            foreach (Player p in players)
            {
                if (p.timeLeft <= 0)
                {
                    playerCount++;
                }
            }
            if (playerCount == numberOfPlayers)
            {
                return;
            }
            clockRunning = !clockRunning;
            foreach (Transform child in allTiles)
            {
                child.gameObject.SetActive(!child.gameObject.activeSelf);
            }
        }

        if (Input.GetKeyDown("o"))
        {
            clockRunning = !clockRunning;   
        }

        if (Input.GetKeyDown("n"))
        {
            SceneManager.LoadScene("Start");
        }

        if (Input.GetKeyDown("r") && turnOrder != 0)
        {
            turnOrder--;   
            players[turnOrder % numberOfPlayers].Unclick();
        }
        
        if (timesLeft.Length != 0 && clockRunning)
        {
            UpdateTimer();
        }
    }

    public void UpdateTimer()
    {
        Player currentPlayer = players[turnOrder % numberOfPlayers];
        if (currentPlayer.timeLeft > 0)
        {
            currentPlayer.timeLeft -= Time.deltaTime;
        }
        else
        {
            currentPlayer.timeLeft = 0;
            turnOrder++;
            currentPlayer = players[turnOrder % numberOfPlayers];
        }

        int playerCount = 0;
        foreach (Player p in players)
        {
            if (p.timeLeft <= 0)
            {
                playerCount++;
            }
        }
        if (playerCount == numberOfPlayers)
        {
            clockRunning = false;
        }
         timerText.text = currentPlayer.name + ": " + (int) currentPlayer.timeLeft;
    }

    /// <summary>
    /// Determines which player is clicking and then clicks the tile
    /// </summary>
    /// <param name="tile"> tile being clicked </param>
    void PlayerClick(GameObject tile)
    {
        Player currentPlayer = players[turnOrder % numberOfPlayers];
        if (currentPlayer.timeLeft > 0) { currentPlayer.ClickTile(tile, gridSize); }
        turnOrder++;
        /*currentPlayer = players[turnOrder % numberOfPlayers];
        while (currentPlayer.timeLeft <= 0)
        {
            turnOrder++;
            currentPlayer = players[turnOrder % numberOfPlayers];
        }*/
    }

    public void DestroyGrid()
    {
        foreach (Transform child in allTiles)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Creates the grid by looping through every possible tile and determining what dimensions it has
    /// </summary>
    public void CreateGame()
    {
        int[] dimensionIndex = new int[numberOfDimensions];
        grid = new GameObject[(int) Mathf.Pow(gridSize, numberOfDimensions)];
        for (int dim = 0; dim < dimensionIndex.Length; dim++)
        {
            dimensionIndex[dim] = 0;
        }

        for (int i = 0; i < Mathf.Pow(gridSize, numberOfDimensions); i++)
        {
            CreateTile(dimensionIndex, i);
            for (int j = 0; j < numberOfDimensions; j++)
            {
                dimensionIndex[j]++;
                if (dimensionIndex[j] != gridSize) { break; }
                else
                {
                    dimensionIndex[j] = 0;
                }
            }
        }
    }

    /// <summary>
    /// Creates a tile based on specfications of CreateGrid()
    /// </summary>
    /// <param name="indexArray"> array with indexs of each dimension for each tile (ex {1, 0, 1} is 1 in 1st dim., 0 in 2nd dim., and 1 in 3rd) </param>
    /// <param name="number"> global index which identifies that particular tile </param>
    void CreateTile(int[] indexArray, int number)
    {
        string indx = "";
        int totalDispX = 0;
        int totalDispY = 0;
        int tempDispX = 1;
        int tempDispY = 1;

        for (int i = 0; i < indexArray.Length; i++)
        {
            if (i % 2 == 0)
            {
                totalDispX += tempDispX * indexArray[i];
                tempDispX = (tempDispX * gridSize) + 1;
            }
            else if (i % 2 == 1)
            {
                totalDispY += tempDispY * indexArray[i];
                tempDispY = (tempDispY * gridSize) + 1;
            }
            indx += indexArray[i].ToString();
        }

        grid[number] = Instantiate(prefab, new Vector3(totalDispX, totalDispY, 0), Quaternion.identity, allTiles);
        grid[number].name = indx;

        Data data = grid[number].GetComponent<Data>();
        data.index = indx;
        data.globalIndex = number;
    }

    /// <summary>
    /// Raycasts to click an object in screen
    /// </summary>
    /// <returns> Returns object clicked or null is no object is clicked </returns>
    GameObject ObjClick()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        if (hit.collider != null) { return hit.collider.gameObject; }
        else { return null; }
    }
}

/// <summary>
/// Object which stores info for 1 player
/// </summary>
public class Player
{
    public string name;
    public float timeLeft;
    public Sprite blankSprite;
    public Sprite playerSprite;
    public List<GameObject> tilesClicked = new List<GameObject>();
    public List<PossibleLine> lines = new List<PossibleLine>();

    /// <summary>
    /// Creates a player object which stores all necessary info about indivual players
    /// </summary>
    /// <param name="n"> name of the player </param>
    /// <param name="s"> sprite which tile will be changed to when clicked (unique to each player) </param>
    public Player(string n, float t, Sprite bs, Sprite ps)
    {
        name = n;
        timeLeft = t;
        blankSprite = bs;
        playerSprite = ps;   
    }

    /// <summary>
    /// clicks tile and checks if any line has been formed by that player
    /// </summary>
    /// <param name="tile"> tile that has been clicked </param>
    /// <param int="gridSize"> grid size of game (required to pass to LineCheck() </param>
    public void ClickTile(GameObject tile, int gridSize)
    {
        Data data = tile.GetComponent<Data>();
        tile.GetComponent<SpriteRenderer>().sprite = playerSprite;
        LineCheck(tile, gridSize);
        data.clickable = false;
        data.whoClicked = name;
        tilesClicked.Add(tile);
    }

    /// <summary>
    /// checks to see if a line has been formed by the current player; first checks point across all currently found possible lines and than makes sure no new lines have been created
    /// </summary>
    /// <param name="clickedTile"> new tile being checked </param>
    /// <param int="gridSize"> grid size of game (required to pass to GenerateOtherPoints() </param>
    /// <returns> true if a line has been found, false if no complete line formed </returns>
    public bool LineCheck(GameObject tile, int gridSize)
    {
        bool output = false;
        foreach (PossibleLine l in lines)
        {
            if (l.CheckNewPoint(tile) && l.missingPoints.Count == 0) 
            {
                l.ColorLine();
                output = true; 
            }
        }

        foreach (GameObject test in tilesClicked)
        {
            PossibleLine line = new PossibleLine(tile, test);
            if (line.GenerateOtherPoints(gridSize))
            {
                if (line.CheckAllPoints(tilesClicked)) 
                {
                    line.ColorLine();
                    output = true; 
                }
                else { lines.Add(line); }
            }
        }
        return output;
    }

    public void Unclick()
    {
        GameObject removed = tilesClicked[tilesClicked.Count - 1];
        removed.GetComponent<SpriteRenderer>().sprite = blankSprite;
        removed.GetComponent<Data>().clickable = true;
        removed.GetComponent<Data>().whoClicked = null;

        for (int i = lines.Count - 1; i >= 0; i--)
        {
            if (GameObject.ReferenceEquals(removed, lines[i].startPoint) || GameObject.ReferenceEquals(removed, lines[i].endPoint))
            {
                lines.RemoveAt(i);
            }
        }
        tilesClicked.Remove(removed);
    }
}

public class PossibleLine
{
    public GameObject startPoint;
    public GameObject endPoint;
    public int numOfDim;
    public List<string> missingPoints = new List<string>();
    public List<GameObject> foundPoints = new List<GameObject>();

    public PossibleLine(GameObject start, GameObject end)
    {
        startPoint = start;
        endPoint = end;
        numOfDim = startPoint.name.Length;
    }

    public bool GenerateOtherPoints(int gridSize)
    {
        string startString = startPoint.name;
        string endString = endPoint.name;  
        string matchPoints = "";
        for (int i = 0; i < startString.Length; i++)
        {
            if (startString[i] == endString[i])
            {
                matchPoints += "M";
            }
            else if (startString[i] == '0' && endString[i] == (gridSize - 1).ToString()[0])
            {
                matchPoints += "U";
            }
            else if (startString[i] == (gridSize - 1).ToString()[0] && endString[i] == '0')
            {
                matchPoints += "D";
            }
            else
            {
                return false;
            }
        }

        missingPoints.Add(startString);

        for (int i = 0; i < gridSize - 2; i++)
        {
            string newPoint = "";
            for (int j = 0; j < matchPoints.Length; j++)
            {
                int temp = missingPoints[i][j] - '0';
                if (matchPoints[j] == 'M')
                {
                    newPoint += startString[j];
                }
                else if (matchPoints[j] == 'U')
                {
                    newPoint += (temp + 1).ToString();
                }
                else if (matchPoints[j] == 'D')
                {
                    newPoint += (temp - 1).ToString();
                }
            }
            missingPoints.Add(newPoint);
        }
        missingPoints.RemoveAt(0);
        foundPoints.Add(startPoint);
        foundPoints.Add(endPoint);
        return true;
    }

    public bool CheckNewPoint(GameObject newPoint)
    {
        string newString = newPoint.name;
        for (int i = 0; i < missingPoints.Count; i++)
        {
            if (missingPoints[i] == newString)
            {
                missingPoints.RemoveAt(i);
                foundPoints.Add(newPoint);
                return true;
            }
        }
        return false;
    }

    public bool CheckAllPoints(List<GameObject> tilesClicked)
    {
        foreach (GameObject tile in tilesClicked)
        {
            if (CheckNewPoint(tile) && missingPoints.Count == 0)
            {
                return true;
            }
        }
        return false;
    }

    
    public void ColorLine()
    {
        for (int i = 0; i < foundPoints.Count; i++)
        {
            foundPoints[i].GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
        }
    }
    
}

