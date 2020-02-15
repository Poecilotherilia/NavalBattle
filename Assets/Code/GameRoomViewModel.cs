using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoBuf;
using protocol;

namespace game
{
	public class GameRoomViewModel : ViewModel
	{
		private GameRoomView m_View;
		private GameRoomModel m_Model;
		private PrepareModel m_PrepareModel;

		public bool[,] m_ChessManaul {
			get {
				return this.m_PrepareModel.m_ChessManaul;
			}
			set {
				this.m_PrepareModel.m_ChessManaul = value;
			}
		}

		public bool[,] m_ReceiveChessManaulOwn {
			get {
				return this.m_Model.m_ReceiveChessManaulOwn;
			}
			set {
				this.m_Model.m_ReceiveChessManaulOwn = value;
			}
		}

		public bool[,] m_ReceiveChessManaulOther {
			get {
				return this.m_Model.m_ReceiveChessManaulOther;
			}
			set {
				this.m_Model.m_ReceiveChessManaulOther = value;
			}
		}

		public bool[,] m_ReceiveChessManaulOwnHit {
			get {
				return this.m_Model.m_ReceiveChessManaulOwnHit;
			}
			set {
				this.m_Model.m_ReceiveChessManaulOwnHit = value;
			}
		}

		public bool[,] m_ReceiveChessManaulOtherHit {
			get {
				return this.m_Model.m_ReceiveChessManaulOtherHit;
			}
			set {
				this.m_Model.m_ReceiveChessManaulOtherHit = value;
			}
		}

		private bool m_IsPlay;
		public bool M_IsPlay {
			get {
				return this.m_IsPlay;
			}
			set {
				this.m_IsPlay = value;
			}
		}

		void Awake()
		{
			this.m_Parent = GameManager.Instance.transform;
			this.m_View = this.gameObject.AddComponent<GameRoomView>();
			this.m_Model = GameManager.GetModel<GameRoomModel>();
			this.m_PrepareModel = GameManager.GetModel<PrepareModel>();
			NetManager.ListenerMsg(ProtoDefine.HitChess, HitChessCallBack);
			NetManager.ListenerMsg(ProtoDefine.WinGame, WinGameCallBack);
		}

		public void StartGame(bool isPlay)
		{
			this.m_Model.RefreshGameRoomModel();
			this.m_View.StartGame(isPlay);
			this.m_IsPlay = isPlay;
			Debug.LogError("StartGame " + isPlay);
		}

		private void WinGameCallBack(IExtensible msgData) {
			WinGame winGame = (WinGame)msgData;
			Debug.LogError("Win!!!!!!!!!!!!" + winGame.win);
			if (winGame.win)
				Debug.LogError("Win!!!!!!!!!!!!");
		}

		private void HitChessCallBack(IExtensible msgData) {
			HitChess hitChess = (HitChess)msgData;
			Debug.LogError("HitChess " + hitChess.hit + "  " + hitChess.x + "  " + hitChess.y + "  " + this.m_IsPlay);
			if (this.m_IsPlay)
			{
				if (hitChess.hit)
				{
					this.m_ReceiveChessManaulOtherHit[hitChess.x, hitChess.y] = true;
					StartCoroutine(this.m_View.SetBoardPlay());
				}
				else
				{
					this.m_ReceiveChessManaulOther[hitChess.x, hitChess.y] = true;
					StartCoroutine(this.m_View.SetBoardNotPlay());
				}
			}
			else {
				if (hitChess.hit)
				{
					this.m_ReceiveChessManaulOwnHit[hitChess.x, hitChess.y] = true;
					StartCoroutine(this.m_View.SetBoardNotPlay());
				}
				else
				{
					this.m_ReceiveChessManaulOwn[hitChess.x, hitChess.y] = true;
					StartCoroutine(this.m_View.SetBoardPlay());
				}
			}
			this.m_View.SetChessPresent((int)hitChess.x, (int)hitChess.y,hitChess.hit);
		}

		public void ReqChessLocation(int placeX,int placeY) {
			ChessLocation msgData = new ChessLocation
			{
				x = (uint)placeX,
				y = (uint)placeY
			};
			NetManager.SendMsg(ProtoDefine.ChessLocation, msgData);
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.A))
			{

			}
		}
	}
}
