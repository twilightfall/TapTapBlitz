using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class GridManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform parentGrid;

    public int columns = 2;
    public int rows = 5;
    public float cellSize = 1.2f;
    public float padding = 1.5f;

    public float[,] grid;

    [SerializeField]
    List<GameObject> gameButtons = new();

    [SerializeField]
    List<Vector3> gridPositions = new();

    public static GridManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        columns = 2;
        rows = 2;
        grid = new float[columns, rows];

        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                Vector3 spawnVector = new(col * padding, row * padding, 0);

                gridPositions.Add(spawnVector);
            }
        }    
    }

    public void SpawnButtons()
    {
        if (gameButtons.Count > 0) return;

        foreach (ButtonPrompt color in System.Enum.GetValues(typeof(ButtonPrompt)))
        {
            GameObject button = Instantiate(buttonPrefab);
            //button.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>($"Sprites/{color}");
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

    public void ClearButtons()
    {
        foreach (GameObject button in gameButtons)
        {
            Destroy(button);
        }

        gameButtons.Clear();
    }

    public void EnableButtons()
    {
        foreach (GameObject button in gameButtons)
        {
            button.SetActive(true);
        }
    }

    public void DisableButtons()
    {
        foreach (GameObject button in gameButtons)
        {
            button.SetActive(false);
        }
    }

    public void ShuffleGrid()
    {
        gridPositions = gridPositions.OrderBy(x => Random.value).ToList();

        for (int count = 0; count < gridPositions.Count; count++)
        {
            gameButtons[count].transform.SetLocalPositionAndRotation(gridPositions[count], Quaternion.identity);
            gameButtons[count].transform.localScale = new(cellSize, cellSize, 1);
        }
    }
}
