using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class BearSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private BearEnemy bearPrefab;

    [SerializeField] private float timeBetweenRounds = 60f;
    [SerializeField] private float timeBetweenSpawn = 1f;
    [SerializeField] private int enemiesPerRound = 3;
    [SerializeField] private int increasePerRound = 2;

    [SerializeField] private Canvas canvasRoundInfo;
    [SerializeField] private TMP_Text currentRoundText;
    [SerializeField] private TMP_Text bearsKilledText;
    [SerializeField] private TMP_Text timeLeftText;

    private int _currentRound = 1;
    private int _bearsKilled = 0;

    private void Start()
    {
        canvasRoundInfo.gameObject.SetActive(false);
    }

    public void StartSpawn()
    {
        canvasRoundInfo.gameObject.SetActive(true);
        StartCoroutine(SpawnRounds());
    }

    private IEnumerator SpawnRounds()
    {
        bearsKilledText.text = $"Bjǫrn's killed: {_bearsKilled}";
        currentRoundText.text = $"Current round: {_currentRound}";

        while (true)
        {
            UpdateTimeLeft();

            StartCoroutine(SpawnBears());

            enemiesPerRound += increasePerRound;

            yield return new WaitForSeconds(timeBetweenRounds);

            UpdateCurrentRound();
        }
    }

    private IEnumerator SpawnBears()
    {
        for (int i = 0; i < enemiesPerRound; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            var bearEnemy = Instantiate(bearPrefab, spawnPoint.position, spawnPoint.rotation);
            bearEnemy.OnBearDied += UpdateKilledBearCount;
            yield return new WaitForSeconds(timeBetweenSpawn);
        }
    }

    private void UpdateKilledBearCount(BearEnemy enemy)
    {
        _bearsKilled++;
        bearsKilledText.text = $"Bjǫrn's killed: {_bearsKilled}";

        var seq = DOTween.Sequence();
        seq.Append(bearsKilledText.transform.DOScale(1.15f, 0.333f).SetEase(Ease.OutBack));
        seq.Append(bearsKilledText.transform.DOScale(1f, 0.333f).SetEase(Ease.OutBack));
    }

    private void UpdateCurrentRound()
    {
        _currentRound++;
        currentRoundText.text = $"Current round: {_currentRound}";

        var seq = DOTween.Sequence();
        seq.Append(currentRoundText.transform.DOScale(1.15f, 0.333f).SetEase(Ease.OutBack));
        seq.Append(currentRoundText.transform.DOScale(1f, 0.333f).SetEase(Ease.OutBack));
    }

    private void UpdateTimeLeft()
    {
        timeLeftText.text = $"Time remaining: {timeBetweenRounds}";

        float timeLeft = timeBetweenRounds;

        var seq = DOTween.Sequence();
        seq.Append(timeLeftText.transform.DOScale(1.15f, 0.333f).SetEase(Ease.OutBack));
        seq.Append(timeLeftText.transform.DOScale(1f, 0.333f).SetEase(Ease.OutBack));
        seq.JoinCallback(() =>
        {
            DOTween.To(() => timeLeft, x =>
                {
                    timeLeft = x;
                    timeLeftText.text = $"Time remaining: {Math.Round(timeLeft, 2)}";
                }, 0, timeBetweenRounds)
                .SetEase(Ease.Linear);
        });
    }
}