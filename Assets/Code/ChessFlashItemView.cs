using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace game {
    public class ChessFlashItemView : MonoBehaviour {
        public GameObject child_1;
        public GameObject child_2;

        private checherBoardViewPreference m_BoardPreferene;
        private ChessFlashItemView m_currentObjectView;
        private Vector2Int m_CurrentObjectVetor = new Vector2Int(0, 0);
        private bool m_NeedUpdate = false;
        private bool M_NeedUpdate {
            get {
                return m_NeedUpdate;
            }
            set {
                if (!value) {
                    this.gameObject.SetActive(false);
                }
                this.m_NeedUpdate = value;
            }
        }
        private bool m_IsRotate = false;
        private ChessType m_ChessType = ChessType.boat;
        private bool[,] m_ChessManaul;

        void Awake()
        {
            this.m_BoardPreferene = GameManager.Instance.checherBoardViewPreference;
            this.transform.parent =
                m_BoardPreferene.transform;
            UpdateManager.Instance.AddToGetButton(
                typeof(PrepareView).ToString(), MouseButtonUpdate);
        }

        private void MouseButtonUpdate(Vector2 point)
        {
            if (!this.m_NeedUpdate)
            {
                return;
            }
            Vector2Int grid = this.CalculateWitchGrid(point);
            if (this.CanViewChess(grid))
            {
                this.m_CurrentObjectVetor = grid;
                this.gameObject.SetActive(true);
                this.transform.position =
                    this.CalculateGridePoint(grid);
            }
        }

        private Vector2Int CalculateWitchGrid(Vector2 point)
        {
            float hight = (this.m_BoardPreferene.top - this.m_BoardPreferene.bottom) / 10;
            float width = (this.m_BoardPreferene.right - this.m_BoardPreferene.left) / 10;
            int x = (int)((point.x - this.m_BoardPreferene.left) / width);
            int y = (int)((point.y - this.m_BoardPreferene.bottom) / hight);
            return new Vector2Int(x, y);
        }

        private bool CanViewChess(Vector2Int grid)
        {
            List<Vector2Int> offsetList = this.GetOffsetList();
            foreach (Vector2Int item in offsetList)
            {
                if (grid.x + item.x > GameManager.MAX_BROAD_SIZE ||
                    grid.x + item.x < 0 ||
                    grid.y + item.y > GameManager.MAX_BROAD_SIZE ||
                    grid.y + item.y < 0 ||
                    this.m_ChessManaul[grid.x + item.x, grid.y + item.y])
                {
                    return false;
                }
            }
            return true;
        }

        private Vector2 CalculateGridePoint(Vector2Int grid, int offset_x = 0, int offset_y = 0)
        {
            float hight = (this.m_BoardPreferene.top - this.m_BoardPreferene.bottom) / 10;
            float width = (this.m_BoardPreferene.right - this.m_BoardPreferene.left) / 10;
            float x = this.m_BoardPreferene.left + width / 2 + width * (grid.x + offset_x);
            float y = this.m_BoardPreferene.bottom + hight / 2 + hight * (grid.y + offset_y);
            return new Vector2(x, y);
        }

        private List<Vector2Int> GetOffsetList()
        {
            List<Vector2Int> offsetList = new List<Vector2Int>();
            switch (this.m_ChessType)
            {
                case ChessType.boat:
                    offsetList.Add(new Vector2Int(0, 0));
                    break;
                case ChessType.ship:
                    if (!m_IsRotate)
                    {
                        offsetList.Add(new Vector2Int(0, 0));
                        offsetList.Add(new Vector2Int(0, -1));
                    }
                    else
                    {
                        offsetList.Add(new Vector2Int(0, 0));
                        offsetList.Add(new Vector2Int(-1, 0));
                    }
                    break;
                case ChessType.vessel:
                    if (!m_IsRotate)
                    {
                        offsetList.Add(new Vector2Int(0, 0));
                        offsetList.Add(new Vector2Int(0, -1));
                        offsetList.Add(new Vector2Int(0, -2));
                    }
                    else
                    {
                        offsetList.Add(new Vector2Int(0, 0));
                        offsetList.Add(new Vector2Int(-1, 0));
                        offsetList.Add(new Vector2Int(-2, 0));
                    }
                    break;
            }
            return offsetList;
        }
        public void SetChess(ChessType chessType, bool[,] chessManaul) {
            this.m_ChessType = chessType;
            this.M_NeedUpdate = true;
            switch (chessType) {
                case ChessType.boat:
                    child_1.SetActive(false);
                    child_2.SetActive(false);
                    break;
                case ChessType.ship:
                    child_1.SetActive(true);
                    child_2.SetActive(false);
                    break;
                case ChessType.vessel:
                    child_1.SetActive(true);
                    child_2.SetActive(true);
                    break;
            }
            this.m_ChessManaul = chessManaul;
            this.gameObject.SetActive(false);
        }

        public void SetRotate() {
            this.m_IsRotate = !this.m_IsRotate;
            if (this.m_IsRotate)
            {
                this.transform.eulerAngles = new Vector3(0, 0, -90);
            }
            else
            {
                this.transform.eulerAngles = new Vector3(0, 0, 0);
            }
            this.gameObject.SetActive(false);
        }

        public bool CanPlace() {
            return this.gameObject.activeSelf;
        }

        public ChessType GetChessType(){
            return m_ChessType;
        }

        public List<Vector2> GetAllChessVector2() {
            List<Vector2Int> offsetList = this.GetOffsetList();
            List<Vector2> returnList = new List<Vector2>();
            foreach (Vector2Int item in offsetList)
            {
                returnList.Add(new Vector2(
                    this.CalculateGridePoint(this.m_CurrentObjectVetor, item.x, item.y).x,
                    this.CalculateGridePoint(this.m_CurrentObjectVetor, item.x, item.y).y));
            }
            return returnList;
        }

        public List<Vector2Int> GetAllChessVector2Int()
        {
            List<Vector2Int> offsetList = this.GetOffsetList();
            List<Vector2Int> returnList = new List<Vector2Int>();
            foreach (Vector2Int item in offsetList)
            {
                returnList.Add(new Vector2Int(
                   this.m_CurrentObjectVetor.x + item.x,
                   this.m_CurrentObjectVetor.y + item.y));
            }
            return returnList;
        }

        public void RemoveChess() {
            this.M_NeedUpdate = false;
        }
    }
}
