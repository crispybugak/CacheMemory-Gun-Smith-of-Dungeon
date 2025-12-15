using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartSpawner : MonoBehaviour
{
    [Header("Players in Scene (비활성화 상태로 2개 존재)")]
    [SerializeField] private List<GameObject> players; // size = 2

    [Header("Start Points (4개 Transform 지정)")]
    [SerializeField] private List<Transform> startPoints; // size = 4

    [Header("Options")]
    [SerializeField] private bool alsoApplyNextFrame = true;   // '꼭' 이동 보장용
    [SerializeField] private bool resetRigidbody2DVelocity = true;

    private void Start()
    {
        StartCoroutine(InitPlayerSpawnRoutine());
    }

    private IEnumerator InitPlayerSpawnRoutine()
    {
        int selectedIndex = PlayerPrefs.GetInt("SelectedPlayer", 0);
        selectedIndex = Mathf.Clamp(selectedIndex, 0, players.Count - 1);

        if (players == null || players.Count < 2)
        {
            Debug.LogError("[GameManager] players 리스트에 플레이어 2개를 넣어야 합니다.");
            yield break;
        }

        if (startPoints == null || startPoints.Count == 0)
        {
            Debug.LogError("[GameManager] startPoints 리스트에 스타트 포인트들을 넣어야 합니다.");
            yield break;
        }

        // 1) 플레이어 on/off
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] != null)
                players[i].SetActive(i == selectedIndex);
        }

        GameObject selectedPlayer = players[selectedIndex];
        if (selectedPlayer == null)
        {
            Debug.LogError("[GameManager] 선택된 플레이어 오브젝트가 null입니다.");
            yield break;
        }

        // 2) 스타트 포인트 랜덤 선택
        Transform sp = startPoints[Random.Range(0, startPoints.Count)];
        if (sp == null)
        {
            Debug.LogError("[GameManager] startPoints에 null이 들어있습니다.");
            yield break;
        }

        // 3) 즉시 순간이동 1회
        ForceMove(selectedPlayer, sp);

        // 4) 비활성화->활성화 직후 다른 스크립트(Start/OnEnable)에서 위치를 덮어쓰는 경우가 있어서
        //    한 프레임 뒤에 한 번 더 고정하면 '꼭' 이동이 보장됩니다.
        if (alsoApplyNextFrame)
        {
            yield return null;
            ForceMove(selectedPlayer, sp);
        }
    }

    private void ForceMove(GameObject playerObj, Transform sp)
    {
        Transform t = playerObj.transform;
        t.position = sp.position;
        t.rotation = sp.rotation;


        if (!resetRigidbody2DVelocity) return;

        Rigidbody2D rb = playerObj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}
