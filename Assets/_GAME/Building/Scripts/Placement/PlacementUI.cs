using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class PlacementUI : MonoBehaviour {
    public AllBuildingDataConfig allBuildingDataConfig;
    public PlacementController placementController;
    public ButtonPlacementUI buttonPlacementUIPrefab;
    public Transform tran_btnPlacementParent;
    public Transform tran_objSelectPlacement;
    List<ButtonPlacementUI> list_btnPlacement = new List<ButtonPlacementUI>();
    public GameObject obj_buildingInfo;
    public TextMeshProUGUI tmp_nameBuilding;
    public ButtonItemUI[] arr_buttonMaterialItemUI;

    void OnEnable() {
        List<Building> list_building = allBuildingDataConfig.GetBuildingByLevelTech();
        for (int i = 0; i < list_building.Count; i++) {
            Building building = list_building[i];
            ButtonPlacementUI btn;
            if (i >= list_btnPlacement.Count) {
                btn = Instantiate(buttonPlacementUIPrefab, tran_btnPlacementParent);
                list_btnPlacement.Add(btn);
            } else
                btn = list_btnPlacement[i];
            btn.Init(building, this);
            btn.gameObject.SetActive(true);
        }

        for (int i = list_building.Count; i < list_btnPlacement.Count; i++) {
            list_btnPlacement[i].gameObject.SetActive(false);
        }
    }

    public void ShowBuildingInfo(string name, S_Material[] arr_material, Vector3 posButton) {
        tran_objSelectPlacement.gameObject.SetActive(true);
        tran_objSelectPlacement.position = posButton;
        for (int i = 0; i < arr_buttonMaterialItemUI.Length; i++) {
            if (i < arr_material.Length) {
                arr_buttonMaterialItemUI[i].InitMaterialForBuilding(arr_material[i].id, arr_material[i].amount);
                arr_buttonMaterialItemUI[i].gameObject.SetActive(true);
            } else
                arr_buttonMaterialItemUI[i].gameObject.SetActive(false);
        }
        tmp_nameBuilding.text = Regex.Replace(name.ToString(), @"[_]+", " ");
        obj_buildingInfo.SetActive(true);
    }

    public void MoveBuildingInfo(Vector3 pos) {
        obj_buildingInfo.transform.position = pos;
    }

    public void HideBuildingInfo() {
        obj_buildingInfo.SetActive(false);
        tran_objSelectPlacement.gameObject.SetActive(false);
    }

    public void AnimWarningNotCanBuilding() {
        for (int i = 0; i < arr_buttonMaterialItemUI.Length; i++)
            if (arr_buttonMaterialItemUI[i].gameObject.activeSelf)
                arr_buttonMaterialItemUI[i].AnimWarningNotCanBuilding();
    }
}