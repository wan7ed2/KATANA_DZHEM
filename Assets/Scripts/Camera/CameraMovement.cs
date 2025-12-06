using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] List<CameraBoundaries> boundaries;
    
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        var cameraRect = GetCameraRect();
        var targetPosition = target.transform.position;
        var currentBoundary = GetCurrentCoundaries(targetPosition.y - cameraRect.height / 2f);

        targetPosition.x = Mathf.Clamp(targetPosition.x, currentBoundary.LeftBound + cameraRect.width / 2f, currentBoundary.RightBound - cameraRect.width / 2f);
        targetPosition.z = transform.position.z;
        targetPosition += offset;
        
        transform.position = targetPosition;
    }

    private Rect GetCameraRect()
    {
        var bottomLeft = _camera.ScreenToWorldPoint(Vector3.zero);
        var topRight = _camera.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight));
        return new Rect(bottomLeft.x, bottomLeft.y, topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
    }

    private CameraBoundaries GetCurrentCoundaries(float currentY)
    {
        if (boundaries[0].Height < currentY)
            return boundaries[0];

        for (var i = 0; i < boundaries.Count; i++)
        {
            if (boundaries[i].Height < currentY)
                return boundaries[i];
        }

        return boundaries[^1];
    }

    [ExecuteAlways]
    private void OnDrawGizmos()
    {
        var boundary = boundaries[0];
        var left = new Vector3(boundary.LeftBound, boundary.Height, 0);
        var right = new Vector3(boundary.RightBound, boundary.Height, 0);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(left, Vector3.up * 100f);
        Gizmos.DrawRay(right, Vector3.up * 100f);
        for (var i = 1; i < boundaries.Count; i++)
        {
            var leftTop = new Vector3(boundaries[i].LeftBound, boundaries[i - 1].Height, 0);
            var rightTop = new Vector3(boundaries[i].RightBound, boundaries[i - 1].Height, 0);

            var leftBottom = new Vector3(boundaries[i].LeftBound, boundaries[i].Height, 0);
            var rightBottom = new Vector3(boundaries[i].RightBound, boundaries[i].Height, 0);

            Gizmos.DrawLine(leftTop, leftBottom);
            Gizmos.DrawLine(rightTop, rightBottom);
        }

        boundary = boundaries[^1];
        left = new Vector3(boundary.LeftBound, boundary.Height, 0);
        right = new Vector3(boundary.RightBound, boundary.Height, 0);
        Gizmos.DrawRay(left, Vector3.down * 100f);
        Gizmos.DrawRay(right, Vector3.down * 100f);
    }
}
