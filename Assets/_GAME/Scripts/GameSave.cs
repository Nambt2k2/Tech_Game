using System;
using System.Collections.Generic;

[Serializable]
public class GameSave {
    public bool isNew = false;

    public int levelTech = 1;
    public List<S_Material> list_material = new List<S_Material>();
    public List<S_Tech> list_tech = new List<S_Tech>();

    public GameSave() {
        isNew = true;
        levelTech = 1;
        list_material = new List<S_Material>();
        list_tech = new List<S_Tech>();

    }
}