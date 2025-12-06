using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Bottle : MonoBehaviour
{
    public static Bottle Instance;
    public List<Cell> Cells = new List<Cell>();
    public SpriteRenderer[] SpriteRenderers;
    public Color[] BottleColors;
    private Vector3 PosFisrt;
    public int Capacity = 3;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        CreateBottle();
        UpdateSpriteRenderers();

    }
    public void CreateBottle()
    {
        for (int i = 0; i < BottleColors.Length; i++)
        {
            SpriteRenderers[i].color = BottleColors[i];
            Cells.Add(new Cell(BottleColors[i]));
        }
    }
    public void UpdateSpriteRenderers()
    {
        for(int i=0; i < Capacity; i++)
        {
            if (i < Cells.Count)
            {
                SpriteRenderers[i].gameObject.SetActive(true);
                SpriteRenderers[i].color = Cells[i].Color;
            }
            else
            {
                SpriteRenderers[i].gameObject.SetActive(false);
                SpriteRenderers[i].color = Color.clear;
            }
        }
    }
    public bool IsFull => Cells.Count >= Capacity;
    public bool IsSameColor
    {
        get
        {
            for (int i = 0; i < Cells.Count; i++)
            {
                if (Cells[i].Color != Cells[0].Color)
                    return false;
            }
            return true;
        }
    }
    public bool IsEmpty => Cells.Count == 0;
    public Cell TopCell
    {
        get
        {
            if (IsEmpty) return null;
            else
                return Cells[Cells.Count - 1];
        }
    }
    private int CountTopSameColorCells()
    {
        if (IsEmpty) return 0;
        Color topColor = TopCell.Color;
        int count = 0;
        for (int i = Cells.Count - 1; i >= 0; i--)
        {
            if (Cells[i].Color == topColor)
                count++;
            else
                break;
        }
        return count;
    }
    public bool CanPourInto(Bottle o1, Bottle o2)
    {
        if(o1.IsEmpty) return false;
        if(o2.IsFull)return false;
        if(o2.IsEmpty) return true;
        return o1.TopCell.Color == o2.TopCell.Color;
    }
    public void PourInto(Bottle o1, Bottle o2, Action callback)
    {
        if (!CanPourInto(o1, o2))
        {
            callback?.Invoke();
            return;
        }
        else
        {
            if (o1.transform.position.x < o2.transform.position.x)
            {
                PosFisrt = o1.transform.position;
                Vector3 target = new Vector3(o2.transform.position.x - 2.32f, o2.transform.position.y + 1.86f, o2.transform.position.z);
                o1.transform.DOMove(target, 0.7f).OnComplete(() =>
                {
                    o1.transform.DORotate(new Vector3(0, 0, -72f), 0.5f).OnComplete(() =>
                    {
                        int sameCount = CountTopSameColorCells();
                        int space = o2.Capacity - o2.Cells.Count;
                        int moveCount = Math.Min(sameCount, space);
                        for (int i = 0; i < moveCount; i++)
                        {
                            Cell top = TopCell;
                            o1.Cells.RemoveAt(o1.Cells.Count - 1);
                            o2.Cells.Add(top);
                            UpdateSpriteRenderers();
                            o2.UpdateSpriteRenderers();
                            if (o2.IsSameColor&& o2.IsFull)
                            {
                                GameObject prefab = Resources.Load<GameObject>("BOTTLE_02");
                                Instantiate(prefab, o2.transform.position + new Vector3(0, 0.177f, 0), Quaternion.identity);
                                o2.GetComponent<BoxCollider2D>().enabled = false;
                                AudioManager.Instance.PlayMusic(AudioManager.Instance.MusicClose);
                            }
                        }
                        o1.transform.DORotate(new Vector3(0, 0, 0f), 0.5f).OnComplete(() =>
                        {
                            callback?.Invoke();
                        });
                    });
                });
            }
            else
            {
                PosFisrt = o1.transform.position;
                Vector3 target = new Vector3(o2.transform.position.x + 2.32f, o2.transform.position.y + 1.86f, o2.transform.position.z);
                o1.transform.DOMove(target, 0.7f).OnComplete(() =>
                {
                    o1.transform.DORotate(new Vector3(0, 0, 72f), 0.5f).OnComplete(() =>
                    {
                        int sameCount = CountTopSameColorCells();
                        int space = o2.Capacity - o2.Cells.Count;
                        int moveCount = Math.Min(sameCount, space);
                        for (int i = 0; i < moveCount; i++)
                        {
                            Cell top = TopCell;
                            o1.Cells.RemoveAt(o1.Cells.Count - 1);
                            o2.Cells.Add(top);
                            UpdateSpriteRenderers();
                            o2.UpdateSpriteRenderers();
                            if (o2.IsSameColor && o2.IsFull)
                            {
                                GameObject prefab= Resources.Load<GameObject>("BOTTLE_02");
                                Instantiate(prefab, o2.transform.position + new Vector3(0, 0.177f, 0), Quaternion.identity);
                                o2.GetComponent<BoxCollider2D>().enabled = false;
                                AudioManager.Instance.PlayMusic(AudioManager.Instance.MusicClose);
                            }
                        }
                        o1.transform.DORotate(new Vector3(0, 0, 0f), 0.5f).OnComplete(() =>
                        {
                            callback?.Invoke();
                        });
                    });
                });
            }
        }
    }
}
