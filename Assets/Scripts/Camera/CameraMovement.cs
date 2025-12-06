using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Vector3 offset;
    [SerializeField] private AnimationCurve cameraDelayByDistance;
    [SerializeField] List<CameraBoundaries> boundaries;
    
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        var bottomLeft = _camera.ScreenToWorldPoint(Vector3.zero);
        var topRight = _camera.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight));
        var cameraRect = new Rect(bottomLeft.x, bottomLeft.y, topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
        var cameraWidth = Mathf.Abs(bottomLeft.x - topRight.x);
        var playerPosition = player.transform.position;
        var boundary = boundaries[0];
        for (var i = 0; i < boundaries.Count; i++)
        {
            if (boundaries[i].Height < playerPosition.y)
            {
                boundary = boundaries[i];
                break;
            }
        }    

        playerPosition.x = Mathf.Clamp(playerPosition.x, boundary.LeftBound + cameraWidth / 2f, boundary.RightBound - cameraWidth / 2f);
        playerPosition.z = transform.position.z;
        playerPosition += offset;
        transform.position = playerPosition;
    }


}
