using UnityEngine;
using UnityEngine.EventSystems;

namespace HappyFinish.VR
{
	// An implementation of the BaseInputModule that uses the player's gaze and the magnet trigger
	// as a raycast generator.  To use, attach to the scene's EventSystem object.  Set the Canvas
	// object's Render Mode to World Space. If you'd like gaze to work with 3D scene objects, add a
	// PhysicsRaycaster to the gazing camera, and add a component that implements one of the Event
	// interfaces (EventTrigger will work nicely).  The objects must have colliders too.
	public class GazeGestureVR : BaseInputModule
	{
		// The pixel through which to cast rays, in viewport coordinates.  Generally, the center
		// pixel is best, assuming a monoscopic camera is selected as the Canvas' event camera.
		Vector2 hotspot = new Vector2(0.5f, 0.5f);

        string m_PrevName = "";

        private PointerEventData pointerData;

		public override bool ShouldActivateModule()
		{
			return base.ShouldActivateModule();
		}

		public override void DeactivateModule()
		{
			base.DeactivateModule();
			if (pointerData != null)
			{
				HandlePointerExitAndEnter(pointerData, null);
				pointerData = null;
			}
			eventSystem.SetSelectedGameObject(null, GetBaseEventData());
		}

		public override bool IsPointerOverGameObject(int pointerId)
		{
			return pointerData != null && pointerData.pointerEnter != null;
		}

		public override void Process()
		{
			CastRayFromGaze();
			UpdateCurrentObject();
		}

		private void CastRayFromGaze()
		{
			if (pointerData == null)
			{
				pointerData = new PointerEventData(eventSystem);
			}
			pointerData.Reset();
			pointerData.position = new Vector2(hotspot.x * Screen.width, hotspot.y * Screen.height);
			eventSystem.RaycastAll(pointerData, m_RaycastResultCache);
			pointerData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
			m_RaycastResultCache.Clear();
		}
		private void UpdateCurrentObject()
		{
			// Send enter events and update the highlight.
			var go = pointerData.pointerCurrentRaycast.gameObject;
            if (go != null)
            {
                if (!go.name.Equals(m_PrevName))
                {
                    //Debug.Log("New Hit : " + go.name);
                    m_PrevName = go.name;
                }
            }
			HandlePointerExitAndEnter(pointerData, go);
			// Update the current selection, or clear if it is no longer the current object.
			var selected = ExecuteEvents.GetEventHandler<ISelectHandler>(go);
			if (selected == eventSystem.currentSelectedGameObject)
			{
				ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, GetBaseEventData(),
									  ExecuteEvents.updateSelectedHandler);
			}
			else
			{
				eventSystem.SetSelectedGameObject(null, pointerData);
			}
		}
	}
}

