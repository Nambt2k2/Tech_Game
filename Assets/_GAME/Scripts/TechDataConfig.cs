using UnityEngine;

[CreateAssetMenu(fileName = "TechDataConfig", menuName = "DATA/Tech Data Config")]
public class TechDataConfig : ScriptableObject {
    public E_NameTech e_name;
    public int levelTech;
    public S_Material[] arr_material;
    public S_TechStat stat;
    public string description;
    public int workload;
    public E_TypeAttributeTech[] arr_typeAttribute;
    public Sprite sprite;
}

[System.Serializable]
public struct S_Material {
    public E_NameMaterial e_name;
    public int amount;
}

[System.Serializable]
public struct S_TechStat {
    public int attack;
    public int durability;
    public int wear;
    public int weight;
}


public enum E_NameTech {

}

public enum E_TypeAttributeTech {
    THỦ_CÔNG, TRỒNG_TRỌT, TƯỚI_NƯỚC, VẬN_CHUYỂN, TRANG_TRẠI, ĐỐN_GỖ, KHAI_KHOÁNG, ĐỐT_LỬA,
}
