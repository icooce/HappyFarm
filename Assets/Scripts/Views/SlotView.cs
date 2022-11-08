using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class EmptyBox
{
    public Transform parent;
    public Vector3 pos;
    public Sprite sprite;
    public int layerNum;
    public int slotIdx;
}


public class SlotView : MonoBehaviour
{
    public static SlotView Instance;

    [SerializeField] public Sprite[] loadingSprites;
    [HideInInspector] public Dictionary<string, int> itemCountDict;

    [SerializeField] public bool isPlayMergeAudioSource;
    [SerializeField] public bool isPlayStarVFX;
    [SerializeField] public string isMergeSpriteName;

    [Space]
    [Header("StartVFX Config")]
    [SerializeField] Transform starVFXs;
    [SerializeField] GameObject starVFXPrefab;

    List<Sprite> tmpSprites = new List<Sprite>();

    [Space]
    [SerializeField] public EmptyBox emptyBox;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        loadingSprites = new Sprite[9];
        itemCountDict = new Dictionary<string, int>();
        isPlayMergeAudioSource = false;
        isPlayStarVFX = false;
        isMergeSpriteName = null;
        emptyBox = null;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < loadingSprites.Length; i++)
        {
            if (!isPlayStarVFX)
            {
                transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
            }

            transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = loadingSprites[i];
            if (loadingSprites[i] == null)
            {
                transform.GetChild(i).GetChild(0).GetComponent<Image>().color = new Color(1.0f,1.0f,1.0f,0f);
            } else
            {
                if (!transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite.name.Equals(isMergeSpriteName))
                {
                    transform.GetChild(i).GetChild(0).GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                }
                
            }
        }
    }

    public void OnloadingSprite(Sprite sprite)
    {
        isPlayMergeAudioSource = false;
        List<Sprite> tmpSprites = new List<Sprite>();
        bool isFilled = false;
        for (int i = 0; i < loadingSprites.Length; i++)
        {
            // 第一个未被填充的Slot
            if (loadingSprites[i] == null)
            {
                if (!isFilled)
                {
                    emptyBox.slotIdx = i;
                    tmpSprites.Add(sprite);
                }
                break;
            } else
            {
                if (sprite.name.Equals(loadingSprites[i].name) && !isFilled)
                {
                    isFilled = true;
                    tmpSprites.Add(sprite);
                    emptyBox.slotIdx = i;
                }
                tmpSprites.Add(loadingSprites[i]);
            }

            

        }

        loadingSprites = new Sprite[9];
        itemCountDict = new Dictionary<string, int>();
        bool isCanMerge = false;
        for (int i = 0; i < tmpSprites.Count; i++)
        {
            loadingSprites[i] = tmpSprites[i];

            if (itemCountDict.ContainsKey(loadingSprites[i].name))
            {
                itemCountDict[loadingSprites[i].name] += 1;
            }
            else
            {
                itemCountDict[loadingSprites[i].name] = 1;
            }

            if (itemCountDict[loadingSprites[i].name] == 3)
            {
                isCanMerge = true;
            }

        }

        if (!isCanMerge && tmpSprites.Count == 9)
        {
            GameManager.Instance.GameStatus = GameManager.GameStatusType.Gameover;
        }

        StartCoroutine(CheckMerge());

    }

    IEnumerator CheckMerge()
    {
        foreach (var item in itemCountDict)
        {
            Debug.Log("item.Key:"+ item.Key+ ",item.Value:"+ item.Value);
            if (item.Value == 3)
            {
                isPlayStarVFX = true;
                if (!isPlayMergeAudioSource)
                {
                    isPlayMergeAudioSource = true;
                    AudioManager.Instance.PlayMergeAudioSource();
                }
                isMergeSpriteName = item.Key;
                DestroyItem(item.Key);
                break;
            }
        }

        yield return new WaitForSeconds(1.0f);

        if (isPlayStarVFX)
        {
            loadingSprites = new Sprite[9];
            for (int i = 0; i < tmpSprites.Count; i++)
            {
                loadingSprites[i] = tmpSprites[i];
            }
            isMergeSpriteName = null;
            isPlayStarVFX = false;
            emptyBox = null;
        }

    }

    void DestroyItem(string name)
    {
        tmpSprites = new List<Sprite>();
        for (int i = 0; i < loadingSprites.Length; i++)
        {
            if (loadingSprites[i] == null)
            {
                break;
            } else
            {
                if (!name.Equals(loadingSprites[i].name))
                {
                    tmpSprites.Add(loadingSprites[i]);
                }

                if (name.Equals(loadingSprites[i].name))
                {
                    transform.GetChild(i).GetChild(0).GetComponent<Image>().color = new Color(1.0f,1.0f,1.0f, 0f);
                    //transform.GetChild(i).GetChild(1).gameObject.SetActive(true);
                    GameObject newStarVFX = Instantiate(starVFXPrefab);
                    newStarVFX.transform.SetParent(starVFXs);
                    newStarVFX.transform.localPosition = new Vector3(-3.9f + i * 1.0f, 0f, 0f);
                    Destroy(newStarVFX, 2.0f);

                    if (GameManager.Instance.isCompletedGuideLevel)
                    {
                        GameManager.Instance.mergeItemCount++;
                    }
                }
            }
        }

        
    }
}
