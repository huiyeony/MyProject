using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }
	#endregion


	public Action<PacketSession, IMessage, ushort> _unityHandler;

	PacketManager()
	{
		Register();
	}

	Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();
		
	public void Register()
	{		
		_onRecv.Add((ushort)MsgId.SEnterGame, MakePacket<SEnterGame>);
		_handler.Add((ushort)MsgId.SEnterGame, PacketHandler.SEnterGameHandler);		
		_onRecv.Add((ushort)MsgId.SLeaveGame, MakePacket<SLeaveGame>);
		_handler.Add((ushort)MsgId.SLeaveGame, PacketHandler.SLeaveGameHandler);		
		_onRecv.Add((ushort)MsgId.SSpawn, MakePacket<SSpawn>);
		_handler.Add((ushort)MsgId.SSpawn, PacketHandler.SSpawnHandler);		
		_onRecv.Add((ushort)MsgId.SDespawn, MakePacket<SDespawn>);
		_handler.Add((ushort)MsgId.SDespawn, PacketHandler.SDespawnHandler);		
		_onRecv.Add((ushort)MsgId.SMove, MakePacket<SMove>);
		_handler.Add((ushort)MsgId.SMove, PacketHandler.SMoveHandler);		
		_onRecv.Add((ushort)MsgId.SSkill, MakePacket<SSkill>);
		_handler.Add((ushort)MsgId.SSkill, PacketHandler.SSkillHandler);		
		_onRecv.Add((ushort)MsgId.SChangeHp, MakePacket<SChangeHp>);
		_handler.Add((ushort)MsgId.SChangeHp, PacketHandler.SChangeHpHandler);		
		_onRecv.Add((ushort)MsgId.SDie, MakePacket<SDie>);
		_handler.Add((ushort)MsgId.SDie, PacketHandler.SDieHandler);		
		_onRecv.Add((ushort)MsgId.SConnected, MakePacket<SConnected>);
		_handler.Add((ushort)MsgId.SConnected, PacketHandler.SConnectedHandler);		
		_onRecv.Add((ushort)MsgId.SLogin, MakePacket<SLogin>);
		_handler.Add((ushort)MsgId.SLogin, PacketHandler.SLoginHandler);		
		_onRecv.Add((ushort)MsgId.SCreatePlayer, MakePacket<SCreatePlayer>);
		_handler.Add((ushort)MsgId.SCreatePlayer, PacketHandler.SCreatePlayerHandler);		
		_onRecv.Add((ushort)MsgId.SItemList, MakePacket<SItemList>);
		_handler.Add((ushort)MsgId.SItemList, PacketHandler.SItemListHandler);		
		_onRecv.Add((ushort)MsgId.SAddItem, MakePacket<SAddItem>);
		_handler.Add((ushort)MsgId.SAddItem, PacketHandler.SAddItemHandler);		
		_onRecv.Add((ushort)MsgId.SEquipItem, MakePacket<SEquipItem>);
		_handler.Add((ushort)MsgId.SEquipItem, PacketHandler.SEquipItemHandler);		
		_onRecv.Add((ushort)MsgId.SLevelUp, MakePacket<SLevelUp>);
		_handler.Add((ushort)MsgId.SLevelUp, PacketHandler.SLevelUpHandler);		
		_onRecv.Add((ushort)MsgId.SPing, MakePacket<SPing>);
		_handler.Add((ushort)MsgId.SPing, PacketHandler.SPingHandler);
	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		Action<PacketSession, ArraySegment<byte>, ushort> action = null;
		if (_onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
	}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{
		T pkt = new T();
		pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);

		
        if (_unityHandler != null)
		{
            _unityHandler.Invoke(session, pkt, id);
        }
		else
		{
            Action<PacketSession, IMessage> action = null;
            if (_handler.TryGetValue(id, out action))
                action.Invoke(session, pkt);
        }
        
	}

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			return action;
		return null;
	}
}