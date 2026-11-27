using TMPro;
using UnityEngine;

public class PlayerMemoryScoreAmount : MonoBehaviour
{

    public int points;
    [SerializeField] private TMP_Text pointText;

    [Header("Score UI")]
    [SerializeField] private GameObject memoryScoreObject;
    [SerializeField] private GameObject normalScoreObject;

    public void GivePoints(int pointAmount)
    {
        points += pointAmount;
        if (pointText) pointText.text = points.ToString();

    }

    public void ResetPoints()
    {
        points = 0;
        if (pointText) pointText.text = points.ToString();
    }

    public void ShowMemoryScore(bool show)
    {
        if (memoryScoreObject) memoryScoreObject.SetActive(show);
        if (normalScoreObject) normalScoreObject.SetActive(!show);
    }
}
