// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 2.0.34
// 

using Colyseus.Schema;
using Action = System.Action;

public partial class LobbyState : Schema {
	[Type(0, "string")]
	public string initializedTimestamp = default(string);

	[Type(1, "map", typeof(MapSchema<ClientInfo>))]
	public MapSchema<ClientInfo> clients = new MapSchema<ClientInfo>();

	[Type(2, "map", typeof(MapSchema<ChatRoomState>))]
	public MapSchema<ChatRoomState> chatRooms = new MapSchema<ChatRoomState>();

	/*
	 * Support for individual property change callbacks below...
	 */

	protected event PropertyChangeHandler<string> __initializedTimestampChange;
	public Action OnInitializedTimestampChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
		if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
		__callbacks.AddPropertyCallback(nameof(this.initializedTimestamp));
		__initializedTimestampChange += __handler;
		if (__immediate && this.initializedTimestamp != default(string)) { __handler(this.initializedTimestamp, default(string)); }
		return () => {
			__callbacks.RemovePropertyCallback(nameof(initializedTimestamp));
			__initializedTimestampChange -= __handler;
		};
	}

	protected event PropertyChangeHandler<MapSchema<ClientInfo>> __clientsChange;
	public Action OnClientsChange(PropertyChangeHandler<MapSchema<ClientInfo>> __handler, bool __immediate = true) {
		if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
		__callbacks.AddPropertyCallback(nameof(this.clients));
		__clientsChange += __handler;
		if (__immediate && this.clients != null) { __handler(this.clients, null); }
		return () => {
			__callbacks.RemovePropertyCallback(nameof(clients));
			__clientsChange -= __handler;
		};
	}

	protected event PropertyChangeHandler<MapSchema<ChatRoomState>> __chatRoomsChange;
	public Action OnChatRoomsChange(PropertyChangeHandler<MapSchema<ChatRoomState>> __handler, bool __immediate = true) {
		if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
		__callbacks.AddPropertyCallback(nameof(this.chatRooms));
		__chatRoomsChange += __handler;
		if (__immediate && this.chatRooms != null) { __handler(this.chatRooms, null); }
		return () => {
			__callbacks.RemovePropertyCallback(nameof(chatRooms));
			__chatRoomsChange -= __handler;
		};
	}

	protected override void TriggerFieldChange(DataChange change) {
		switch (change.Field) {
			case nameof(initializedTimestamp): __initializedTimestampChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
			case nameof(clients): __clientsChange?.Invoke((MapSchema<ClientInfo>) change.Value, (MapSchema<ClientInfo>) change.PreviousValue); break;
			case nameof(chatRooms): __chatRoomsChange?.Invoke((MapSchema<ChatRoomState>) change.Value, (MapSchema<ChatRoomState>) change.PreviousValue); break;
			default: break;
		}
	}
}

