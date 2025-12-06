using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public GameObject Ui_Win;
    public GameObject Ui_Lose;
    public List<Bottle> Bottles = new List<Bottle>();
    private Camera m_cam;
    private Bottle bottle;
    private RaycastHit2D Firsthit;
    private Vector3 PosFirst;
    private int count = 0;
    public bool Canpress;
    void Start()
    {
        m_cam = Camera.main;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(!Canpress)
            {
                var hit = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit.collider != null)
                {
                    Firsthit = hit;
                    bottle = Firsthit.collider.GetComponent<Bottle>();
                    PosFirst = Firsthit.collider.transform.position;
                    Firsthit.transform.DOMoveY(PosFirst.y + 0.8f, 0.2f);
                    Debug.Log("Selected " + hit.collider.name);
                    Canpress = true;
                    return;
                }
                else
                {
                    Canpress = false;
                }
            }
            if (Canpress)
            {
                var hit1 = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit1.collider != null)
                {
                    if(Firsthit.collider != hit1.collider)
                    {
                        Bottle o1 = Firsthit.collider.GetComponent<Bottle>();
                        Bottle o2 = hit1.collider.GetComponent<Bottle>();
                        bottle.PourInto(o1, o2, () =>
                        {
                            Firsthit.transform.DOMove(PosFirst, 0.2f);
                            Check_Win();
                            Check_Lose();
                            count = 0;
                            Canpress = false;
                        }
                        );
                    }
                    else
                    {
                        Firsthit.transform.DOMoveY(PosFirst.y, 0.2f);
                        Canpress = false;
                    }
                }
            }
        }

    }
    private void Check_Win()
    {
        for(int i=0; i < Bottles.Count; i++)
        {
            if((Bottles[i].IsFull && Bottles[i].IsSameColor) || Bottles[i].IsEmpty)
            {
                count++;
                if(count == Bottles.Count)
                {
                    AudioManager.Instance.PlayMusic(AudioManager.Instance.MusicFull);
                    Ui_Win.SetActive(true);
                }
            }
        }
    }
    private void Check_Lose()
    {
        if(NeverPour())
        {
            Ui_Lose.SetActive(true);
        }
    }
    public bool NeverPour()
    {
        for (int i = 0; i < Bottles.Count; i++)
        {
            for (int j = 0; j < Bottles.Count; j++)
            {
                if (i == j) continue;

                Bottle from = Bottles[i];
                Bottle to = Bottles[j];
                if (from.IsEmpty || to.IsFull) continue;
                if (to.IsEmpty) return false;
                if (from.TopCell.Color == to.TopCell.Color) return false;
            }
        }
        return true;
    }

    public void ResetLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
