using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace game
{
	public class UpdateManager : MonoBehaviour
	{
		private static UpdateManager _instance;
		public static UpdateManager Instance {
			get {
				if (_instance == null)
				{
					_instance = FindObjectOfType(typeof(UpdateManager)) as UpdateManager;
				}
				return _instance;
			}
		}
		public Dictionary<string, UnityAction> updataDic 
			= new Dictionary<string, UnityAction>();
		public Dictionary<string, UnityAction<Vector2>> getButtonDic
			= new Dictionary<string, UnityAction<Vector2>>();

		void Update()
		{
			foreach (UnityAction updateAction in this.updataDic.Values) {
				updateAction.Invoke();
			}
			if (Input.GetMouseButton(0)) {
				Vector2 mousePoint =
						Camera.main.ScreenToWorldPoint(Input.mousePosition);
				foreach (UnityAction<Vector2> buttonAcion in this.getButtonDic.Values)
				{
					buttonAcion.Invoke(mousePoint);
				}
			}
		}

		public void AddToUpdate(string actionName,UnityAction action) {
			this.updataDic.Add(actionName, action);
		}

		public void RemoveToUpdate(string actionName)
		{
			this.updataDic.Remove(actionName);
		}

		public void AddToGetButton(string actionName, UnityAction<Vector2> action)
		{
			this.getButtonDic.Add(actionName, action);
		}

		public void RemoveToGetButton(string actionName)
		{
			this.getButtonDic.Remove(actionName);
		}
	}
}
