using Quantum;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("UI/Multiplayer Button", 30)]
public class MultiplayerButton : Selectable, ISubmitHandler
{
    [Serializable]
    /// <summary>
    /// Function definition for a button click event.
    /// </summary>
    public class ButtonClickedEvent : UnityEvent<int> { }

    // Event delegates triggered on click.
    [SerializeField]
    private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

    protected MultiplayerButton()
    { }

    public ButtonClickedEvent onClick
    {
        get { return m_OnClick; }
        set { m_OnClick = value; }
    }

    private void Press(int playerNum)
    {
        if (!IsActive() || !IsInteractable())
            return;

        UISystemProfilerApi.AddMarker("Button.onClick", this);
        m_OnClick.Invoke(playerNum);
    }

    public virtual void OnSubmit(BaseEventData eventData)
    {
        Press((eventData as PlayerEventData).PlayerNum);

        // if we get set disabled during the press
        // don't run the coroutine.
        if (!IsActive() || !IsInteractable())
            return;

        DoStateTransition(SelectionState.Pressed, false);
        StartCoroutine(OnFinishSubmit());
    }

    private IEnumerator OnFinishSubmit()
    {
        var fadeTime = colors.fadeDuration;
        var elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        DoStateTransition(currentSelectionState, false);
    }

    #region Selectable

    private static Dictionary<GameObject, Selectable[]> _nearbySelectables = new();

    public override Selectable FindSelectableOnRight()
    {
        return FindSelectableBetter(transform.rotation * Vector3.right);
    }

    public override Selectable FindSelectableOnLeft()
    {
        return FindSelectableBetter(transform.rotation * Vector3.left);
    }

    public override Selectable FindSelectableOnUp()
    {
        return FindSelectableBetter(transform.rotation * Vector3.up);
    }

    public override Selectable FindSelectableOnDown()
    {
        return FindSelectableBetter(transform.rotation * Vector3.down);
    }

    private Selectable FindSelectableBetter(Vector3 dir)
    {
        if (!_nearbySelectables.ContainsKey(transform.parent.gameObject))
            _nearbySelectables.Add(transform.parent.gameObject, transform.parent.gameObject.GetComponentsInChildren<Selectable>());

        dir = dir.normalized;
        Vector3 localDir = Quaternion.Inverse(transform.rotation) * dir;
        Vector3 pos = transform.TransformPoint(GetPointOnRectEdge(transform as RectTransform, localDir));
        float maxScore = Mathf.NegativeInfinity;
        float maxFurthestScore = Mathf.NegativeInfinity;
        float score = 0;
        bool wantsWrapAround = navigation.wrapAround && (navigation.mode == UnityEngine.UI.Navigation.Mode.Vertical || navigation.mode == UnityEngine.UI.Navigation.Mode.Horizontal);
        Selectable bestPick = null;
        Selectable bestFurthestPick = null;

        for (int i = 0; i < _nearbySelectables[transform.parent.gameObject].Length; ++i)
        {
            Selectable sel = _nearbySelectables[transform.parent.gameObject][i];

            if (sel == this)
                continue;

            if (!sel.IsInteractable() || sel.navigation.mode == UnityEngine.UI.Navigation.Mode.None)
                continue;

#if UNITY_EDITOR
            // Apart from runtime use, FindSelectable is used by custom editors to
            // draw arrows between different selectables. For scene view cameras,
            // only selectables in the same stage should be considered.
            if (Camera.current != null && !UnityEditor.SceneManagement.StageUtility.IsGameObjectRenderedByCamera(sel.gameObject, Camera.current))
                continue;
#endif

            var selRect = sel.transform as RectTransform;
            Vector3 selCenter = selRect != null ? (Vector3)selRect.rect.center : Vector3.zero;
            Vector3 myVector = sel.transform.TransformPoint(selCenter) - pos;

            // Value that is the distance out along the direction.
            float dot = Vector3.Dot(dir, myVector);

            // If element is in wrong direction and we have wrapAround enabled check and cache it if furthest away.
            if (wantsWrapAround && dot < 0)
            {
                score = -dot * myVector.sqrMagnitude;

                if (score > maxFurthestScore)
                {
                    maxFurthestScore = score;
                    bestFurthestPick = sel;
                }

                continue;
            }

            // Skip elements that are in the wrong direction or which have zero distance.
            // This also ensures that the scoring formula below will not have a division by zero error.
            if (dot <= 0)
                continue;

            // This scoring function has two priorities:
            // - Score higher for positions that are closer.
            // - Score higher for positions that are located in the right direction.
            // This scoring function combines both of these criteria.
            // It can be seen as this:
            //   Dot (dir, myVector.normalized) / myVector.magnitude
            // The first part equals 1 if the direction of myVector is the same as dir, and 0 if it's orthogonal.
            // The second part scores lower the greater the distance is by dividing by the distance.
            // The formula below is equivalent but more optimized.
            //
            // If a given score is chosen, the positions that evaluate to that score will form a circle
            // that touches pos and whose center is located along dir. A way to visualize the resulting functionality is this:
            // From the position pos, blow up a circular balloon so it grows in the direction of dir.
            // The first Selectable whose center the circular balloon touches is the one that's chosen.
            score = dot / myVector.sqrMagnitude;

            if (score > maxScore)
            {
                maxScore = score;
                bestPick = sel;
            }
        }

        if (wantsWrapAround && null == bestPick) return bestFurthestPick;

        return bestPick;
    }

    private static Vector3 GetPointOnRectEdge(RectTransform rect, Vector2 dir)
    {
        if (rect == null)
            return Vector3.zero;
        if (dir != Vector2.zero)
            dir /= Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
        dir = rect.rect.center + Vector2.Scale(rect.rect.size, dir * 0.5f);
        return dir;
    }

    #endregion
}
