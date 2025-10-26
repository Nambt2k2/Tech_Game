using System.Collections;
using UnityEngine;

public class PlacementController : MonoBehaviour {
    [Header("___________PLACEMENT")]
    public E_StatePlacement statePlacement;
    public AllBuildingDataConfig allBuildingDataConfig;
    public Grid grid;
    public LayerMask groundLayer;
    public string tagCheckBuilding, tagCheckGround;
    public Color colorPreviewCheckActive, colorPreviewCheckDeactive;
    public int orderLayerPreviewCheckActive, orderLayerPreviewCheckDeactive;
    public SpriteRenderer previewCheck;
    public SpriteRenderer preview;
    public BoxCollider2D colli;
    public float multiSizeColliCheck;
    bool canBuild;
    Building building;
    Vector3 mouseWorldPos, mouseWorldPosCheck;
    Vector3 offsetPreviewCheck;
    [Header("___________CAMERA")]
    public float speedMoveCam;
    Camera camMain;
    Vector3 dirMoveCam;
    float dirZoomCam;

    IEnumerator Start() {
        Application.targetFrameRate = 60;
        camMain = Camera.main;
        Reset();
        yield return new WaitUntil(() => DataManager.ins != null);
        DataManager.ins.LoadBuilding(allBuildingDataConfig, grid.transform);
        yield return new WaitUntil(() => MainUI.ins != null);
        MainUI.ins.InitInputFieldLevelTech();
        MainUI.ins.InitCheatMaterial();
    }
    
    void Update() {
        // di chuyển cam
        MoveCam();
        // check hủy xây dựng
        if (Input.GetKeyDown(KeyCode.Escape))
            CancelBuilding();
        else if (Input.GetKeyDown(KeyCode.B))
            MainUI.ins.ShowPlacementUI();
        switch (statePlacement) {
            case E_StatePlacement.MoveBuilding:
                // di chuyển công trình theo chuột
                mouseWorldPos = grid.GetCellCenterWorld(grid.WorldToCell(camMain.ScreenToWorldPoint(Input.mousePosition)));
                mouseWorldPos.z = grid.transform.position.z;
                preview.transform.position = mouseWorldPos;
                // xây building tại vị đã check có thể build
                if (Input.GetMouseButtonDown(0) && canBuild)
                    if (!building.StartBuilding(mouseWorldPosCheck, grid.transform))
                        CancelBuilding();
                break;
        }
    }

    void FixedUpdate() {
        // tính toán vị trí để đặt preview check
        switch (statePlacement) {
            case E_StatePlacement.MoveBuilding:
                mouseWorldPosCheck = mouseWorldPos;
                previewCheck.transform.position = mouseWorldPosCheck + offsetPreviewCheck;
                if (canBuild) {
                    previewCheck.color = colorPreviewCheckActive;
                    previewCheck.sortingOrder = orderLayerPreviewCheckActive;
                } else {
                    previewCheck.color = colorPreviewCheckDeactive;
                    previewCheck.sortingOrder = orderLayerPreviewCheckDeactive;
                }
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag(tagCheckBuilding))
            canBuild = false;
        else if (collision.CompareTag(tagCheckGround))
            canBuild = true;
    }

    void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag(tagCheckBuilding))
            canBuild = false;
        else if (collision.CompareTag(tagCheckGround))
            canBuild = true;
    }

    void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag(tagCheckBuilding))
            canBuild = true;
        else if (collision.CompareTag(tagCheckGround))
            canBuild = false;
    }

    void Reset() {
        statePlacement = E_StatePlacement.None;
        colli.enabled = false;
        preview.enabled = false;
        previewCheck.enabled = false;
        building = null;
    }

    public void StartMoveBuilding(Building building) {
        statePlacement = E_StatePlacement.MoveBuilding;
        this.building = building;
        colli.offset = building.colli.offset;
        colli.size = building.colli.size * multiSizeColliCheck;
        colli.enabled = true;
        preview.sprite = building.spriteRenderer.sprite;
        preview.enabled = true;
        previewCheck.size = new Vector2(building.colli.size.x, building.colli.size.y);
        offsetPreviewCheck = new Vector3(building.colli.offset.x, building.colli.offset.y);
        previewCheck.transform.localPosition += offsetPreviewCheck;
        previewCheck.enabled = true;
    }

    public void CancelBuilding() {
        Reset();
        MainUI.ins.HidePlacementUI();
    }

    public void MoveCam() {
        dirMoveCam = Vector3.zero;
        dirZoomCam = 0;
        if (Input.GetKey(KeyCode.W))
            dirMoveCam += Vector3.up;
        if (Input.GetKey(KeyCode.S))
            dirMoveCam += Vector3.down;
        if (Input.GetKey(KeyCode.A))
            dirMoveCam += Vector3.left;
        if (Input.GetKey(KeyCode.D))
            dirMoveCam += Vector3.right;
        if (Input.GetKey(KeyCode.Q))
            dirZoomCam = 1;
        if (Input.GetKey(KeyCode.E))
            dirZoomCam = -1;
        camMain.transform.position = Vector3.Lerp(camMain.transform.position, camMain.transform.position + dirMoveCam.normalized, speedMoveCam * Time.deltaTime);
        camMain.orthographicSize = Mathf.Lerp(camMain.orthographicSize, camMain.orthographicSize + dirZoomCam, speedMoveCam * Time.deltaTime);
    }
}

public enum E_StatePlacement {
    None, MoveBuilding,
}