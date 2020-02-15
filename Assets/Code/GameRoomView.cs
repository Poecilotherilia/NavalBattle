using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace game
{
    public class GameRoomView : View
    {
        private GameRoomViewModel m_ViewModel;
        private checherBoardViewPreference m_BoardPreferene;
        private GameRoomViewPreference m_Preference;
        List<GameObject> m_ChessObjList = new List<GameObject>();

        private ChessFlashItemView m_currentObjectView;
        void Awake()
        {
            this.m_ViewModel = this.gameObject.GetComponent<GameRoomViewModel>();
            this.m_Preference = UIManager.Instance.GetUI("GameRoomView").
                GetComponent<GameRoomViewPreference>();
            this.m_BoardPreferene = GameManager.Instance.checherBoardViewPreference;
            GameObject chessPreb =
                    Resources.Load("Prefab/chessFlash") as GameObject; ;
            this.m_currentObjectView = GameManager.Instance.ChessFlashItemView;
            this.m_Preference.btnPlace.onClick.AddListener(this.OnBtnPlaceClick);
        }

        public void StartGame(bool isPlay) {
            if (isPlay)
            {
                this.m_currentObjectView.
                    SetChess(ChessType.boat,
                    this.MixManaul(this.m_ViewModel.m_ReceiveChessManaulOther,
                    this.m_ViewModel.m_ReceiveChessManaulOtherHit));
            }
        }

        private void OnBtnPlaceClick()
        {
            if (!this.m_currentObjectView.CanPlace())
            {
                return;
            }

            this.m_currentObjectView.RemoveChess();
            List<Vector2Int> vector2Ints = 
                this.m_currentObjectView.GetAllChessVector2Int();
            this.m_ViewModel.ReqChessLocation(vector2Ints[0].x, vector2Ints[0].y);
        }

        public void SetChessPresent(int x, int y, bool hit) {
            GameObject obj =
                    ObjectPoolManager.Instance.GetGameObject(PoolType.Chess,
                            this.CalculateGridePoint(x, y));
            obj.GetComponent<SpriteRenderer>().color =
                hit ? (this.m_ViewModel.M_IsPlay ? Color.green : Color.red) : Color.black;
            this.m_ChessObjList.Add(obj);
        }

        public IEnumerator SetBoardPlay(){
            Debug.LogError("SetBoardPlay");
            yield return new WaitForSeconds(2);
            foreach (GameObject item in this.m_ChessObjList) {
                ObjectPoolManager.Instance.RemoveGameObject(PoolType.Chess, item);
            }
            this.m_ViewModel.M_IsPlay = true;
            for (int i = 0; i < 10; i++) {
                for (int j = 0; j < 10; j++) {
                    if (this.m_ViewModel.m_ReceiveChessManaulOtherHit[i,j]) {
                        Debug.Log("Hit  :"+i+" "+j);
                        GameObject obj =
                    ObjectPoolManager.Instance.GetGameObject(PoolType.Chess,
                            this.CalculateGridePoint(i, j));
                        obj.GetComponent<SpriteRenderer>().color = Color.green;
                        this.m_ChessObjList.Add(obj);
                    }
                }
            }
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (this.m_ViewModel.m_ReceiveChessManaulOther[i, j])
                    {
                        Debug.Log("NotHit  :" + i + " " + j);
                        GameObject obj =
                    ObjectPoolManager.Instance.GetGameObject(PoolType.Chess,
                            this.CalculateGridePoint(i, j));
                        obj.GetComponent<SpriteRenderer>().color = Color.black;
                        this.m_ChessObjList.Add(obj);
                    }
                }
            }
            this.m_currentObjectView.
                    SetChess(ChessType.boat,
                    this.MixManaul(this.m_ViewModel.m_ReceiveChessManaulOther,
                    this.m_ViewModel.m_ReceiveChessManaulOtherHit));
        }

        public IEnumerator SetBoardNotPlay()
        {
            Debug.LogError("SetBoardNotPlay");
            yield return new WaitForSeconds(2);
            foreach (GameObject item in this.m_ChessObjList)
            {
                ObjectPoolManager.Instance.RemoveGameObject(PoolType.Chess, item);
            }
            this.m_ViewModel.M_IsPlay = false;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (this.m_ViewModel.m_ReceiveChessManaulOwnHit[i, j])
                    {
                        Debug.Log("Hit  :" + i + " " + j);
                        GameObject obj =
                    ObjectPoolManager.Instance.GetGameObject(PoolType.Chess,
                            this.CalculateGridePoint(i, j));
                        obj.GetComponent<SpriteRenderer>().color = Color.red;
                        this.m_ChessObjList.Add(obj);
                    }
                }
            }
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (this.m_ViewModel.m_ReceiveChessManaulOwn[i, j])
                    {
                        Debug.Log("NotHit  :" + i + " " + j);
                        GameObject obj =
                    ObjectPoolManager.Instance.GetGameObject(PoolType.Chess,
                            this.CalculateGridePoint(i, j));
                        obj.GetComponent<SpriteRenderer>().color = Color.black;
                        this.m_ChessObjList.Add(obj);
                    }
                }
            }
            this.m_currentObjectView.RemoveChess();
        }

        private Vector3 CalculateGridePoint(int x,int y)
        {
            float hight = (this.m_BoardPreferene.top - this.m_BoardPreferene.bottom) / 10;
            float width = (this.m_BoardPreferene.right - this.m_BoardPreferene.left) / 10;
            float returnX = this.m_BoardPreferene.left + width / 2 + width * x;
            float returnY = this.m_BoardPreferene.bottom + hight / 2 + hight * y;
            return new Vector3(returnX, returnY ,-1);
        }

        private bool[,] MixManaul(bool[,] vs1,bool[,] vs2) {
            bool[,] returnVs = new bool[10, 10];
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (vs2[i,j] || vs1[i,j])
                    {
                        returnVs[i, j] = true;
                    }
                }
            }
            return vs1;
        }
    }
}
