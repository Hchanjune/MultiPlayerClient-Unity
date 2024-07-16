// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 2.0.34
// 

using Colyseus.Schema;
using Action = System.Action;

public partial class ChatRoomInfo : Schema {
	[Type(0, "string")]
	public string roomId = default(string);

	[Type(1, "string")]
	public string roomName = default(string);

	[Type(2, "string")]
	public string roomOwner = default(string);

	[Type(3, "uint8")]
	public byte maxClients = default(byte);

	[Type(4, "boolean")]
	public bool isPrivate = default(bool);

	[Type(5, "string")]
	public string password = default(string);

	[Type(6, "uint8")]
	public byte currentPlayers = default(byte);

	[Type(7, "boolean")]
	public bool isPlaying = default(bool);

	/*
	 * Support for individual property change callbacks below...
	 */

	protected event PropertyChangeHandler<string> __roomIdChange;
	public Action OnRoomIdChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
		if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
		__callbacks.AddPropertyCallback(nameof(this.roomId));
		__roomIdChange += __handler;
		if (__immediate && this.roomId != default(string)) { __handler(this.roomId, default(string)); }
		return () => {
			__callbacks.RemovePropertyCallback(nameof(roomId));
			__roomIdChange -= __handler;
		};
	}

	protected event PropertyChangeHandler<string> __roomNameChange;
	public Action OnRoomNameChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
		if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
		__callbacks.AddPropertyCallback(nameof(this.roomName));
		__roomNameChange += __handler;
		if (__immediate && this.roomName != default(string)) { __handler(this.roomName, default(string)); }
		return () => {
			__callbacks.RemovePropertyCallback(nameof(roomName));
			__roomNameChange -= __handler;
		};
	}

	protected event PropertyChangeHandler<string> __roomOwnerChange;
	public Action OnRoomOwnerChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
		if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
		__callbacks.AddPropertyCallback(nameof(this.roomOwner));
		__roomOwnerChange += __handler;
		if (__immediate && this.roomOwner != default(string)) { __handler(this.roomOwner, default(string)); }
		return () => {
			__callbacks.RemovePropertyCallback(nameof(roomOwner));
			__roomOwnerChange -= __handler;
		};
	}

	protected event PropertyChangeHandler<byte> __maxClientsChange;
	public Action OnMaxClientsChange(PropertyChangeHandler<byte> __handler, bool __immediate = true) {
		if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
		__callbacks.AddPropertyCallback(nameof(this.maxClients));
		__maxClientsChange += __handler;
		if (__immediate && this.maxClients != default(byte)) { __handler(this.maxClients, default(byte)); }
		return () => {
			__callbacks.RemovePropertyCallback(nameof(maxClients));
			__maxClientsChange -= __handler;
		};
	}

	protected event PropertyChangeHandler<bool> __isPrivateChange;
	public Action OnIsPrivateChange(PropertyChangeHandler<bool> __handler, bool __immediate = true) {
		if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
		__callbacks.AddPropertyCallback(nameof(this.isPrivate));
		__isPrivateChange += __handler;
		if (__immediate && this.isPrivate != default(bool)) { __handler(this.isPrivate, default(bool)); }
		return () => {
			__callbacks.RemovePropertyCallback(nameof(isPrivate));
			__isPrivateChange -= __handler;
		};
	}

	protected event PropertyChangeHandler<string> __passwordChange;
	public Action OnPasswordChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
		if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
		__callbacks.AddPropertyCallback(nameof(this.password));
		__passwordChange += __handler;
		if (__immediate && this.password != default(string)) { __handler(this.password, default(string)); }
		return () => {
			__callbacks.RemovePropertyCallback(nameof(password));
			__passwordChange -= __handler;
		};
	}

	protected event PropertyChangeHandler<byte> __currentPlayersChange;
	public Action OnCurrentPlayersChange(PropertyChangeHandler<byte> __handler, bool __immediate = true) {
		if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
		__callbacks.AddPropertyCallback(nameof(this.currentPlayers));
		__currentPlayersChange += __handler;
		if (__immediate && this.currentPlayers != default(byte)) { __handler(this.currentPlayers, default(byte)); }
		return () => {
			__callbacks.RemovePropertyCallback(nameof(currentPlayers));
			__currentPlayersChange -= __handler;
		};
	}

	protected event PropertyChangeHandler<bool> __isPlayingChange;
	public Action OnIsPlayingChange(PropertyChangeHandler<bool> __handler, bool __immediate = true) {
		if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
		__callbacks.AddPropertyCallback(nameof(this.isPlaying));
		__isPlayingChange += __handler;
		if (__immediate && this.isPlaying != default(bool)) { __handler(this.isPlaying, default(bool)); }
		return () => {
			__callbacks.RemovePropertyCallback(nameof(isPlaying));
			__isPlayingChange -= __handler;
		};
	}

	protected override void TriggerFieldChange(DataChange change) {
		switch (change.Field) {
			case nameof(roomId): __roomIdChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
			case nameof(roomName): __roomNameChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
			case nameof(roomOwner): __roomOwnerChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
			case nameof(maxClients): __maxClientsChange?.Invoke((byte) change.Value, (byte) change.PreviousValue); break;
			case nameof(isPrivate): __isPrivateChange?.Invoke((bool) change.Value, (bool) change.PreviousValue); break;
			case nameof(password): __passwordChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
			case nameof(currentPlayers): __currentPlayersChange?.Invoke((byte) change.Value, (byte) change.PreviousValue); break;
			case nameof(isPlaying): __isPlayingChange?.Invoke((bool) change.Value, (bool) change.PreviousValue); break;
			default: break;
		}
	}
}

