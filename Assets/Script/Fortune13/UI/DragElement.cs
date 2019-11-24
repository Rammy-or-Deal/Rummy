namespace Assets.Scripts.Core
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using System;
    using UnityEngine.UI;

    public class DragElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IDropHandler
    {
        public Vector2 LastPosition { get; private set; }
        
        public void Start()
        {
            
        }
        
        /// <summary>
        /// Trigger event when drag is beginning.
        /// </summary>
        /// <param name="eventData">Data from that event.</param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            LastPosition = transform.position;
            this.GetComponent<Graphic>().raycastTarget = false;
        }

        /// <summary>
        /// Trigger event when drag continue.
        /// </summary>
        /// <param name="eventData">Data from that event.</param>
        public void OnDrag(PointerEventData eventData)
        {
            if (FortuneUIController.Inst.changeDlg.gameObject.activeSelf)
                transform.position += (Vector3)eventData.delta;
        }

        /// <summary>
        /// Trigger event when drag is ended and object is dropped.
        /// </summary>
        /// <param name="eventData">Data from that event.</param>
        public void OnEndDrag(PointerEventData eventData)
        {
            if (FortuneUIController.Inst.changeDlg.gameObject.activeSelf)
            {
                this.GetComponent<Graphic>().raycastTarget = true;
                transform.position = LastPosition;    
            }
        }
        
        public void OnDrop(PointerEventData data)
        {
            if (!FortuneUIController.Inst.changeDlg.gameObject.activeSelf)
                return;
            GameObject fromItem = data.pointerDrag;
            if (data.pointerDrag == null) return; // (will never happen)
            DragElement d = fromItem.GetComponent<DragElement>();
            if (d == null)
            {
                return;
            }

            Debug.Log ("dropped  " + fromItem.name +" onto " +gameObject.name);
            FortuneCard fromCard = fromItem.GetComponent<FortuneCard>();
            Card card = fromCard.GetValue();
            FortuneCard toCard = gameObject.GetComponent<FortuneCard>();
            fromCard.SetValue(toCard.GetValue());
            toCard.SetValue(card);

            FortuneUIController.Inst.changeDlg.UpdateHandSuitString();
        }
    }
}
