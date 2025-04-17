using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class BearSpawner : MonoBehaviour
{
    [Header("Spawner")] [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private BearEnemy bearPrefab;

    [SerializeField] private float timeBetweenRounds = 60f;

    [SerializeField] private float timeBetweenSpawn = 1f;

    [SerializeField] private int enemiesPerRound = 3;

    [SerializeField] private int increasePerRound = 2;

    [Header("Canvas")] [SerializeField] private Canvas canvasRoundInfo;

    [SerializeField] private TMP_Text currentRoundText;

    [SerializeField] private TMP_Text bearsKilledText;

    [SerializeField] private TMP_Text timeLeftText;

    [SerializeField] private float textScale = 1.0f;

    [SerializeField] private float textScaleEffect = 1.15f;

    [SerializeField] private float textScaleEffectDuration = 0.333f;

    [SerializeField] private int timeRemainingRound = 2;

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
            var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            var bearEnemy = Instantiate(bearPrefab, spawnPoint.position, spawnPoint.rotation);
            bearEnemy.OnBearDied += UpdateKilledBearCount;

            yield return new WaitForSeconds(timeBetweenSpawn);
        }
    }

    private void UpdateKilledBearCount(BearEnemy enemy)
    {
        _bearsKilled++;
        bearsKilledText.text = $"Bjǫrn's killed: {_bearsKilled}";

        ApplyTextMeshProTextAnimation(bearsKilledText);
    }

    private void UpdateCurrentRound()
    {
        _currentRound++;
        currentRoundText.text = $"Current round: {_currentRound}";

        ApplyTextMeshProTextAnimation(currentRoundText);
    }

    private void UpdateTimeLeft()
    {
        timeLeftText.text = $"Time remaining: {timeBetweenRounds}";

        float timeLeft = timeBetweenRounds;

        var seq = ApplyTextMeshProTextAnimation(timeLeftText);
        seq.JoinCallback(() =>
        {
            DOTween.To(() => timeLeft, x =>
                {
                    timeLeft = x;
                    timeLeftText.text = $"Time remaining: {Math.Round(timeLeft, timeRemainingRound)}";
                }, 0, timeBetweenRounds)
                .SetEase(Ease.Linear);
        });
    }

    private Sequence ApplyTextMeshProTextAnimation(TMP_Text text)
    {
        var seq = DOTween.Sequence();
        seq.Append(text.transform.DOScale(textScaleEffect, textScaleEffectDuration).SetEase(Ease.OutBack));
        seq.Append(text.transform.DOScale(textScale, textScaleEffectDuration).SetEase(Ease.OutBack));

        return seq;
    }
}