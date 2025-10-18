using System;
using UnityEngine;

public class DataManager : MonoBehaviour {
    public static DataManager Ins;
    public bool isLoaded = false;
    public GameSave gameSave;
    public GameSave gameSave_BackUp;

    void OnApplicationPause(bool pause) { if (pause) SaveGame(); }

    void OnApplicationQuit() { SaveGame(); }

    void Awake() {
        if (Ins == null)
            Ins = this;
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
}