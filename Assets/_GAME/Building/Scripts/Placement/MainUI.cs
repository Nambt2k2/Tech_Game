using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour {
    public static MainUI ins;

    public PlacementUI placementUI;
    public Button buttonPlacement;
    public GameObject obj_buildingButtonParent;
    public Button buttonCreateItemInBuilding;
    public Button buttonDestroyBuilding;
    public TMP_InputField inputFieldLevelTech;
    public ButtonItemUI[] arr_btnCheatMeterial;


    void Awake() {
        if (ins == null)
            ins = this;
    }

    public void ShowPlacementUI() {
        placementUI.gameObject.SetActive(true);
        placementUI.placementController.ChangeState(E_StatePlacement.ShowBuilding);
        buttonPlacement.gameObject.SetActive(false);
        obj_buildingButtonParent.gameObject.SetActive(false);
    }

    public void HidePlacementUI() {
        placementUI.gameObject.SetActive(false);
        placementUI.HideBuildingInfo();
        buttonPlacement.gameObject.SetActive(true);
        obj_buildingButtonParent.gameObject.SetActive(false);
    }

    public void ShowUISelectBuilding(int indexInData, Action action) {
        buttonPlacement.gameObject.SetActive(false);
        buttonCreateItemInBuilding.gameObject.SetActive(DataManager.ins.GetStateBuilding(indexInData) == E_StateTech.COMPLETED);
        obj_buildingButtonParent.gameObject.SetActive(true);
        obj_buildingButtonParent.transform.position = Input.mousePosition;
        buttonDestroyBuilding.onClick.RemoveAllListeners();
        buttonDestroyBuilding.onClick.AddListener(() => action?.Invoke());
    }

    public void ShowButtonCreateItemInBuilding() {
        buttonCreateItemInBuilding.gameObject.SetActive(true);
    }

    public void HideUISelectBuilding() {
        obj_buildingButtonParent.gameObject.SetActive(false);
        buttonDestroyBuilding.onClick.RemoveAllListeners();
        buttonPlacement.gameObject.SetActive(true);
    }

    public void InitInputFieldLevelTech() {
        inputFieldLevelTech.onValidateInput += ValidateInput;
        inputFieldLevelTech.onEndEdit.RemoveAllListeners();
        inputFieldLevelTech.onEndEdit.AddListener(OnEndEdit);
        inputFieldLevelTech.text = DataManager.ins.gameSave.levelTech.ToString();
    }

    char ValidateInput(string text, int charIndex, char addedChar) {
        // Nếu là dấu -, chặn nó
        if (addedChar == '-')
            return '\0';
        // Chỉ cho phép chữ số
        if (char.IsDigit(addedChar))
            return addedChar;
        // Chặn các ký tự khác
        return '\0';
    }

    void OnEndEdit(string value) {
        if (string.IsNullOrEmpty(value)) {
            inputFieldLevelTech.text = "1";
            return;
        }
        if (int.TryParse(value, out int num)) {
            if (num <= 0)
                inputFieldLevelTech.text = "1";
        } else
            inputFieldLevelTech.text = "1";
    }

    #region Cheat
    public void CheatLevelTech() {
        if (int.TryParse(inputFieldLevelTech.text, out int num)) {
            num = Mathf.Max(1, num);
            DataManager.ins.SetLevelTech(num);
        }
    }

    public void InitUICheatMaterial() {
        for (int i = 0; i < Enum.GetValues(typeof(E_IDMaterial)).Length; i++) {
            if (i >= arr_btnCheatMeterial.Length)
                break;
            arr_btnCheatMeterial[i].InitCheat((E_IDMaterial)i);
        }
    }
    #endregion
}