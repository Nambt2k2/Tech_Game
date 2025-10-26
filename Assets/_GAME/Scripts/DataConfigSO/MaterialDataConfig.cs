using UnityEngine;

[CreateAssetMenu(fileName = "MaterialDataConfig", menuName = "DATA/Material Data Config")]
public class MaterialDataConfig : ScriptableObject {
    public E_IDMaterial id;
    public string nameMaterial;
    public Sprite sprite;
}

public enum E_IDMaterial {
    GỖ, ĐÁ, PEG, SỢI, LEN, VẢI, THỎI, DA, ĐINH, HẠT_MÂM_XÔI, NGUYÊN_TỐ_LỬA, NGUYÊN_TỐ_NƯỚC, NGUYÊN_TỐ_BĂNG,
}

[System.Serializable]
public struct S_Material {
    public E_IDMaterial id;
    public int amount;

    public S_Material(E_IDMaterial id, int amount) {
        this.id = id;
        this.amount = amount;
    }
}
