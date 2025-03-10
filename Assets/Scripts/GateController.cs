using UnityEngine;
using System.Collections;
public class GateController : MonoBehaviour
{
    public new GameObject gameObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
        // Bắt đầu Coroutine khi scene chạy
        StartCoroutine(ActiveAfterDelay());
    }

    public IEnumerator ActiveAfterDelay()
    {
        // Đợi giây
        yield return new WaitForSeconds(10f);
        // Kích hoạt GameObject
        gameObject.SetActive(true);
    }
}
