using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    public enum GameStatusType { Ready,Ingaming,Gameover,Completed}
    private GameStatusType gameStatus;
    public GameStatusType GameStatus
    {
        get { return gameStatus; }
        set
        {
            gameStatus = value;
            switch (gameStatus)
            {
                case GameStatusType.Gameover:
                    // show gameover panel
                    EndGameView.Instance.OnShowGameOverView();
                    BarView.Instance.OnShowResumeView();
                    break;
                case GameStatusType.Ingaming:
                    if (isCompletedGuideLevel)
                    {
                        GameObject[] emptyboxs = GameObject.FindGameObjectsWithTag("EmptyBox");
                        itemMax = emptyboxs.Length;
                    }
                    break;
                case GameStatusType.Completed:
                    EndGameView.Instance.OnShowCompletedView();break;
            }
        }
    }

    [SerializeField] public bool isCompletedGuideLevel;
    [SerializeField] GameObject guideLevel;
    [SerializeField] GameObject dailyLevel;

    [SerializeField] public int itemMax;
    [SerializeField] public int mergeItemCount;


    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        isCompletedGuideLevel = false;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("GameStatus:" + gameStatus);

        GameObject[] emptyBoxs = GameObject.FindGameObjectsWithTag("EmptyBox");
        if (emptyBoxs.Length == 0)
        {
            if (!isCompletedGuideLevel)
            {
                isCompletedGuideLevel = true;
                guideLevel.SetActive(false);
                dailyLevel.SetActive(true);

                LayerManager.Instance.PrepareItems();
            } else
            {
                GameStatus = GameStatusType.Completed;
            }
        }
    }


    public void OnNewStartGame()
    {
        SceneManager.LoadScene("Main");
    }
}
