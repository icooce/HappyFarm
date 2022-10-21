using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemController : MonoBehaviour,IPointerDownHandler
{
    [HideInInspector] Rigidbody2D rb;

    [SerializeField] public int layerNum;
    [SerializeField] public bool isMasked;
    [SerializeField] public bool isUsed = false;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.Instance.GameStatus == GameManager.GameStatusType.Gameover)
        {
            return;
        }

        if (isMasked)
        {
            return;
        }

        if (SlotView.Instance.isPlayStarVFX)
        {
            return;
        }

        AudioManager.Instance.PlayClickBtnAudioSource();

        transform.GetComponent<BoxCollider2D>().enabled = false;
        transform.GetComponent<SpriteRenderer>().sortingLayerName = "UI";
        transform.GetChild(0).transform.GetComponent<SpriteRenderer>().sortingLayerName = "UI";

        SlotView.Instance.emptyBox = new EmptyBox();
        SlotView.Instance.emptyBox.parent = transform.parent;
        SlotView.Instance.emptyBox.pos = transform.position;
        SlotView.Instance.emptyBox.sprite = transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        SlotView.Instance.emptyBox.layerNum = layerNum;

        transform.localScale = new Vector3(transform.localScale.x*1.3f,transform.localScale.y*1.3f,transform.localScale.z*1.3f);

        SlotView.Instance.OnloadingSprite(transform.GetChild(0).GetComponent<SpriteRenderer>().sprite);

        Destroy(gameObject,0.2f);


        

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag.Equals("EmptyBox"))
        {
            if (layerNum > other.GetComponent<ItemController>().layerNum)
            {
                other.GetComponent<SpriteRenderer>().color = new Color(0.65f, 0.65f, 0.65f, 1.0f);
                other.GetComponent<ItemController>().isMasked = true;
            }
            
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        int layerSorting = gameObject.GetComponent<SpriteRenderer>().sortingOrder;
        if (other.tag.Equals("EmptyBox"))
        {
            int oLayerSorting = other.GetComponent<SpriteRenderer>().sortingOrder;
            if (layerNum>other.GetComponent<ItemController>().layerNum || layerSorting > oLayerSorting)
            {
                other.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                other.GetComponent<ItemController>().isMasked = false;
                other.GetComponent<Rigidbody2D>().WakeUp();

            }
        }
    }
}
