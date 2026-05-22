using DefqonEngine.Core.Timeline.Events;
using DefqonEngine.Core.Timeline.Tracks;
using DefqonEngine.UI.Timeline.Control;
using System.Collections.Generic;
using UnityEngine;

namespace DefqonEngine.Core.Timeline.History
{
    public class TimelineHistory : MonoBehaviour
    {
        public static TimelineHistory Instance { get; private set; }

        private readonly Stack<TimelineState> undoStack = new();
        private readonly Stack<TimelineState> redoStack = new();
        public bool IsRestoring { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            TimelineInputController.Instance.OnUndo += Undo;
            TimelineInputController.Instance.OnRedo += Redo;
        }

        public void SaveState(string description = "")
        {
            Debug.Log($"Saving state: {description}");
            if (IsRestoring)
                return;

            undoStack.Push(CaptureState());

            redoStack.Clear();
        }

        public void Undo()
        {
            if (undoStack.Count == 0)
                return;

            Debug.Log($"Undoing, stackCount: {undoStack.Count}");

            IsRestoring = true;

            try
            {
                redoStack.Push(CaptureState());

                TimelineState previous =
                    undoStack.Pop();

                RestoreState(previous);
            }
            finally
            {
                IsRestoring = false;
            }

            Debug.Log($"Undo complete, undoStackCount: {undoStack.Count}, redoStackCount: {redoStack.Count}");
        }

        public void Redo()
        {
            if (redoStack.Count == 0)
                return;

            Debug.Log($"Redoing, stackCount: {redoStack.Count}");

            IsRestoring = true;

            try
            {
                undoStack.Push(CaptureState());

                TimelineState redo =
                    redoStack.Pop();

                RestoreState(redo);
            }
            finally
            {
                IsRestoring = false;
            }

            Debug.Log($"Redo complete, undoStackCount: {undoStack.Count}, redoStackCount: {redoStack.Count}");
        }

        public TimelineState CaptureState()
        {
            TimelineState state = new();

            // Tracks
            state.trackCount =
                TimelineTrackManager.Instance.TrackCount;

            // Events
            state.events = TimelineEventManager.Instance.GetEventsCopy();

            // Selection
            if (TimelineEventManager.Instance.selectedEvents != null)
            {
                foreach (var selectedEvent in TimelineEventManager.Instance.selectedEvents)
                {
                    int index = TimelineEventManager.Instance.events.IndexOf(selectedEvent);
                    if (index != -1)
                    {
                        state.selectedEventIndexes.Add(index);
                    }
                }
            }

            return state;
        }

        public void RestoreState(TimelineState state)
        {
            if (state == null) return;

            // Clear events
            TimelineEventManager.Instance.ClearEvents();

            if (TimelineTrackManager.Instance.TrackCount != state.trackCount)
            {
                // Clear tracks
                TimelineTrackManager.Instance.ClearAll();

                // Rebuild tracks
                TimelineTrackManager.Instance.AddTracks(state.trackCount);
            }

            // Rebuild events
            foreach (var ev in state.events)
            {
                TimelineEventManager.Instance.AddEvent(TimelineEventManager.CloneEvent(ev), false);
            }

            // Restore selection
            foreach (var index in state.selectedEventIndexes)
            {
                if (index >= 0 &&
                    index < state.events.Count)
                {
                    TimelineEventManager.Instance.SelectEvent(
                        TimelineEventManager.Instance.events[
                            index]);
                }
                else
                {
                    TimelineEventManager.Instance.DeselectEvent();
                }
            }
        }
    }

}