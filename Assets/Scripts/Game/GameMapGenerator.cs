using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameMapGenerator : MonoBehaviour
{
    [Serializable]
    public class PositionInfos
    {
        public List<Vector2> positionBricksHor;
        public List<Vector2> positionBricksVer;
        public List<Vector2> positionBricksJoint;
        public List<Vector2> positionBricksTunnelIn;
        public List<Vector2> positionBricksTunnelOut;
        public List<Vector2> positionRoofs;
        public List<Vector2> positionPits;
    }

    public GameObject cellPrefab;
    public GameObject borderCellPrefab;
    public GameObject exitCellPrefab;
    public GameObject[] obstaclePrefab;
    public int gridSizeX;
    public int gridSizeY;
    public float cellSize = 1f;
    public float obstacleProbability = 0.2f; // Probability of spawning an obstacle in each cell
    public int numberOfTetrominos = 3; // Number of tetrominos to generate

    public List<Vector2> positionForBricks = new List<Vector2>();
    public List<Vector2> positionForBricksHorizontal = new List<Vector2>();
    public List<Vector2> positionForBricksVertical = new List<Vector2>();
    public List<Vector2> positionForBricksJoint = new List<Vector2>();
    public List<Vector2> positionForRoofs = new List<Vector2>();
    public List<Vector2> positionForPits = new List<Vector2>();
    public List<Vector2> positionForTeleInput = new List<Vector2>();
    public List<Vector2> positionForTeleOutput = new List<Vector2>();
    public PositionInfos positionInfo;

    [Space(10)]
    public List<Vector2> playerSpawnPos = new List<Vector2>();

    [HideInInspector] public List<Transform> cellTransform = new List<Transform>();
    [HideInInspector] public List<Vector2> tetrominoTransform = new List<Vector2>();

    public List<Transform> listOfSpawnPos = new List<Transform>();

    public GameObject teleportEntryPrefab;
    public GameObject teleportExitPrefab;
    public int numberOfTeleporters = 5;
    public static bool isStarted = false;

    //private List<Vector2> teleportEntryPositions = new List<Vector2>();
    //private List<Vector2> teleportExitPositions = new List<Vector2>();

    //public Text testText;

    // Define tetromino shapes
    private readonly int[][][] tetrominos =
    {
        // I Tetromino
        new int[][] {
            new int[] {1},
            new int[] {1},
            new int[] {1},
            new int[] {1}
        },
        // J Tetromino
        new int[][] {
            new int[] {1, 0},
            new int[] {1, 0},
            new int[] {1, 1}
        },
        // L Tetromino
        new int[][] {
            new int[] {0, 1},
            new int[] {0, 1},
            new int[] {1, 1}
        },
        // O Tetromino
        new int[][] {
            new int[] {1, 1},
            new int[] {1, 1}
        },
        // S Tetromino
        new int[][] {
            new int[] {0, 1, 1},
            new int[] {1, 1, 0}
        },
        // T Tetromino
        new int[][] {
            new int[] {1, 1, 1},
            new int[] {0, 1, 0}
        },
        // Z Tetromino
        new int[][] {
            new int[] {1, 1, 0},
            new int[] {0, 1, 1}
        }
    };
    

    void Start()
    {
        if (gridSizeX <= 0) gridSizeX = 5;
        if (gridSizeY <= 0) gridSizeY = 5;

        isStarted = false;

        StartCoroutine(DelayToActivatePlayer());        
    }

    void GenerateGridWithObstacles()
    {
        // Load map data               

        string jsonData = PlayerPrefs.GetString("INFOS_ROOM1");
        positionInfo = JsonUtility.FromJson<PositionInfos>(jsonData);

        Vector2 startPosition = new Vector2(transform.position.x - (gridSizeX / 2) * cellSize, transform.position.y + (gridSizeY / 2) * cellSize);

        // List to store valid positions for exit cell on the top border
        List<int> validExitPositions = new List<int>();
        bool isWall = false;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                GameObject prefabToInstantiate = cellPrefab;
                isWall = false;

                // Check if it's a border cell
                if (y == 0 || y == gridSizeY - 1)
                {
                    prefabToInstantiate = obstaclePrefab[5];
                    isWall = true;
                }
                else if (x == gridSizeX - 1 || x == 0)
                {
                    prefabToInstantiate = obstaclePrefab[7];
                    isWall = true;
                }

                // Check if it's an exit cell
                if (y == gridSizeY - 1 && x != 0 && x != gridSizeX - 1)
                {
                    validExitPositions.Add(x); // Add valid position on the top border
                }

                Vector2 spawnPosition = startPosition + new Vector2(x * cellSize, -y * cellSize);

                // Instantiate cell prefab
                GameObject newCell = Instantiate(prefabToInstantiate, spawnPosition, Quaternion.identity);
                newCell.transform.parent = transform; // Set the grid object as the parent

                if (!isWall)
                {
                    cellTransform.Add(newCell.transform);
                }                            

                if (newCell.tag == "Road" && (spawnPosition.x <= -10 && spawnPosition.y <= -10)) playerSpawnPos.Add(spawnPosition);
            }
        }

        // Choose random position from valid exit positions
        //int exitPosX = validExitPositions[UnityEngine.Random.Range(0, validExitPositions.Count)];

        //// Place the exit cell
        //Vector2 exitSpawnPosition = startPosition + new Vector2(exitPosX * cellSize, -cellSize);
        //GameObject exitCell = Instantiate(exitCellPrefab, exitSpawnPosition, Quaternion.identity);
        //exitCell.transform.parent = transform;
        //GameManager.instance.finishLine = exitCell;
        //GameManager.instance.finishLine.SetActive(false);

        // Keep track of occupied positions
        HashSet<Vector2> occupiedPositions = new HashSet<Vector2>();

        // Generate walls
        //for (int i = 0; i < positionForBricksHorizontal.Count; i++)
        //{
        //    Vector2 spawnPositionForBricks = startPosition + new Vector2((positionForBricksHorizontal[i].x) * cellSize, -(positionForBricksHorizontal[i].y) * cellSize);
        //    GameObject newObstacle = Instantiate(obstaclePrefab[5], spawnPositionForBricks, Quaternion.identity);
        //    newObstacle.transform.parent = transform;

        //    tetrominoTransform.Add(spawnPositionForBricks);
        //    occupiedPositions.Add(spawnPositionForBricks);
        //}

        //for (int i = 0; i < positionForBricksVertical.Count; i++)
        //{
        //    Vector2 spawnPositionForBricks = startPosition + new Vector2((positionForBricksVertical[i].x) * cellSize, -(positionForBricksVertical[i].y) * cellSize - 1.4f);
        //    GameObject newObstacle = Instantiate(obstaclePrefab[7], spawnPositionForBricks, Quaternion.identity);
        //    newObstacle.transform.parent = transform;

        //    tetrominoTransform.Add(spawnPositionForBricks);
        //    occupiedPositions.Add(spawnPositionForBricks);
        //}

        //for (int i = 0; i < positionForBricksJoint.Count; i++)
        //{
        //    Vector2 spawnPositionForBricks = startPosition + new Vector2((positionForBricksJoint[i].x) * cellSize, -(positionForBricksJoint[i].y) * cellSize);
        //    GameObject newObstacle = Instantiate(obstaclePrefab[8], spawnPositionForBricks, Quaternion.identity);
        //    newObstacle.transform.parent = transform;

        //    tetrominoTransform.Add(spawnPositionForBricks);
        //    occupiedPositions.Add(spawnPositionForBricks);
        //}

        // Generate walls from json
        for (int i = 0; i < positionInfo.positionBricksHor.Count; i++)
        {
            Vector2 spawnPositionForBricks = startPosition + new Vector2((positionInfo.positionBricksHor[i].x) * cellSize, -(positionInfo.positionBricksHor[i].y) * cellSize);
            GameObject newObstacle = Instantiate(obstaclePrefab[5], spawnPositionForBricks, Quaternion.identity);
            newObstacle.transform.parent = transform;

            tetrominoTransform.Add(spawnPositionForBricks);
            occupiedPositions.Add(spawnPositionForBricks);
        }

        for (int i = 0; i < positionInfo.positionBricksVer.Count; i++)
        {
            Vector2 spawnPositionForBricks = startPosition + new Vector2((positionInfo.positionBricksVer[i].x) * cellSize, -(positionInfo.positionBricksVer[i].y + 1) * cellSize);
            GameObject newObstacle = Instantiate(obstaclePrefab[7], spawnPositionForBricks, Quaternion.identity);
            newObstacle.transform.parent = transform;

            tetrominoTransform.Add(spawnPositionForBricks);
            occupiedPositions.Add(spawnPositionForBricks);
        }

        for (int i = 0; i < positionInfo.positionBricksJoint.Count; i++)
        {
            Vector2 spawnPositionForBricks = startPosition + new Vector2((positionInfo.positionBricksJoint[i].x) * cellSize, -(positionInfo.positionBricksJoint[i].y) * cellSize);
            GameObject newObstacle = Instantiate(obstaclePrefab[8], spawnPositionForBricks, Quaternion.identity);
            newObstacle.transform.parent = transform;

            tetrominoTransform.Add(spawnPositionForBricks);
            occupiedPositions.Add(spawnPositionForBricks);
        }

        // Generate roofs
        //for (int i = 0; i < positionForRoofs.Count; i++)
        //{
        //    Vector2 spawnPositionForBricks = startPosition + new Vector2((positionForRoofs[i].x + 1) * cellSize, -(positionForRoofs[i].y + 4) * cellSize);
        //    GameObject newObstacle = Instantiate(obstaclePrefab[2], spawnPositionForBricks, Quaternion.identity);
        //    newObstacle.transform.parent = transform;

        //    tetrominoTransform.Add(spawnPositionForBricks);
        //    occupiedPositions.Add(spawnPositionForBricks);
        //}

        // From json
        for (int i = 0; i < positionInfo.positionRoofs.Count; i++)
        {
            Vector2 spawnPositionForBricks = startPosition + new Vector2((positionInfo.positionRoofs[i].x + 1) * cellSize, -(positionInfo.positionRoofs[i].y + 4) * cellSize);
            GameObject newObstacle = Instantiate(obstaclePrefab[2], spawnPositionForBricks, Quaternion.identity);
            newObstacle.transform.parent = transform;

            tetrominoTransform.Add(spawnPositionForBricks);
            occupiedPositions.Add(spawnPositionForBricks);
        }

        // Generate big pits
        //for (int i = 0; i < 6; i++)
        //{
        //    Vector2 spawnPositionForBricks = startPosition + new Vector2((positionForPits[i].x + 1) * cellSize, -(positionForPits[i].y + 4) * cellSize);
        //    GameObject newObstacle = Instantiate(obstaclePrefab[4], spawnPositionForBricks, Quaternion.identity);
        //    newObstacle.transform.parent = transform;

        //    tetrominoTransform.Add(spawnPositionForBricks);
        //    occupiedPositions.Add(spawnPositionForBricks);
        //}

        for (int i = 0; i < 6; i++)
        {
            Vector2 spawnPositionForBricks = startPosition + new Vector2((positionInfo.positionPits[i].x + 1) * cellSize, -(positionInfo.positionPits[i].y + 4) * cellSize);
            GameObject newObstacle = Instantiate(obstaclePrefab[4], spawnPositionForBricks, Quaternion.identity);
            newObstacle.transform.parent = transform;

            tetrominoTransform.Add(spawnPositionForBricks);
            occupiedPositions.Add(spawnPositionForBricks);
        }

        // Generate small pits
        //for (int i = 6; i < positionForPits.Count; i++)
        //{
        //    Vector2 spawnPositionForBricks = startPosition + new Vector2((positionForPits[i].x + 1) * cellSize, -(positionForPits[i].y + 4) * cellSize);
        //    GameObject newObstacle = Instantiate(obstaclePrefab[6], spawnPositionForBricks, Quaternion.identity);
        //    newObstacle.transform.parent = transform;

        //    tetrominoTransform.Add(spawnPositionForBricks);
        //    occupiedPositions.Add(spawnPositionForBricks);
        //}

        for (int i = 6; i < positionInfo.positionPits.Count; i++)
        {
            Vector2 spawnPositionForBricks = startPosition + new Vector2((positionInfo.positionPits[i].x + 1) * cellSize, -(positionInfo.positionPits[i].y + 4) * cellSize);
            GameObject newObstacle = Instantiate(obstaclePrefab[6], spawnPositionForBricks, Quaternion.identity);
            newObstacle.transform.parent = transform;

            tetrominoTransform.Add(spawnPositionForBricks);
            occupiedPositions.Add(spawnPositionForBricks);
        }

        // Generate teleport areas from Json
        for (int i = 0; i < positionInfo.positionBricksTunnelIn.Count; i++)
        {
            Vector2 spawnPositionForBricks = startPosition + new Vector2((positionInfo.positionBricksTunnelIn[i].x + 1) * cellSize, -(positionInfo.positionBricksTunnelIn[i].y + 1) * cellSize);
            GameObject newObstacle = Instantiate(teleportEntryPrefab, spawnPositionForBricks, Quaternion.identity);
            newObstacle.transform.parent = transform;

            tetrominoTransform.Add(spawnPositionForBricks);
            occupiedPositions.Add(spawnPositionForBricks);
        }

        for (int i = 0; i < positionInfo.positionBricksTunnelOut.Count; i++)
        {
            Vector2 spawnPositionForBricks = startPosition + new Vector2((positionInfo.positionBricksTunnelOut[i].x + 1) * cellSize, -(positionInfo.positionBricksTunnelOut[i].y + 1) * cellSize);
            GameObject newObstacle = Instantiate(teleportExitPrefab, spawnPositionForBricks, Quaternion.identity);
            newObstacle.transform.parent = transform;

            tetrominoTransform.Add(spawnPositionForBricks);
            occupiedPositions.Add(spawnPositionForBricks);
        }

        //for (int i = 0; i < positionForTeleInput.Count; i++)
        //{
        //    Vector2 spawnPositionForBricks = startPosition + new Vector2((positionForTeleInput[i].x + 1) * cellSize, -(positionForTeleInput[i].y + 1) * cellSize);
        //    GameObject newObstacle = Instantiate(teleportEntryPrefab, spawnPositionForBricks, Quaternion.identity);
        //    newObstacle.transform.parent = transform;

        //    tetrominoTransform.Add(spawnPositionForBricks);
        //    occupiedPositions.Add(spawnPositionForBricks);
        //}

        //for (int i = 0; i < positionForTeleOutput.Count; i++)
        //{
        //    Vector2 spawnPositionForBricks = startPosition + new Vector2((positionForTeleOutput[i].x + 1) * cellSize, -(positionForTeleOutput[i].y + 1) * cellSize);
        //    GameObject newObstacle = Instantiate(teleportExitPrefab, spawnPositionForBricks, Quaternion.identity);
        //    newObstacle.transform.parent = transform;

        //    tetrominoTransform.Add(spawnPositionForBricks);
        //    occupiedPositions.Add(spawnPositionForBricks);
        //}        

        for (int i = 0; i < cellTransform.Count - 200; i++)
        {
            if (!occupiedPositions.Contains(cellTransform[i].transform.position))
            {
                listOfSpawnPos.Add(cellTransform[i].transform);
            }
        }

        // Save map data
        //string saveFilePath = Path.Combine(Application.streamingAssetsPath, "PlayerData.json");
        //positionInfo.positionBricksHor = positionForBricksHorizontal;
        //positionInfo.positionBricksVer = positionForBricksVertical;
        //positionInfo.positionBricksJoint = positionForBricksJoint;
        //positionInfo.positionBricksTunnelIn = positionForTeleInput;
        //positionInfo.positionBricksTunnelOut = positionForTeleOutput;
        //positionInfo.positionPits = positionForPits;
        //positionInfo.positionRoofs = positionForRoofs;
        //string saveWallPositions = JsonUtility.ToJson(positionInfo);
        //File.WriteAllText(saveFilePath, saveWallPositions);
    }


    bool IsPositionOccupied(Vector2 position, int width, int height)
    {
        Collider[] colliders = Physics.OverlapBox(position, new Vector3(width * cellSize / 10, height * cellSize / 10, 0.1f));

        foreach (Collider collider in colliders)
        {
            if (collider.tag == "Thorn" || collider.tag == "Grass" || collider.tag == "Pit")
            {
                return true;
            }
        }

        return false;
    }

    void DisableExtraCells()
    {
        for (int i = 0; i < cellTransform.Count; i++)
        {
            Vector2 mPos = new Vector2(cellTransform[i].position.x, cellTransform[i].position.y);

            if (tetrominoTransform.Contains(mPos))
            {
                if (cellTransform[i].gameObject.GetComponent<Collider>()) cellTransform[i].gameObject.GetComponent<Collider>().enabled = false;
                cellTransform[i].gameObject.SetActive(false);
            }
        }
    }

    //void GenerateKeyAndFire()
    //{
    //    int rFire = UnityEngine.Random.Range(0, listOfSpawnPos.Count);
    //    GameObject fireClone = Instantiate(GameManager.instance.firePrefab, listOfSpawnPos[rFire].position, Quaternion.identity);
    //    listOfSpawnPos.RemoveAt(rFire);

    //    int rKey = UnityEngine.Random.Range(0, listOfSpawnPos.Count);
    //    GameObject keyClone = Instantiate(GameManager.instance.keyPrefab, listOfSpawnPos[rKey].position, Quaternion.identity);
    //    listOfSpawnPos.RemoveAt(rKey);
    //}

    void ActivatePlayer()
    {
        if (!MainPun.isPlaying)
        {
            int r = UnityEngine.Random.Range(0, listOfSpawnPos.Count);

            GameManager.instance.player.transform.position = listOfSpawnPos[r].position;

            GameManager.instance.player.GetComponent<SpriteMask>().enabled = true;

            listOfSpawnPos.RemoveAt(r);

            MainPun.isPlaying = true;
        }                
    }

    IEnumerator DelayToActivatePlayer()
    {        
        GenerateGridWithObstacles();

        yield return new WaitForSeconds(1f);

        DisableExtraCells();
        
        yield return new WaitForSeconds(0.1f);
        ActivatePlayer();
        isStarted = true;
    }
}
