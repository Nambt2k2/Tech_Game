using System.Collections.Generic;
using UnityEngine;

public class PlacementController : MonoBehaviour {
    [Header("___________PLACEMENT")]
    public AllBuildingDataConfig allBuildingDataConfig;
    public Grid grid;
    public bool isGridCheck;
    public float sizeGridCheck;
    public LayerMask groundLayer;
    public string tagCheckBuilding, tagCheckGround;
    public SpriteRenderer preview;
    public SpriteRenderer previewCheck;
    public Color colorPreviewCheckActive, colorPreviewCheckDeactive;
    public int orderLayerPreviewCheckActive, orderLayerPreviewCheckDeactive;
    public PlacementCheck placementCheck;
    public bool canBuild;
    Building building;
    Vector3 mouseWorldPos, mouseWorldPosCheck;
    [Header("___________CAMERA")]
    public float speedMoveCam;
    Camera camMain;
    Vector3 dirMoveCam;

    #region UNITY
    void Start() {
        Application.targetFrameRate = 60;
        camMain = Camera.main;
        grid.cellSize = new Vector3(sizeGridCheck, sizeGridCheck, 0);
    }

    void Update() {
        // di chuyển cam
        MoveCam();
        // di chuyển công trình theo chuột
        mouseWorldPos = camMain.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = grid.transform.position.z;
        preview.transform.position = mouseWorldPos;
        // xây building tại vị đã check có thể build
        if (Input.GetMouseButtonDown(0) && canBuild)
            Instantiate(building, mouseWorldPosCheck, Quaternion.identity);
    }

    void FixedUpdate() {
        // tính toán vị trí để đặt preview check
        mouseWorldPosCheck = isGridCheck ? grid.GetCellCenterWorld(grid.WorldToCell(mouseWorldPos)) : mouseWorldPos;
        previewCheck.transform.position = mouseWorldPosCheck;
        if (canBuild) {
            previewCheck.color = colorPreviewCheckActive;
            previewCheck.sortingOrder = orderLayerPreviewCheckActive;
        } else {
            previewCheck.color = colorPreviewCheckDeactive;
            previewCheck.sortingOrder = orderLayerPreviewCheckDeactive;
        }
    }
    #endregion

    #region Placement
    public List<Building> GetBuildingByLevelTech() {
        List<Building> result = new List<Building>();
        for (int i = 0; i < allBuildingDataConfig.arr_building.Length; i++)
            if (allBuildingDataConfig.arr_building[i].techDataConfig.levelTech <= DataManager.Ins.gameSave.levelTech)
                result.Add(allBuildingDataConfig.arr_building[i]);
        return result;
    }

    public void SetBuilldingActiveCur(Building building) {
        this.building = building;
        //placementCheck.spriteRenderer.sprite;
        placementCheck.colli.offset = building.colli.offset;
        placementCheck.colli.size = building.colli.size;
        placementCheck.gameObject.SetActive(true);
        preview.sprite = building.spriteRenderer.sprite;
    }
    #endregion

    #region Cam
    public void MoveCam() {
        dirMoveCam = Vector3.zero;
        if (Input.GetKey(KeyCode.UpArrow))
            dirMoveCam += Vector3.up;
        if (Input.GetKey(KeyCode.DownArrow))
            dirMoveCam += Vector3.down;
        if (Input.GetKey(KeyCode.LeftArrow))
            dirMoveCam += Vector3.left;
        if (Input.GetKey(KeyCode.RightArrow))
            dirMoveCam += Vector3.right;
        camMain.transform.position = Vector3.Lerp(camMain.transform.position, camMain.transform.position + dirMoveCam.normalized, speedMoveCam * Time.deltaTime);
    }
    #endregion

}