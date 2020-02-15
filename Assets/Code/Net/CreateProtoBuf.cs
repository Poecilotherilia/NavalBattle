//动态修改，不要手动修改

using game;
using protocol;
public class CreateProtoBuf
{
  public static ProtoBuf.IExtensible GetProtoData(ProtoDefine protoId, byte[] msgData)
  {
      switch (protoId)
      {
            case ProtoDefine.Handshake:
                return NetUtilcs.Deserialize<Handshake>(msgData);
            case ProtoDefine.StartGame:
                return NetUtilcs.Deserialize<StartGame>(msgData);
            case ProtoDefine.ChessManaul:
                return NetUtilcs.Deserialize<ChessManaul>(msgData);
            case ProtoDefine.HitChess:
                return NetUtilcs.Deserialize<HitChess>(msgData);
            case ProtoDefine.WinGame:
                return NetUtilcs.Deserialize<WinGame>(msgData);
            case ProtoDefine.ChessLocation:
                return NetUtilcs.Deserialize<ChessLocation>(msgData);
            case ProtoDefine.CloseConnect:
                return NetUtilcs.Deserialize<CloseConnect>(msgData);
          default:
              return null;
      }
  }
}
