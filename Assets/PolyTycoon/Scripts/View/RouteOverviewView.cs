using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RouteOverviewView : MonoBehaviour
{
    public RectTransform _visibleObject;
    public Button _exitButton;
    public Button _showButton;

    public Button _sortingAllButton;
    public Button _sortingRoadButton;
    public Button _sortingRailButton;
    public Button _sortingSeaButton;
    public Button _sortingAirButton;

    public TMP_Text _searchResultInfo;
    public FoldableList _foldableList;
}
