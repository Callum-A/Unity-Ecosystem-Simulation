using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    private Vector3 lastFramePosition;
    private Vector3 currFramePosition;
    private World world => WorldController.Instance.World;

    /// <summary>
    /// Gets the current mouse position as a unity vector3.
    /// </summary>
    /// <returns>Mouse position as a vector 3</returns>
    public Vector3 GetMousePosition()
    {
        // Gets the mouse position in world space
        return currFramePosition;
    }

    /// <summary>
    /// Gets the current mouse position as a tile, can return null if out of bounds.
    /// </summary>
    /// <returns>The tile we are mousing over.</returns>
    public Tile GetMouseoverTile()
    {
        return WorldController.Instance.World.GetTileAt(currFramePosition);
    }

    // Update is called once per frame
    void Update()
    {
        // Set the curr frame position of the mouse
        currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currFramePosition.z = 0;

        UpdateCameraMovement();

        // Set the last frame position, we need to reget the pos due to movement
        lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); // reget position as it has changed due to translate
        lastFramePosition.z = 0;
    }

    /// <summary>
    /// Method to handle camera movement can drag the camera by using either RMB or MMB.
    /// </summary>
    public void UpdateCameraMovement()
    {
        // Handle screen dragging
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2)) // RMB or MMB
        {
            Vector3 diff = lastFramePosition - currFramePosition;
            diff.z = 0;
            Camera.main.transform.Translate(diff);
        }

        Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel");
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 3f, 25f);
    }
}
