using DefqonEngine.Sequencing.Audio;
using DefqonEngine.UI.Popup;
using DefqonEngine.UI.Timeline.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefqonEngine.UI.Timeline.Waveform
{

    [RequireComponent(typeof(WaveformTileFactory))]
    [RequireComponent(typeof(WaveformAnalyzer))]
    [RequireComponent(typeof(WaveformPositioner))]
    public class WaveformDrawer : MonoBehaviour
    {
        public static WaveformDrawer Instance { get; private set; }

        [Header("Settings")]
        public float tileDurationSeconds = 10f;
        public int samplesPerPoint = 256;

        [SerializeField] private WaveformTileFactory tileFactory;
        [SerializeField] private WaveformAnalyzer analyzer;
        [SerializeField] private WaveformPositioner positioner;
        [SerializeField] private PopupCanvas loadingPopUp;
        [SerializeField] private ProgressBar progressBar;
        public event Action OnWaveformLoaded;

        private readonly List<WaveformTile> tiles = new();
        private Coroutine refreshRoutine;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        void Start()
        {
            AudioPlaybackController.Instance.OnAudioSourceChanged += Refresh;
            Refresh();
        }

        void Update()
        {
            positioner.PositionTiles(tiles);

            if (TimelineView.Instance != null)
                positioner.UpdateContainerSize(transform);
        }

        public void Refresh()
        {
            if (refreshRoutine != null)
                StopCoroutine(refreshRoutine);

            refreshRoutine = StartCoroutine(RefreshAsync());
        }

        private IEnumerator RefreshAsync()
        {
            var clip = AudioPlaybackController.Instance.GetAudioClip();

            if (clip == null || TimelineView.Instance == null)
                yield break;

            ClearTiles();

            loadingPopUp.Open();

            yield return null; // wait a frame to ensure UI updates before heavy processing

            yield return analyzer.GenerateTilesAsync(
                clip,
                tileDurationSeconds,
                samplesPerPoint,
                OnTilesGenerated,
                (progress) => SetProgress(progress, "Analyzing audio...")
            );
        }

        private void SetProgress(float progress, string label)
        {
            progressBar.SetProgress(progress, label);
            if (progress >= 1f)
                loadingPopUp.Close();
        }

        private void OnTilesGenerated(List<WaveformAnalyzer.TileData> tileData)
        {
            StartCoroutine(BuildTiles(tileData));
        }

        private IEnumerator BuildTiles(List<WaveformAnalyzer.TileData> tileData)
        {
            int total = tileData.Count;

            for (int i = 0; i < tileData.Count; i++)
            {
                WaveformAnalyzer.TileData data = tileData[i];
                var tile = tileFactory.CreateTile(
                    transform,
                    data.points,
                    data.startSample,
                    data.endSample);

                tiles.Add(tile);
                float progress = (i + 1f) / total;
                SetProgress(progress, "Loading Audio tiles...");

                if (tiles.Count % 5 == 0)
                    yield return null;
            }

            positioner.UpdateContainerSize(transform);

            OnWaveformLoaded?.Invoke();
        }


        void ClearTiles()
        {
            foreach (var tile in tiles)
            {
                if (tile.texture != null)
                    Destroy(tile.texture);

                if (tile.image != null)
                    Destroy(tile.image.gameObject);
            }

            tiles.Clear();
        }
    }
}