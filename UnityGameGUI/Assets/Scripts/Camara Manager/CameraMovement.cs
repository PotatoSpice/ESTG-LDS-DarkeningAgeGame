using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField]
    private SpriteRenderer mapRenderer;

    private float targetZoom;
    private float zoomFactor = 3f;
    private float zoomLerpSpeed = 10;

    private float mapMinX, mapMaxX, mapMinY, mapMaxY;

    private Vector3 originPoint;

    private void Start()
    {
        targetZoom = cam.orthographicSize;
    }

    private void Awake()
    {
        mapMinX = mapRenderer.transform.position.x - mapRenderer.bounds.size.x / 1.5f;
        mapMaxX = mapRenderer.transform.position.x + mapRenderer.bounds.size.x / 1.5f;

        mapMinY = mapRenderer.transform.position.y - mapRenderer.bounds.size.y / 1.5f;
        mapMaxY = mapRenderer.transform.position.y + mapRenderer.bounds.size.y / 1.5f;
    }

    void Update()
    {
        if (Utils.CheckMouseOverUI())
        {
            MoveCamera();
        

        float scrollData;
        scrollData = Input.GetAxis("Mouse ScrollWheel");

        targetZoom = targetZoom - scrollData * zoomFactor;
        targetZoom = Mathf.Clamp(targetZoom, 4f, 8f);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomLerpSpeed);
        cam.transform.position = ClampCamera(cam.transform.position);
        }
    }

    private void MoveCamera()
    {
        if (Input.GetMouseButtonDown(0))
        {
            originPoint = cam.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 difference = originPoint - cam.ScreenToWorldPoint(Input.mousePosition);

            cam.transform.position = ClampCamera(cam.transform.position + difference);
        }
    }

    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        float camHeight = cam.orthographicSize;
        float camWidth = cam.orthographicSize * cam.aspect;

        float minX = mapMinX + camWidth;
        float maxX = mapMaxX - camWidth;
        float minY = mapMinY + camHeight;
        float maxY = mapMaxY - camHeight;

        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newY = Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(newX, newY, targetPosition.z);
    }
}
