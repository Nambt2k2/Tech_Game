using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementController : MonoBehaviour {
    [Header("___________PLACEMENT")]
    public E_StatePlacement statePlacement;
    public AllBuildingDataConfig allBuildingDataConfig;
    public Grid grid;
    public LayerMask groundLayer, buildingLayer;
    public string tagCheckBuilding, tagCheckGround;
    public Color colorPreviewCheckActive, colorPreviewCheckDeactive;
    public int orderLayerPreviewCheckActive, orderLayerPreviewCheckDeactive;
    public SpriteRenderer previewCheck;
    public SpriteRenderer preview;
    public BoxCollider2D colli;
    public float multiSizeColliCheck;
    public CompositeCollider2D compositeColliGround;
    Vector2[][] cachedPaths;
    bool isCacheValid = false;
    bool canBuild;
    Building buildingBuildCur;
    Vector3 mouseWorldPos, mouseWorldPosCheck;
    Vector3 offsetPreviewCheck;
    public List<Building> list_buildingCur = new List<Building>();
    [Header("___________CAMERA")]
    public float speedMoveCam;
    Camera camMain;
    Vector3 dirMoveCam;
    float dirZoomCam;

    #region Placement
    IEnumerator Start() {
        Application.targetFrameRate = 60;
        camMain = Camera.main;
        ResetPlacement();
        yield return new WaitUntil(() => DataManager.ins != null);
        DataManager.ins.LoadBuilding(allBuildingDataConfig, this);
        yield return new WaitUntil(() => MainUI.ins != null);
        MainUI.ins.InitInputFieldLevelTech();
        MainUI.ins.InitUICheatMaterial();
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
                    if (!buildingBuildCur.StartBuilding(mouseWorldPosCheck, this))
                        CancelBuilding();
                break;
            case E_StatePlacement.None:
                if (Input.GetMouseButtonDown(0)) {
                    Collider2D hitCollider = Physics2D.OverlapPoint(camMain.ScreenToWorldPoint(Input.mousePosition), buildingLayer);
                    if (hitCollider != null) {
                        Building building = hitCollider.GetComponent<Building>();
                        if (building != null) {
                            statePlacement = E_StatePlacement.SelectBuilding;
                            MainUI.ins.ShowUISelectBuilding(building.indexInData, () => DestroyBuilding(building));
                        }
                    }
                }
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
        switch (statePlacement) {
            case E_StatePlacement.MoveBuilding:
                if (collision.CompareTag(tagCheckBuilding))
                    canBuild = false;
                else if (collision.CompareTag(tagCheckGround))
                    canBuild = IsBoxCompletelyInsideComposite();
                break;
        }
    }

    void OnTriggerStay2D(Collider2D collision) {
        switch (statePlacement) {
            case E_StatePlacement.MoveBuilding:
                if (collision.CompareTag(tagCheckBuilding))
                    canBuild = false;
                else if (collision.CompareTag(tagCheckGround))
                    canBuild = IsBoxCompletelyInsideComposite();
                break;
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        switch (statePlacement) {
            case E_StatePlacement.MoveBuilding:
                if (collision.CompareTag(tagCheckBuilding))
                    canBuild = true;
                else if (collision.CompareTag(tagCheckGround))
                    canBuild = false;
                break;
        }
    }

    #region Check Placement Logic
    bool IsBoxCompletelyInsideComposite() {
        if (colli == null || compositeColliGround == null)
            return false;
        if (!isCacheValid) {
            CachePaths();
            isCacheValid = true;
        }
        // lấy 4 góc của box
        Bounds boxBounds = colli.bounds;
        Vector2[] boxCorners = {
            new Vector2(boxBounds.min.x, boxBounds.min.y),
            new Vector2(boxBounds.max.x, boxBounds.min.y),
            new Vector2(boxBounds.max.x, boxBounds.max.y),
            new Vector2(boxBounds.min.x, boxBounds.max.y)
        };
        // nếu 1 góc ở ngoài thì không xây được
        for (int i = 0; i < boxCorners.Length; i++)
            if (!IsPointInComposite(boxCorners[i]))
                return false;
        // check thêm collider không giao cắt biên của collider Composite của ground
        if (DoesBoxIntersectPerimeter(boxCorners))
            return false;
        // collider hoàn toàn nằm trong hợp lệ
        return true;
    }

    void CachePaths() {
        // Cache paths các cạnh của Composite colli ground
        int pathCount = compositeColliGround.pathCount;
        cachedPaths = new Vector2[pathCount][];
        for (int i = 0; i < pathCount; i++) {
            int length = compositeColliGround.GetPathPointCount(i);
            cachedPaths[i] = new Vector2[length];
            compositeColliGround.GetPath(i, cachedPaths[i]);
        }
    }

    bool IsPointInComposite(Vector2 point) {
        int pathCount = cachedPaths.Length;
        for (int i = 0; i < pathCount; i++)
            if (IsPointInPolygon(point, cachedPaths[i]))
                return true;
        return false;
    }

    // Ray Casting algorithm: Đếm số lần ray từ điểm cắt edge (odd = inside)
    bool IsPointInPolygon(Vector2 point, Vector2[] polygon) {
        int n = polygon.Length;
        bool inside = false;
        for (int i = 0, j = n - 1; i < n; j = i++) {
            Vector2 a = polygon[i];
            Vector2 b = polygon[j];
            if ((a.y > point.y) != (b.y > point.y) &&
                (point.x < a.x + (b.x - a.x) * (point.y - a.y) / (b.y - a.y + 0.0001f))) {  // Tránh chia 0
                inside = !inside;
            }
        }
        return inside;
    }

    // kiểm tra 4 cạnh Box có cắt tất cả edge Composite
    bool DoesBoxIntersectPerimeter(Vector2[] boxCorners) {
        // 4 cạnh của Box: (0-1, 1-2, 2-3, 3-0)
        for (int i = 0; i < 4; i++) {
            Vector2 start = boxCorners[i];
            Vector2 end = boxCorners[(i + 1) % 4];
            if (DoesLineIntersectAnyEdge(start, end)) {
                return true;
            }
        }
        return false;
    }

    // Kiểm tra line segment có cắt bất kỳ edge nào của Composite
    bool DoesLineIntersectAnyEdge(Vector2 lineStart, Vector2 lineEnd) {
        foreach (Vector2[] path in cachedPaths) {
            int n = path.Length;
            for (int i = 0; i < n; i++) {
                Vector2 edgeStart = path[i];
                Vector2 edgeEnd = path[(i + 1) % n];
                if (LineSegmentsIntersect(lineStart, lineEnd, edgeStart, edgeEnd)) {
                    return true;
                }
            }
        }
        return false;
    }

    // Line segment intersection (dựa trên orientation method)
    bool LineSegmentsIntersect(Vector2 p1, Vector2 q1, Vector2 p2, Vector2 q2) {
        float orient1 = Orientation(p1, q1, p2);
        float orient2 = Orientation(p1, q1, q2);
        float orient3 = Orientation(p2, q2, p1);
        float orient4 = Orientation(p2, q2, q1);

        if (orient1 * orient2 < 0 && orient3 * orient4 < 0)
            return true;  // General case
        // Special cases (collinear và overlap) - bỏ qua nếu không cần chính xác cao
        return false;
    }

    float Orientation(Vector2 p, Vector2 q, Vector2 r) {
        float val = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);
        if (val == 0)
            return 0;  // Collinear
        return val > 0 ? 1 : 2;  // Clockwise or Counterclockwise
    }
    #endregion

    void ResetPlacement() {
        statePlacement = E_StatePlacement.None;
        colli.enabled = false;
        preview.enabled = false;
        previewCheck.enabled = false;
        buildingBuildCur = null;
    }
    #endregion

    public void StartMoveBuilding(Building building) {
        statePlacement = E_StatePlacement.MoveBuilding;
        buildingBuildCur = building;
        colli.offset = building.colli.offset;
        colli.size = building.colli.size * multiSizeColliCheck;
        colli.enabled = true;
        preview.sprite = building.spriteRenderer.sprite;
        if (building.spriteRenderer.drawMode == SpriteDrawMode.Sliced) {
            preview.drawMode = SpriteDrawMode.Sliced;
            preview.size = building.spriteRenderer.size;
        }
        preview.enabled = true;
        previewCheck.size = new Vector2(building.colli.size.x, building.colli.size.y);
        offsetPreviewCheck = new Vector3(building.colli.offset.x, building.colli.offset.y);
        previewCheck.transform.localPosition += offsetPreviewCheck;
        previewCheck.enabled = true;
    }

    public void CancelBuilding() {
        ResetPlacement();
        MainUI.ins.HidePlacementUI();
    }

    public void DestroyBuilding(Building building) {
        // xử lý index của các building khác đang có trước khi xóa building
        for (int i = 0; i < list_buildingCur.Count; i++)
            if (list_buildingCur[i].indexInData > building.indexInData)
                list_buildingCur[i].indexInData--;
        list_buildingCur.Remove(building);
        building.DestroyBuilding();
        statePlacement = E_StatePlacement.None;
        MainUI.ins.HideUISelectBuilding();
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

    public void ChangeState(E_StatePlacement state) {
        statePlacement = state;
    }
}

public enum E_StatePlacement {
    None, ShowBuilding ,MoveBuilding, SelectBuilding
}