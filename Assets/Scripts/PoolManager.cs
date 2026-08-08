using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    public GameObject donPrefab;
    public GameObject katPrefab;
    public GameObject bigDonPrefab;
    public GameObject bigKatPrefab;

    public Queue<Note> donPrefabs = new();
    public Queue<Note> katPrefabs = new();
    public Queue<Note> bigDonPrefabs = new();
    public Queue<Note> bigKatPrefabs = new();

    public List<Queue<Note>> PrefabsList = new();

    private void Awake()
    {
        Instance = this;

        PrefabsList.Add(new Queue<Note>()); // idx 맞추기 위해 빈 큐 생성
        PrefabsList.Add(donPrefabs);
        PrefabsList.Add(katPrefabs);
        PrefabsList.Add(bigDonPrefabs);
        PrefabsList.Add(bigKatPrefabs);
        InitQueue();
    }

    private void InitQueue()
    {
        for (int i = 0; i < 150; i++)
        {
            GameObject go1 = Instantiate(donPrefab);
            donPrefabs.Enqueue(go1.GetComponent<Note>());
            go1.transform.SetParent(transform);
            go1.SetActive(false);

            GameObject go2 = Instantiate(katPrefab);
            katPrefabs.Enqueue(go2.GetComponent<Note>());
            go2.transform.SetParent(transform);
            go2.SetActive(false);

            GameObject go3 = Instantiate(bigDonPrefab);
            bigDonPrefabs.Enqueue(go3.GetComponent<Note>());
            go3.transform.SetParent(transform);
            go3.SetActive(false);

            GameObject go4 = Instantiate(bigKatPrefab);
            bigKatPrefabs.Enqueue(go4.GetComponent<Note>());
            go4.transform.SetParent(transform);
            go4.SetActive(false);
        }
    }

    public Note GetFromPool(Transform spawnPoint, int idx)
    {
        Note note = PrefabsList[idx].Dequeue();
        note.transform.position = spawnPoint.position;
        note.transform.localScale = Vector3.one * GameManager.Instance.appliedNoteScale;
        note.gameObject.SetActive(true);
        
        return note;
    }

    public void ReturnToPool(Note note, int idx)
    {
        PrefabsList[idx].Enqueue(note);
        note.gameObject.SetActive(false);
    }

    public void ReturnToPoolAll()
    {
        Note[] notes = FindObjectsByType<Note>(FindObjectsSortMode.None);

        foreach (Note note in notes)
        {
            if (note.gameObject.activeSelf) { ReturnToPool(note, (int)note.type); }
        }
    }
}