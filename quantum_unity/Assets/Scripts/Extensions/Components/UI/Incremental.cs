using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Extensions.Components.UI
{
    [AddComponentMenu("UI/Incremental", 13)]
    public class Incremental : Selectable, IMoveHandler
    {
        [SerializeField] private UnityEvent<int> _onIncrementDecrement;
        public UnityEvent<int> OnIncrementDecrement => _onIncrementDecrement;

        [SerializeField] private int _default;
        [SerializeField] private int _min;
        [SerializeField] private int _max;
        
        [SerializeField] private Animator _up;
        [SerializeField] private Animator _down;

        private int _value = 0;

        protected override void Awake()
        {
            base.Awake();

            _value = _default;
        }

        void IMoveHandler.OnMove(AxisEventData eventData)
        {
            if (eventData.moveDir == MoveDirection.Up)
            {
                Up();
            }
            else if (eventData.moveDir == MoveDirection.Down)
            {
                Down();
            }
            else if (eventData.moveDir == MoveDirection.Left)
            {
                Selectable selectable = FindSelectableOnLeft();
                if (selectable)
                    eventData.selectedObject = selectable.gameObject;
            }
            else if (eventData.moveDir == MoveDirection.Right)
            {
                Selectable selectable = FindSelectableOnRight();
                if (selectable)
                    eventData.selectedObject = selectable.gameObject;
            }
        }

        private void Up()
        {
            ++_value;
            if (_value > _max)
            {
                _value = _max;
            }

            _up.enabled = true;
            _onIncrementDecrement.Invoke(1);
        }

        private void Down()
        {
            --_value;
            if (_value < _min)
            {
                _value = _min;
            }

            _down.enabled = true;
            _onIncrementDecrement.Invoke(-1);
        }

        public void AdjustUp()
        {
            Up();
        }

        public void AdjustDown()
        {
            Down();
        }
    }
}
