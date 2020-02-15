using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace game
{
    public enum ChessType { 
        boat,
        ship,
        vessel
    }
    public class PrepareView : View
    {
        private PrepareViewModel m_ViewModel;
        private PrepareViewPreference m_Preference;
        private checherBoardViewPreference m_BoardPreferene;

        List<GameObject> m_ChessObjList = new List<GameObject>();

        private ChessFlashItemView m_currentObjectView;
        private int m_BoatCount = 4;
        private int m_ShipCount = 3;
        private int m_vesselCount = 2;
        void Awake()
        {
            this.m_ViewModel = this.gameObject.GetComponent<PrepareViewModel>();
            this.m_Preference = UIManager.Instance.GetUI("PrepareView").
                GetComponent<PrepareViewPreference>();
            this.m_BoardPreferene = GameManager.Instance.checherBoardViewPreference;
            ObjectPool pool = ObjectPoolManager.Instance.
                CreateObjectPool<ChessPool>(PoolType.Chess, this.m_BoardPreferene.transform);
            pool.Prefab = Resources.Load("Prefab/chess") as GameObject;
            this.m_currentObjectView = GameManager.Instance.ChessFlashItemView;
            this.m_Preference.btnBoat.onClick.AddListener(this.OnBtnBoatClick);
            this.m_Preference.btnShip.onClick.AddListener(this.OnBtnShipClick);
            this.m_Preference.btnVessel.onClick.AddListener(this.OnBtnVesselClick);
            this.m_Preference.btnRotate.onClick.AddListener(this.OnBtnRotateClick);
            this.m_Preference.btnPlace.onClick.AddListener(this.OnBtnPlaceClick);
            this.m_Preference.btnStart.onClick.AddListener(this.OnBtnStartClick);

            this.m_Preference.txtBoatCount.text = this.m_BoatCount.ToString();
            this.m_Preference.txtShipCount.text = this.m_ShipCount.ToString();
            this.m_Preference.txtVesselCount.text = this.m_vesselCount.ToString();
        }

        private void OnBtnStartClick() {
            this.m_ViewModel.StartGame();
            this.StartGame();
            this.m_currentObjectView.RemoveChess();
        }

        private void OnBtnBoatClick()
        {
            if (this.m_BoatCount <= 0) {
                return;
            }
            this.m_currentObjectView.
                SetChess(ChessType.boat, this.m_ViewModel.m_ChessManaul);
        }

        private void OnBtnShipClick()
        {
            if (this.m_ShipCount <= 0)
            {
                return;
            }
            this.m_currentObjectView.
                SetChess(ChessType.ship, this.m_ViewModel.m_ChessManaul);
        }

        private void OnBtnVesselClick()
        {
            if (this.m_vesselCount <= 0)
            {
                return;
            }
            this.m_currentObjectView.
                SetChess(ChessType.vessel, this.m_ViewModel.m_ChessManaul);
        }


        private void OnBtnRotateClick() {
            this.m_currentObjectView.SetRotate();
        }

        private void OnBtnPlaceClick() {
            if (!this.m_currentObjectView.CanPlace()) {
                return;
            }

            this.m_currentObjectView.RemoveChess();
            this.SetAndStorageChess();
            switch (this.m_currentObjectView.GetChessType()) {
                case ChessType.boat:
                    if (--this.m_BoatCount <= 0) {
                        this.m_currentObjectView.RemoveChess();
                    }
                    this.m_Preference.txtBoatCount.text = this.m_BoatCount.ToString();
                    break;
                case ChessType.ship:
                    if (--this.m_ShipCount <= 0)
                    {
                        this.m_currentObjectView.RemoveChess();
                    }
                    this.m_Preference.txtShipCount.text = this.m_ShipCount.ToString();
                    break;
                case ChessType.vessel:
                    if (--this.m_vesselCount <= 0)
                    {
                        this.m_currentObjectView.RemoveChess();
                    }
                    this.m_Preference.txtVesselCount.text = this.m_vesselCount.ToString();
                    break;
            }
        }

        private void SetAndStorageChess() {
            foreach (Vector2 item in this.m_currentObjectView.GetAllChessVector2())
            {
                GameObject obj =
                    ObjectPoolManager.Instance.GetGameObject(PoolType.Chess,
                            new Vector3(item.x,item.y, -1));
                this.m_ChessObjList.Add(obj);
            }
            foreach (Vector2Int item in this.m_currentObjectView.GetAllChessVector2Int())
            {
                this.m_ViewModel.StorageChessManaul(item);
            }
        }

        private void StartGame() {
            foreach (GameObject item in m_ChessObjList) {
                ObjectPoolManager.Instance.RemoveGameObject(PoolType.Chess, item);
            }
            UIManager.Instance.RemoveUI("PrepareView");
        }
    }
}
