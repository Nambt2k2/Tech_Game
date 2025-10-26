using UnityEngine;

public class Building : MonoBehaviour {
    public TechDataConfig techDataConfig;
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D colli;
    [HideInInspector] public int indexInData;


    public void LoadBuilding(Vector3 pos, Transform parent, int index) {
        indexInData = index;
        Instantiate(this, pos, Quaternion.identity, parent);
    }

    public bool StartBuilding(Vector3 pos, Transform parent) {
        if (DataManager.ins.CheckAmountMaterial(techDataConfig.arr_material)) {
            Instantiate(this, pos, Quaternion.identity, parent);
            // cần thêm time
            indexInData = DataManager.ins.SaveBuilding(new S_Tech(techDataConfig.id, E_StateTech.BUILDING, "none", pos));
            for (int i = 0; i < techDataConfig.arr_material.Length; i++)
                DataManager.ins.SetAmountMaterial(techDataConfig.arr_material[i].id, -techDataConfig.arr_material[i].amount);
            MainUI.ins.InitCheatMaterial();
        }
        return DataManager.ins.CheckAmountMaterial(techDataConfig.arr_material);
    }
}