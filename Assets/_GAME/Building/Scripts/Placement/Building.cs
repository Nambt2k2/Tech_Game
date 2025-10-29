using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Building : MonoBehaviour {
    public TechDataConfig techDataConfig;
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D colli;
    int indexInData;
    public TextMeshPro tmp_timeBuilding;
    WaitForSeconds waitForSeconds;

    void Init() {
        if (DataManager.ins.gameSave.list_tech[indexInData].state == E_StateTech.BUILDING) {
            if (GetTimeBuilding() <= 0)
                CompleteBuilding();
            else {
                tmp_timeBuilding.text = FormatTime(GetTimeBuilding());
                tmp_timeBuilding.gameObject.SetActive(true);
                waitForSeconds = new WaitForSeconds(1f);
                StartCoroutine(UpdateTimeBuilding());
            }
        } else
            tmp_timeBuilding.gameObject.SetActive(false);
    }

    public void LoadBuilding(Vector3 pos, Transform parent, int index) {
        Building building = Instantiate(this, pos, Quaternion.identity, parent);
        building.indexInData = index;
        building.Init();
    }

    public bool StartBuilding(Vector3 pos, Transform parent) {
        if (DataManager.ins.CheckAmountMaterial(techDataConfig.arr_material)) {
            int index = DataManager.ins.SaveBuilding(new S_Tech(techDataConfig.id, E_StateTech.BUILDING, DateTime.UtcNow.AddSeconds(CalcTimeBuilding()).ToString(), pos));
            for (int i = 0; i < techDataConfig.arr_material.Length; i++)
                DataManager.ins.SetAmountMaterial(techDataConfig.arr_material[i].id, -techDataConfig.arr_material[i].amount);
            MainUI.ins.InitCheatMaterial();
            Building building = Instantiate(this, pos, Quaternion.identity, parent);
            building.indexInData = index;
            building.Init();
        }
        return DataManager.ins.CheckAmountMaterial(techDataConfig.arr_material);
    }

    int CalcTimeBuilding() {
        // time xây đang cố định X5 workload, chờ kịch bản để tính toán lại
        return techDataConfig.workload * 5;
    }

    public void CompleteBuilding() {
        S_Tech tech = DataManager.ins.gameSave.list_tech[indexInData];
        tech.state = E_StateTech.COMPLETED;
        DataManager.ins.SaveBuilding(tech, indexInData);
        tmp_timeBuilding.gameObject.SetActive(false);
    }

    public long GetTimeBuilding() {
        TimeSpan timeDifference = DateTime.Parse(DataManager.ins.gameSave.list_tech[indexInData].timeCompletedBuilding) - DateTime.UtcNow;
        return timeDifference.Ticks / TimeSpan.TicksPerSecond;
    }

    string FormatTime(long totalSeconds) {
        long hours = totalSeconds / 3600;
        long minutes = (totalSeconds % 3600) / 60;
        long seconds = totalSeconds % 60;
        if (hours > 0)
            return $"{hours:00}:{minutes:00}:{seconds:00}";
        else if (minutes > 0)
            return $"{minutes:00}:{seconds:00}";
        else
            return $"{seconds}";
    }

    IEnumerator UpdateTimeBuilding() {
        if (GetTimeBuilding() <= 0)
            CompleteBuilding();
        else {
            tmp_timeBuilding.text = FormatTime(GetTimeBuilding());
            yield return waitForSeconds;
            StartCoroutine(UpdateTimeBuilding());
        }
    }
}