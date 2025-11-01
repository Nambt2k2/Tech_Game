using System;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour {
    public static DataManager ins;
    public bool isLoaded = false;
    public GameSave gameSave;
    public GameSave gameSave_BackUp;

    void OnApplicationPause(bool pause) { if (pause) SaveGame(); }

    void OnApplicationQuit() { SaveGame(); }

    void Awake() {
        if (ins == null)
            ins = this;
        LoadData();
    }

    public void LoadData() {
        if (!isLoaded) {
            isLoaded = true;
            if (PlayerPrefs.HasKey("GameSave"))
                gameSave = JsonUtility.FromJson<GameSave>(PlayerPrefs.GetString("GameSave"));
            if (gameSave.isNew)
                InitData();
        }
    }

    public void SaveGame() {
        try {
            if (!isLoaded)
                return;
            if (gameSave == null) {
                if (gameSave_BackUp != null) {
                    gameSave = gameSave_BackUp;
                    Debug.LogError("gameSave bị null, backup thành công");
                } else {
                    InitData();
                    Debug.LogError("gameSave bị null, backup không thành công. Reset data");
                }
            }
            gameSave_BackUp = gameSave;
            PlayerPrefs.SetString("GameSave", JsonUtility.ToJson(gameSave));
            PlayerPrefs.Save();
        } catch (Exception ex) {
            Debug.LogError("Lỗi LoadData:" + ex);
        }
    }

    void InitData() {
        gameSave = new GameSave();
        gameSave.isNew = false;
    }

    #region Building
    public void SetLevelTech(int level) {
        gameSave.levelTech = level;
    }

    public int SaveBuilding(S_Tech tech, int index = -1) {
        if (index < 0 || index >= gameSave.list_tech.Count) {
            gameSave.list_tech.Add(tech);
            SaveGame();
            return gameSave.list_tech.Count - 1;
        }
        gameSave.list_tech[index] = tech;
        SaveGame();
        return index;
    }

    public void LoadBuilding(AllBuildingDataConfig allBuildingDataConfig, PlacementController placementController) {
        if (gameSave.list_tech.Count == 0)
            return;
        List<int> list_index = new List<int>();
        for (int i = 0; i < gameSave.list_tech.Count; i++)
            list_index.Add(i);
        for (int indexBuilding = 0; indexBuilding < allBuildingDataConfig.arr_building.Length; indexBuilding++)
            for (int index = list_index.Count - 1; index >= 0; index--)
                if (gameSave.list_tech[list_index[index]].id == allBuildingDataConfig.arr_building[indexBuilding].techDataConfig.id) {
                    allBuildingDataConfig.arr_building[indexBuilding].LoadBuilding(gameSave.list_tech[list_index[index]].pos, placementController, list_index[index]);
                    list_index.RemoveAt(index);
                }
    }

    public void DestroyBuilding(int index) {
        gameSave.list_tech.RemoveAt(index);
    }

    public E_StateTech GetStateBuilding(int index) {
        return gameSave.list_tech[index].state;
    }
    #endregion

    #region Material
    public bool CheckAmountMaterial(S_Material[] arr_material) {
        for (int i = 0; i < arr_material.Length; i++)
            if (arr_material[i].amount > GetAmountMaterial(arr_material[i].id))
                return false;
        return true;
    }

    public void SetAmountMaterial(E_IDMaterial id, int amount) {
        bool isHave = false;
        for (int i = 0; i < gameSave.list_material.Count; i++)
            if (gameSave.list_material[i].id == id) {
                gameSave.list_material[i] = new S_Material(id, gameSave.list_material[i].amount + amount);
                isHave = true;
                break;
            }
        if (!isHave)
            gameSave.list_material.Add(new S_Material(id, amount));
    }

    public int GetAmountMaterial(E_IDMaterial id) {
        for (int i = 0; i < gameSave.list_material.Count; i++)
            if (gameSave.list_material[i].id == id) {
                return gameSave.list_material[i].amount;
            }
        return 0;
    }
    #endregion
}