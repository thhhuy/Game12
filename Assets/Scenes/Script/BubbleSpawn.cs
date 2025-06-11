using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSpawn : MonoBehaviour
{
    public GameObject[] bubblePrefabs;       // Mảng chứa các prefab bubble
    public GameObject spawnVFXPrefab;        // Prefab hiệu ứng VFX khi spawn
    public float spawnCooldown = 0.5f;       // Thời gian trễ giữa các lần spawn
    public Collider2D areaSpawn;             // Collider đại diện cho vùng spawn (lồng)

    private float lastSpawnTime = 0f;        // Lưu thời gian lần spawn gần nhất
    private bool isMouseDown = false;        // Kiểm tra nếu chuột đang giữ

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isMouseDown = true;
        }

        if (Input.GetMouseButtonUp(0) && isMouseDown)
        {
            isMouseDown = false;

            Vector2 spawnPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Time.time - lastSpawnTime >= spawnCooldown &&
                areaSpawn != null &&
                areaSpawn.OverlapPoint(spawnPosition))
            {
                SpawnBubble(spawnPosition);
                lastSpawnTime = Time.time;
            }
        }
    }

    private void SpawnBubble(Vector2 position)
    {
        int randomIndex = Random.Range(0, bubblePrefabs.Length);
        GameObject selectedBubble = bubblePrefabs[randomIndex];

        // Tạo instance tạm để lấy đúng kích thước (và scale hiện tại của prefab)
        GameObject tempBubble = Instantiate(selectedBubble);

        SpriteRenderer spriteRenderer = tempBubble.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"Prefab {selectedBubble.name} không có SpriteRenderer!");
            Destroy(tempBubble);
            return;
        }

        Bounds spriteBounds = spriteRenderer.sprite.bounds;
        Vector3 scale = tempBubble.transform.localScale;

        Vector2 bubbleExtents = new Vector2(spriteBounds.extents.x * scale.x, spriteBounds.extents.y * scale.y);
        Vector2 clampedPosition = ClampToArea(position, bubbleExtents);

        Destroy(tempBubble);

        // Spawn bubble thực sự ở vị trí clamped
        GameObject bubble = Instantiate(selectedBubble, clampedPosition, Quaternion.identity);
        bubble.transform.rotation = Quaternion.identity;

        // Hiển thị hiệu ứng VFX nếu có
        if (spawnVFXPrefab != null)
        {
            Instantiate(spawnVFXPrefab, clampedPosition, Quaternion.identity);
        }
    }

    private Vector2 ClampToArea(Vector2 position, Vector2 bubbleExtents)
    {
        Bounds bounds = areaSpawn.bounds;

        float clampedX = Mathf.Clamp(position.x, bounds.min.x + bubbleExtents.x, bounds.max.x - bubbleExtents.x);
        float clampedY = Mathf.Clamp(position.y, bounds.min.y + bubbleExtents.y, bounds.max.y - bubbleExtents.y);

        return new Vector2(clampedX, clampedY);
    }
}
