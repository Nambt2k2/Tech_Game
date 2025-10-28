using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonPlacementUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public AspectRatioFitter aspectRatioFitter;
    public Button button;
    public Image image;
    Action actionHover, actionExit, actionStayHover;
    RectTransform rectTransform;

    void Update() {
        actionStayHover?.Invoke();
    }

    public void Init(Building building, PlacementUI placementUI) {
        actionStayHover = null;
        image.sprite = building.spriteRenderer.sprite;
        aspectRatioFitter.aspectRatio = image.preferredWidth / image.preferredHeight;
        rectTransform = GetComponent<RectTransform>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {
            if (DataManager.ins.CheckAmountMaterial(building.techDataConfig.arr_material)) {
                placementUI.gameObject.SetActive(false);
                placementUI.placementController.StartMoveBuilding(building);
            } else
                placementUI.AnimWarningNotCanBuilding();
        });
        actionHover = () => {
            placementUI.ShowBuildingInfo(building.techDataConfig.name, building.techDataConfig.arr_material, transform.position);
            actionStayHover = () => {
                Vector2 localMousePos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    rectTransform,
                    Input.mousePosition,
                    null,
                    out localMousePos
                );
                placementUI.MoveBuildingInfo(new Vector3(transform.position.x - 50, transform.position.y + localMousePos.y,transform.position.z));
            };
        };
        actionExit = () => placementUI.HideBuildingInfo();
    }

    public void OnPointerEnter(PointerEventData eventData) {
        actionHover?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData) {
        actionExit?.Invoke();
        actionStayHover = null;
    }
}