using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class GridManager : MonoBehaviour
{
    //public GameObject buttonPrefab;
    //public static Transform parentGrid;

    //public static int columns;
    //public  static int rows;
    //public  static float cellSize = 1.2f;
    //public static float padding = 1.5f;

    public static float[,] grid;

    [SerializeField]
    static List<GameObject> gameButtons = new();

    [SerializeField]
    static List<Vector2> gridPositions = new();

    public static void GenerateGrid(int columns, int rows, float padding)
    {
        grid = new float[columns, rows];

        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                Vector2 spawnVector = new(col * padding, row * padding);

                gridPositions.Add(spawnVector);
            }
        }
    }

    public static void SpawnButtons(GameObject buttonPrefab, Transform parentGrid, float cellSize, float padding)
    {
        if (gameButtons.Count > 0) return;

        foreach (ButtonPrompt color in System.Enum.GetValues(typeof(ButtonPrompt)))
        {
            GameObject button = Instantiate(buttonPrefab);
            button.GetComponent<SpriteRenderer>().color = color switch
            {
                ButtonPrompt.RED => Color.red,
                ButtonPrompt.BLUE => Color.blue,
                ButtonPrompt.GREEN => Color.green,
                ButtonPrompt.YELLOW => Color.yellow,
                _ => Color.black
            };
            button.name = color.ToString();
            button.GetComponent<Button>().SetColor(color);
            gameButtons.Add(button);
        }

        gridPositions = gridPositions.OrderBy(x => Random.value).ToList();

        for (int count = 0; count < gridPositions.Count; count++)
        {
            gameButtons[count].transform.SetPositionAndRotation(gridPositions[count], Quaternion.identity);
            gameButtons[count].transform.SetParent(parentGrid, true);
            gameButtons[count].transform.localScale = new(cellSize, cellSize, 1);
        }

        parentGrid.SetPositionAndRotation(new(-padding / 2f, -padding / 2f, 0), Quaternion.identity);
    }

    public static void ClearButtons()
    {
        foreach (GameObject button in gameButtons)
        {
            Destroy(button);
        }

        gameButtons.Clear();
    }

    public static void EnableButtons()
    {
        foreach (GameObject button in gameButtons)
        {
            button.SetActive(true);
        }
    }

    public static void DisableButtons()
    {
        foreach (GameObject button in gameButtons)
        {
            button.SetActive(false);
        }
    }

    public static void ShuffleGrid(float cellSize, float padding)
    {
        gridPositions = gridPositions.OrderBy(x => Random.value).ToList();

        for (int count = 0; count < gridPositions.Count; count++)
        {
            gameButtons[count].transform.SetLocalPositionAndRotation(gridPositions[count], Quaternion.identity);
            gameButtons[count].transform.localScale = new(cellSize, cellSize, 1);
        }
    }

    public static void ShuffleGrid()
    {
        gridPositions = gridPositions.OrderBy(x => Random.value).ToList();

        for (int count = 0; count < gridPositions.Count; count++)
        {
            gameButtons[count].transform.SetLocalPositionAndRotation(gridPositions[count], Quaternion.identity);
            //gameButtons[count].transform.localScale = new(cellSize, cellSize, 1);
        }
    }
}
