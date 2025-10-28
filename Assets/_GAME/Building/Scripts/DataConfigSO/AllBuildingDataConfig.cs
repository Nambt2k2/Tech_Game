using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllBuildingDataConfig", menuName = "DATA/All Building Data Config")]
public class AllBuildingDataConfig : ScriptableObject {
    public Building[] arr_building;

    public List<Building> GetBuildingByLevelTech() {
        List<Building> result = new List<Building>();
        for (int i = 0; i < arr_building.Length; i++)
            if (arr_building[i].techDataConfig.levelTech <= DataManager.ins.gameSave.levelTech)
                result.Add(arr_building[i]);
        return result;
    }
}