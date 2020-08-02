using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    // Parameters
    [SerializeField] private int width = 6;
    [SerializeField] private int height = 6;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color touchedColor = Color.magenta;

    // Components
    [SerializeField] private HexCell hexCellPrefab;
    [SerializeField] private Text hexCellLabelPrefab;

    // Component Members
    private Canvas _gridCanvas;
    private HexMesh _hexMesh;

    // Data
    private HexCell[] _hexCells;

    // Life Cycle
    private void Awake()
    {
        _gridCanvas = GetComponentInChildren<Canvas>();
        _hexMesh = GetComponentInChildren<HexMesh>();
        _hexCells = CreateCellGrid(width, height);
        CreateCellLabels();
    }

    private void Start()
    {
        _hexMesh.Triangulate(_hexCells);
    }

    // Interfaces
    public void ColorCell(Vector3 position, Color color)
    {
        position = transform.InverseTransformPoint(position);
        var coordinates = HexCoordinates.FromPosition(position);
        var index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
        var cell = _hexCells[index];
        cell.color = color;
        _hexMesh.Triangulate(_hexCells);
    }

    // Methods
    private HexCell[] CreateCellGrid(int width, int height)
    {
        var hexCells = new HexCell[height * width];
        for (int z = 0, i = 0; z < height; z++)
        {
            for (var x = 0; x < width; x++)
            {
                hexCells[i] = CreateCell(x, z, i++);
            }
        }

        return hexCells;
    }

    private HexCell CreateCell(int x, int z, int index)
    {
        var position = new Vector3(
            (x + z * 0.5f - z / 2) * HexMetrics.InnerRadius * 2f,
            0f,
            z * HexMetrics.OuterRadius * 1.5f);
        var hexCell = Instantiate(hexCellPrefab, transform);
        hexCell.transform.localPosition = position;
        hexCell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        hexCell.color = defaultColor;
        return hexCell;
    }

    private void CreateCellLabels()
    {
        foreach (var hexCell in _hexCells)
        {
            CreateCellLabel(hexCell);
        }
    }

    private void CreateCellLabel(HexCell hexCell)
    {
        var label = Instantiate(hexCellLabelPrefab, _gridCanvas.transform);
        var position = hexCell.transform.position;
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = hexCell.coordinates.ToStringOnSeparateLines();
    }
}