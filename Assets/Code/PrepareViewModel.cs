using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;
using protocol;

namespace game {
	public class PrepareViewModel : ViewModel {
		private PrepareView m_View;
		private PrepareModel m_Model;

		public bool[,] m_ChessManaul {
			get {
				return this.m_Model.m_ChessManaul;
			}
			set {
				this.m_Model.m_ChessManaul = value;
			}
		}
		void Awake() {
			this.m_Parent = GameManager.Instance.transform;
			this.m_View = this.gameObject.AddComponent<PrepareView>();
			this.m_Model = GameManager.GetModel<PrepareModel>();
			NetManager.ListenerMsg(ProtoDefine.StartGame, StartGameCallBack);
		}

		public void ReqChessManaul(byte[] bytes)
		{
			ChessManaul msgData = new ChessManaul
			{
				manaul = bytes
			};
			NetManager.SendMsg(ProtoDefine.ChessManaul, msgData);
		}

		public void ReqStartGame()
		{
			StartGame msgData = new StartGame
			{
			};
			NetManager.SendMsg(ProtoDefine.StartGame, msgData);
		}

		public void StartGameCallBack(IExtensible msgData) {
			StartGame startGame = (StartGame)msgData;
			GameManager.Instance.StartGame(startGame.start);
			Debug.LogError("Start Game！！！！！！！！！！！");
		}

		public void StorageChessManaul(Vector2Int vector2, int offset_x = 0, int offset_y = 0) {
			vector2.x += offset_x;
			vector2.y += offset_y;
			this.m_ChessManaul[vector2.x, vector2.y] = true;
		}

		public void StartGame() {
			byte[] bytes = new byte[100];
			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					Debug.Log(i + " " + j + " " + m_ChessManaul[i,j]);
					if (this.m_ChessManaul[i, j])
						bytes[i * 10 + j] = 1;
					else
					{
						bytes[i * 10 + j] = 0;
					}
				}
			}
			this.ReqChessManaul(bytes);
			//yield return new WaitForSeconds(3);
			this.ReqStartGame();
		}

		void Update() {
			if (Input.GetKeyDown(KeyCode.A)) {
			
			}
		}
	}
}
