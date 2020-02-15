using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace game
{
	public class GameRoomModel : Model
	{
		public void RefreshGameRoomModel()
		{
			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					this.m_ReceiveChessManaulOwn[i, j] = false;
					this.m_ReceiveChessManaulOther[i, j] = false;
					this.m_ReceiveChessManaulOwnHit[i, j] = false;
					this.m_ReceiveChessManaulOtherHit[i, j] = false;
				}
			}
		}
		public bool[,] m_ReceiveChessManaulOwn = new bool[10, 10];
		public bool[,] m_ReceiveChessManaulOther = new bool[10, 10];
		public bool[,] m_ReceiveChessManaulOwnHit = new bool[10, 10];
		public bool[,] m_ReceiveChessManaulOtherHit = new bool[10, 10];
	}
}