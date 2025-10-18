using UnityEngine;

public class PlacementCheck : MonoBehaviour {
    public PlacementController placementController;
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D colli;

    void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag(placementController.tagCheckBuilding))
            placementController.canBuild = false;
        else if (collision.CompareTag(placementController.tagCheckGround))
            placementController.canBuild = true;
    }

    void OnTriggerStay2D(Collider2D collision) {
        if (collision.CompareTag(placementController.tagCheckBuilding))
            placementController.canBuild = false;
        else if (collision.CompareTag(placementController.tagCheckGround))
            placementController.canBuild = true;
    }

    void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag(placementController.tagCheckBuilding))
            placementController.canBuild = true;
        else if (collision.CompareTag(placementController.tagCheckGround))
            placementController.canBuild = false;
    }
}