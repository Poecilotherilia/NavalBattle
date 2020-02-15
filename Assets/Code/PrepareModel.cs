using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace game
{
	public class PrepareModel : Model
	{
		public PrepareModel() {
			for (int i = 0; i < 10; i++) {
				for (int j = 0; j < 10; j++) {
					this.m_ChessManaul[i, j] = false;
				}
			}
		}
		public bool[,] m_ChessManaul = new bool[10, 10];
	}
}
