using System;
using UnityEngine;

public class Building : MonoBehaviour {
    public TechDataConfig techDataConfig;
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D colli;
    [HideInInspector] public int indexInData;

    public void LoadBuilding(Vector3 pos, Transform parent, int index) {
        indexInData = index;
        Instantiate(this, pos, Quaternion.identity, parent);
        if (DataManager.ins.gameSave.list_tech[indexInData].state == E_StateTech.BUILDING) {
            // kiểm tra khi người chơi vào lại game và công trình đã xây xong
            if (GetTimeBuilding() <= 0) {
                CompleteBuilding();
            }
        }
    }

    public bool StartBuilding(Vector3 pos, Transform parent) {
        if (DataManager.ins.CheckAmountMaterial(techDataConfig.arr_material)) {
            Instantiate(this, pos, Quaternion.identity, parent);
            // time xây đang cố định theo workload, chờ kịch bản để tính toán lại
            indexInData = DataManager.ins.SaveBuilding(new S_Tech(techDataConfig.id, E_StateTech.BUILDING, DateTime.UtcNow.AddSeconds(techDataConfig.workload).ToString(), pos));
            for (int i = 0; i < techDataConfig.arr_material.Length; i++)
                DataManager.ins.SetAmountMaterial(techDataConfig.arr_material[i].id, -techDataConfig.arr_material[i].amount);
            MainUI.ins.InitCheatMaterial();
        }
        return DataManager.ins.CheckAmountMaterial(techDataConfig.arr_material);
    }

    public void CompleteBuilding() {
        S_Tech tech = DataManager.ins.gameSave.list_tech[indexInData];
        tech.state = E_StateTech.COMPLETED;
        DataManager.ins.SaveBuilding(tech, indexInData);
    }

    public long GetTimeBuilding() {
        TimeSpan timeDifference = DateTime.UtcNow - DateTime.Parse(DataManager.ins.gameSave.list_tech[indexInData].timeCompletedBuilding);
        return timeDifference.Ticks / TimeSpan.TicksPerSecond;
    }
}