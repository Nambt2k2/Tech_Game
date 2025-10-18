using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacementUI : MonoBehaviour {
    public PlacementController placementController;
    public Button btn_placementPrefab;
    public Transform tran_btnPlacementParent;
    List<Button> list_btnPlacement = new List<Button>();

    void Start() {
        list_btnPlacement.Clear();
        List<Building> list_building = placementController.GetBuildingByLevelTech();
        for (int i = 0; i < list_building.Count; i++) {
            Building building = list_building[i];
            Button btn = Instantiate(btn_placementPrefab, tran_btnPlacementParent);
            btn.image.sprite = building.spriteRenderer.sprite;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => placementController.SetBuilldingActiveCur(building));
        }
    }
}