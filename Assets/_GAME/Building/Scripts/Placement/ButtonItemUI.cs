using DG.Tweening;
using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonItemUI : MonoBehaviour {
    public TextMeshProUGUI tmp_nameItem;
    public TextMeshProUGUI tmp_amountItem;
    public Button button;
    Action actionWarning;
    Tween tweenWarning;

    public void InitCheat(E_IDMaterial idMaterial) {
        tmp_nameItem.text = Regex.Replace(idMaterial.ToString(), @"[_]+", " ");
        tmp_amountItem.text = DataManager.ins.GetAmountMaterial(idMaterial).ToString();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {
            DataManager.ins.SetAmountMaterial(idMaterial, 10);
            tmp_amountItem.text = DataManager.ins.GetAmountMaterial(idMaterial).ToString();
        });
    }

    public void InitMaterialForBuilding(E_IDMaterial idMaterial, int amount) {
        actionWarning = null;
        tmp_nameItem.text = Regex.Replace(idMaterial.ToString(), @"[_]+", " ");
        int amountHave = DataManager.ins.GetAmountMaterial(idMaterial);
        tmp_amountItem.text = $"{amountHave} / {amount}";
        if (amountHave >= amount) {
            tmp_amountItem.color = Color.green;
            return;
        }
        tmp_amountItem.color = Color.red;
        actionWarning = () => {
            if (tweenWarning != null && tweenWarning.IsActive() && tweenWarning.IsPlaying())
                return;
            transform.localScale = Vector3.one;
            transform.DOScale(1.3f, .2f).SetEase(Ease.OutBack);
            tweenWarning = transform.DOScale(1f, .2f).SetEase(Ease.InBack).SetDelay(.2f);
        };
    }

    public void AnimWarningNotCanBuilding() {
        actionWarning?.Invoke();
    }
}