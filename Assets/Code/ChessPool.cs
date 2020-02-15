using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace game {
    public class ChessPool : ObjectPool
    {
        public override GameObject Get(Vector3 pos)
        {
            GameObject obj;
            obj = base.Get(pos);

            return obj;
        }
    }
}
