//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//namespace game {
//    public class VPCollectorManage : MonoBehaviour {
//        private static VPCollectorManage _instance;
//        public static VPCollectorManage Instance {
//            get {
//                if (_instance == null) {
//                    _instance = FindObjectOfType(typeof(VPCollectorManage)) as VPCollectorManage;
//                }
//                return _instance;
//            }
//        }
//        private Dictionary<string, viewPreference> VPDic = new Dictionary<string, viewPreference>();

//        public void AddVP(string name, viewPreference view) {
//            this.VPDic[name] = view;
//        }

//        public void RemoveVP(string VPName) {
//            this.VPDic.Remove(VPName);
//        }

//        public viewPreference GetVP(string VPName)
//        {
//            return this.VPDic[VPName];
//        }
//    }
//}
