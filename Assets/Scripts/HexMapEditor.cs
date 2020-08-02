using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HexMapEditor : MonoBehaviour
{
    // Parameters
    [SerializeField] private Color[] colors;

    // Components
    [SerializeField] private HexGrid hexGrid;
    [SerializeField] private RectTransform togglesContainer;
    [SerializeField] private Toggle toggleColorPrefab;

    // Data
    private Color _activeColor;

    // Life Cycle
    private void Awake()
    {
        SelectColor(0);
        CreateColorToggles();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
            HandleInput();
    }

    // Methods
    private void HandleInput()
    {
        var inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(inputRay, out var hit)) hexGrid.ColorCell(hit.point, _activeColor);
    }

    private void SelectColor(int index)
    {
        _activeColor = colors[index];
    }

    private void CreateColorToggles()
    {
        for (var i = 0; i < colors.Length; i++)
        {
            CreateColorToggle(colors[i], i);
        }
    }

    private void CreateColorToggle(Color color, int index)
    {
        var toggleGroup = togglesContainer.GetComponent<ToggleGroup>();
        var toggle = Instantiate(toggleColorPrefab, togglesContainer);
        var toggleLabel = toggle.GetComponentInChildren<Text>();
        toggleLabel.text = $"Color {index + 1}";
        toggleLabel.color = color;
        toggle.group = toggleGroup;
        toggle.onValueChanged.AddListener((isActive) =>
        {
            if (isActive)
                SelectColor(index);
        });
    }
}