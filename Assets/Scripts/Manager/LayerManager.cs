using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerManager : MonoBehaviour
{

    public static LayerManager Instance;

    [SerializeField] public Sprite[] sprites;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        PrepareItems();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PrepareItems()
    {
        GameObject[] emptyBoxs = GameObject.FindGameObjectsWithTag("EmptyBox");
        int[] idxs = new int[emptyBoxs.Length];
        for (int i = 0; i < idxs.Length; i++)
        {
            int idx = Random.Range(0, sprites.Length);
            idxs[i] = idx;
            idxs[i+1] = idx;
            i += 1;
        }

        int[] newIdxs = Shuffle(idxs);

        for (int i = 0; i < newIdxs.Length; i++)
        {
            emptyBoxs[i].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = sprites[newIdxs[i]];
        }

        GameManager.Instance.GameStatus = GameManager.GameStatusType.Ingaming;

    }

    private int[] Shuffle(int[] oList)
    {
        // KD混排
        for (int i = oList.Length-1; i > 0; i--)
        {
            int exchange = Random.Range(0, i + 1);
            int tmp = oList[i];
            oList[i] = oList[exchange];
            oList[exchange] = tmp;
        }

        return oList;
    }

}
