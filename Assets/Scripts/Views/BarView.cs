using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarView : MonoBehaviour
{
    public static BarView Instance;

    public enum PropType { Unuse,Remove,Undo,Refresh,Resume}

    [SerializeField] public PropType propType;

    [SerializeField] GameObject popView;
    [SerializeField] GameObject removeView;
    [SerializeField] GameObject undoView;
    [SerializeField] GameObject refreshView;
    [SerializeField] GameObject resumeView;

    [Space]

    [SerializeField] GameObject removePlus;
    [SerializeField] GameObject undoPlus;
    [SerializeField] GameObject refreshPlus;


    [Space]

    [SerializeField] int removePropCount;
    [SerializeField] int undoPropCount;
    [SerializeField] int refreshPropCount;
    [SerializeField] Sprite oneSprite;
    [SerializeField] Sprite plusSprite;
    [SerializeField] GameObject removePropMask;
    [SerializeField] GameObject undoPropMask;
    [SerializeField] GameObject refreshPropMask;
    [SerializeField] GameObject propLayer;
    [SerializeField] public bool isRefresh;

    [Space]
    [SerializeField] GameObject emptyBoxPrefab;
    
    List<Vector3> oriEmptyBoxsPos;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        propType = PropType.Unuse;
        isRefresh = false;
        oriEmptyBoxsPos = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRefresh)
        {
            //start
            StartCoroutine(StartRefresh());
            
        }
    }

    IEnumerator StartRefresh()
    {
        GameObject[] emptyBoxs = GameObject.FindGameObjectsWithTag("EmptyBox");
        float angle = 0f;

        List<Sprite> sprites = new List<Sprite>();
        for (int i = 0; i < emptyBoxs.Length; i++)
        {
            if (emptyBoxs[i].GetComponent<ItemController>().isUsed)
            {
                continue;
            }
            angle += i * 10 * Time.deltaTime;

            emptyBoxs[i].transform.position = new Vector3(3.0f * Mathf.Cos(angle * Mathf.Rad2Deg), 3.0f * Mathf.Sin(angle * Mathf.Rad2Deg), 0f);

            sprites.Add(emptyBoxs[i].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite);
        }


        // KD洗牌
        for (int i = sprites.Count-1; i > 0; i--)
        {
            int exchange = Random.Range(0, i + 1);
            Sprite tmp = sprites[i];
            sprites[i] = sprites[exchange];
            sprites[exchange] = tmp;
        }

        for (int i = 0; i < emptyBoxs.Length; i++)
        {
            if (emptyBoxs[i].GetComponent<ItemController>().isUsed)
            {
                continue;
            }

            emptyBoxs[i].transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = sprites[i];
        }



        yield return new WaitForSeconds(1.5f);
        isRefresh = false;
        for (int i = 0; i < emptyBoxs.Length; i++)
        {
            if (emptyBoxs[i].GetComponent<ItemController>().isUsed)
            {
                continue;
            }
            emptyBoxs[i].transform.position = oriEmptyBoxsPos[i];
        }

    }

    public void OnClickProp(int idx)
    {
        if (propType != PropType.Unuse)
        {
            return;
        }

        AudioManager.Instance.PlayClickBtnAudioSource();

        switch (idx)
        {
            case 1:
                if (removePropCount == 0)
                {
                    propType = PropType.Remove;
                    popView.SetActive(true);
                    removeView.SetActive(true);
                } else
                {
                    OnUseRemoveProp();
                }
                break;
            case 2:
                if (undoPropCount == 0)
                {
                    propType = PropType.Undo;
                    popView.SetActive(true);
                    undoView.SetActive(true);
                } else
                {
                    OnUseUndoProp();
                }
                break;
            case 3:
                if (refreshPropCount == 0)
                {
                    propType = PropType.Refresh;
                    popView.SetActive(true);
                    refreshView.SetActive(true);
                }
                else {
                    OnUseRefreshProp();
                }
                break;
            default:
                propType = PropType.Unuse;
                break;
        }
    }

    public void OnGetProp()
    {
        AudioManager.Instance.PlayClickBtnAudioSource();
        switch (propType)
        {
            case PropType.Remove:
                removePropCount = 1;
                removePlus.GetComponent<Image>().sprite = oneSprite;
                removeView.SetActive(false);
                break;
            case PropType.Undo:
                undoPropCount = 1;
                undoPlus.GetComponent<Image>().sprite = oneSprite;
                undoView.SetActive(false);
                break;
            case PropType.Refresh:
                refreshPropCount = 1;
                refreshPlus.GetComponent<Image>().sprite = oneSprite;
                refreshView.SetActive(false);
                break;
            case PropType.Resume:
                OnUseResumeProp();
                break;
        }

        propType = PropType.Unuse;
        popView.SetActive(false);
    }

    public void OnGiveUpProp()
    {
        AudioManager.Instance.PlayClickBtnAudioSource();
        switch (propType)
        {
            case PropType.Remove:
                removeView.SetActive(false);
                break;
            case PropType.Undo:
                undoView.SetActive(false);
                break;
            case PropType.Refresh:
                refreshView.SetActive(false);
                break;
            case PropType.Resume:
                resumeView.SetActive(false);
                break;
        }

        propType = PropType.Unuse;
        popView.SetActive(false);
    }


    public void OnUseRemoveProp()
    {
        removePropCount = 0;
        removePlus.GetComponent<Image>().sprite = plusSprite;
        removePropMask.SetActive(true);
        removeView.SetActive(false);

        propType = PropType.Unuse;

        OnRemove();

    }


    private void OnRemove()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < propLayer.transform.childCount; j++)
            {
                if (!propLayer.transform.GetChild(j).gameObject.activeSelf)
                {
                    propLayer.transform.GetChild(j).gameObject.SetActive(true);
                    propLayer.transform.GetChild(j).GetChild(0).GetComponent<SpriteRenderer>().sprite = SlotView.Instance.loadingSprites[i];
                    propLayer.transform.GetChild(j).GetComponent<ItemController>().isUsed = true;
                    SlotView.Instance.loadingSprites[i] = null;
                    break;
                }
            }
        }


        for (int i = 3; i < SlotView.Instance.loadingSprites.Length; i++)
        {
            SlotView.Instance.loadingSprites[i - 3] = SlotView.Instance.loadingSprites[i];
            SlotView.Instance.loadingSprites[i] = null;
        }
    }

    public void OnUseUndoProp()
    {
        if (SlotView.Instance.emptyBox == null)
        {
            return;
        }

        undoPropCount = 0;
        undoPlus.GetComponent<Image>().sprite = plusSprite;
        undoPropMask.SetActive(true);
        undoView.SetActive(false);

        propType = PropType.Unuse;

        EmptyBox emptyBox = SlotView.Instance.emptyBox;
        GameObject newEmptyBox = Instantiate(emptyBoxPrefab, emptyBox.pos, Quaternion.identity);
        newEmptyBox.transform.SetParent(emptyBox.parent);
        newEmptyBox.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = emptyBox.sprite;
        newEmptyBox.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerName = "Layer" + emptyBox.layerNum;
        newEmptyBox.GetComponent<SpriteRenderer>().sortingLayerName = "Layer" + emptyBox.layerNum;
        newEmptyBox.GetComponent<ItemController>().layerNum = emptyBox.layerNum;

        SlotView.Instance.loadingSprites[emptyBox.slotIdx] = null;


        for (int i = emptyBox.slotIdx+1; i < SlotView.Instance.loadingSprites.Length; i++)
        {
            if (SlotView.Instance.loadingSprites[i] == null)
            {
                break;
            }
            SlotView.Instance.loadingSprites[i - 1] = SlotView.Instance.loadingSprites[i];
            SlotView.Instance.loadingSprites[i] = null;

        }
    }


    public void OnUseRefreshProp()
    {
        refreshPropCount = 0;
        refreshPlus.GetComponent<Image>().sprite = plusSprite;
        refreshPropMask.SetActive(true);
        refreshView.SetActive(false);

        propType = PropType.Unuse;

        GameObject[] emptyBoxs = GameObject.FindGameObjectsWithTag("EmptyBox");
        for (int i = 0; i < emptyBoxs.Length; i++)
        {
            if (emptyBoxs[i].GetComponent<ItemController>().isUsed)
            {
                continue;
            }
            oriEmptyBoxsPos.Add(emptyBoxs[i].transform.position);
        }

        isRefresh = true;
    }


    public void OnShowResumeView()
    {
        popView.SetActive(true);
        resumeView.SetActive(true);

        propType = PropType.Resume;
    }


    public void OnUseResumeProp()
    {
        resumeView.SetActive(false);
        popView.SetActive(false);

        propType = PropType.Unuse;

        EndGameView.Instance.OnCloseGameOverView();

        OnRemove();

        GameManager.Instance.GameStatus = GameManager.GameStatusType.Ingaming;


    }

}
