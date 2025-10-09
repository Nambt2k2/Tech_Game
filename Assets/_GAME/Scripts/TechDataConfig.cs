using UnityEngine;

[CreateAssetMenu(fileName = "TechDataConfig", menuName = "DATA/Tech Data Config")]
public class TechDataConfig : ScriptableObject {
    public E_IDTech id;
    public string nameTech;
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
    public E_IDMaterial id;
    public int amount;
}

[System.Serializable]
public struct S_TechStat {
    public float attack;
    public float durability;
    public float wear;
    public float weight;
}

public enum E_IDTech {
    BÀN_LÀM_VIỆC_NGUYÊN_THUỶ, RÌU_ĐÁ, CUỐC_ĐÁ, ĐUỐC,
    LÕI_CĂN_CỨ, LƯ_BẮT_LINH_HỒN_LV1, LỬA_TRẠI, RƯƠNG_GỖ, BÀN_SỬA_CHỮA,
    CUNG_TRE, KIẾM_TRE, MŨI_TÊN_TRE, GIƯỜNG_TRE, GIƯỜNG_RƠM, VẢI,
    QUẦN_ÁO_VẢI, HỘP_THỨC_ĂN, CHUÔNG_BÁO_ĐỘNG, KHIÊN,
    ĐỒN_ĐIỀN_TRỒNG_CÂY_MÂM_XÔI, CHUỒNG_GIA_CẦM, CHUỒNG_NUÔI_TẰM, CUNG_LỬA, TÊN_LỬA,
    BÙA_1,
    BÃI_KHAI_THÁC_GỖ, BÃI_KHAI_THÁC_ĐÁ,
    MÁY_NGHIỀN,
    TRANG_PHỤC_CHỐNG_NÓNG_LỬA, TRANG_PHỤC_CHỐNG_NÓNG_BĂNG,
    SUỐI_NƯỚC_NÓNG,
    CUNG_BA_PHÁT, LÒ_NUNG_NGUYÊN_THUỶ, ĐINH, TÚI_ĐỰNG_THỨC_ĂN_NHỎ,
    RÌU_KIM_LOẠI, CUỐC_KIM_LOẠI, BÀN_LÀM_VIỆC_CHẤT_LƯỢNG_CAO,
}

public enum E_TypeAttributeTech {
    THỦ_CÔNG, TRỒNG_TRỌT, TƯỚI_NƯỚC, VẬN_CHUYỂN, TRANG_TRẠI, ĐỐN_GỖ, KHAI_KHOÁNG, ĐỐT_LỬA,
}
