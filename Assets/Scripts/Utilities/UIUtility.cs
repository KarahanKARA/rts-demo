using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utilities
{
    public static class UIUtility
    {
        public static bool IsPointerOverUIObject()
        {
            var eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            return results.Count > 0;
        }
    }
}