// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 2.0.34
// 

using Colyseus.Schema;
using Action = System.Action;

public partial class ChatRoomPlayer : Schema {
	[Type(0, "string")]
	public string lobbySessionId = default(string);

	[Type(1, "string")]
	public string sessionId = default(string);

	[Type(2, "string")]
	public string id = default(string);

	[Type(3, "string")]
	public string name = default(string);

	[Type(4, "string")]
	public string team = default(string);

	/*
	 * Support for individual property change callbacks below...
	 */

	protected event PropertyChangeHandler<string> __lobbySessionIdChange;
	public Action OnLobbySessionIdChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
		if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
		__callbacks.AddPropertyCallback(nameof(this.lobbySessionId));
		__lobbySessionIdChange += __handler;
		if (__immediate && this.lobbySessionId != default(string)) { __handler(this.lobbySessionId, default(string)); }
		return () => {
			__callbacks.RemovePropertyCallback(nameof(lobbySessionId));
			__lobbySessionIdChange -= __handler;
		};
	}

	protected event PropertyChangeHandler<string> __sessionIdChange;
	public Action OnSessionIdChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
		if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
		__callbacks.AddPropertyCallback(nameof(this.sessionId));
		__sessionIdChange += __handler;
		if (__immediate && this.sessionId != default(string)) { __handler(this.sessionId, default(string)); }
		return () => {
			__callbacks.RemovePropertyCallback(nameof(sessionId));
			__sessionIdChange -= __handler;
		};
	}

	protected event PropertyChangeHandler<string> __idChange;
	public Action OnIdChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
		if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
		__callbacks.AddPropertyCallback(nameof(this.id));
		__idChange += __handler;
		if (__immediate && this.id != default(string)) { __handler(this.id, default(string)); }
		return () => {
			__callbacks.RemovePropertyCallback(nameof(id));
			__idChange -= __handler;
		};
	}

	protected event PropertyChangeHandler<string> __nameChange;
	public Action OnNameChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
		if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
		__callbacks.AddPropertyCallback(nameof(this.name));
		__nameChange += __handler;
		if (__immediate && this.name != default(string)) { __handler(this.name, default(string)); }
		return () => {
			__callbacks.RemovePropertyCallback(nameof(name));
			__nameChange -= __handler;
		};
	}

	protected event PropertyChangeHandler<string> __teamChange;
	public Action OnTeamChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
		if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
		__callbacks.AddPropertyCallback(nameof(this.team));
		__teamChange += __handler;
		if (__immediate && this.team != default(string)) { __handler(this.team, default(string)); }
		return () => {
			__callbacks.RemovePropertyCallback(nameof(team));
			__teamChange -= __handler;
		};
	}

	protected override void TriggerFieldChange(DataChange change) {
		switch (change.Field) {
			case nameof(lobbySessionId): __lobbySessionIdChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
			case nameof(sessionId): __sessionIdChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
			case nameof(id): __idChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
			case nameof(name): __nameChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
			case nameof(team): __teamChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
			default: break;
		}
	}
}

