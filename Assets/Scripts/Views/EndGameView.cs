using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameView : MonoBehaviour
{

    public static EndGameView Instance;
    [SerializeField] GameObject gameOverView;
    [SerializeField] Text processText;
    [SerializeField] GameObject completedView;
    [SerializeField] GameObject sunsetImage;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        sunsetImage.transform.localEulerAngles += new Vector3(0f,0f, 100.0f * Time.deltaTime);
    }

    private void OnDisable()
    {
        sunsetImage.transform.localEulerAngles = Vector3.zero;
    }

    public void OnShowGameOverView()
    {
        StartCoroutine(ShowGameOverView());
    }

    IEnumerator ShowGameOverView()
    {
        yield return new WaitForSeconds(1.2f);

        gameOverView.SetActive(true);
        int process = 100 * GameManager.Instance.mergeItemCount / GameManager.Instance.itemMax;
        processText.text = "已完成" + process + "%,离成功又进一步！";

    }


    public void OnShowCompletedView()
    {
        StartCoroutine(ShowCompletedView());
    }

    IEnumerator ShowCompletedView()
    {
        yield return new WaitForSeconds(1.2f);

        completedView.SetActive(true);
    }


    public void OnCloseGameOverView()
    {
        gameOverView.SetActive(false);
    }

}
