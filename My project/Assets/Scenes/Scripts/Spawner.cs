using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public float spawnRate = 1f;
    public float spawnOffset = 6f;

    private bool isSpawning = false; // Biến để kiểm tra liệu có đang spawn enemy hay không

    private void OnEnable()
    {
        isSpawning = true; // Bắt đầu spawn khi đối tượng được kích hoạt
        InvokeRepeating(nameof(Spawn), spawnRate, spawnRate);
    }

    private void OnDisable()
    {
        isSpawning = false; // Dừng spawn khi đối tượng bị tắt
    }

    private void Spawn()
    {
        if (!isSpawning) return; // Kiểm tra nếu không được phép spawn, thì thoát ra

        // Tính toán vị trí spawn mới với offset
        Vector3 spawnPosition = new Vector3(transform.position.x + spawnOffset, transform.position.y, transform.position.z);

        // Tạo đối tượng enemy tại vị trí đã tính toán
        GameObject enemy = Instantiate(prefab, spawnPosition, Quaternion.identity);

        // Gán Collider của đối tượng quái vật là một Trigger
        enemy.GetComponent<Collider2D>().isTrigger = true;
    }
}
