// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 2.0.34
// 

using Colyseus.Schema;
using Action = System.Action;

public partial class ChatMessage : Schema {
	[Type(0, "string")]
	public string senderId = default(string);

	[Type(1, "string")]
	public string senderName = default(string);

	[Type(2, "string")]
	public string senderTeam = default(string);

	[Type(3, "string")]
	public string message = default(string);

	[Type(4, "string")]
	public string timestamp = default(string);

	/*
	 * Support for individual property change callbacks below...
	 */

	protected event PropertyChangeHandler<string> __senderIdChange;
	public Action OnSenderIdChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
		if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
		__callbacks.AddPropertyCallback(nameof(this.senderId));
		__senderIdChange += __handler;
		if (__immediate && this.senderId != default(string)) { __handler(this.senderId, default(string)); }
		return () => {
			__callbacks.RemovePropertyCallback(nameof(senderId));
			__senderIdChange -= __handler;
		};
	}

	protected event PropertyChangeHandler<string> __senderNameChange;
	public Action OnSenderNameChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
		if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
		__callbacks.AddPropertyCallback(nameof(this.senderName));
		__senderNameChange += __handler;
		if (__immediate && this.senderName != default(string)) { __handler(this.senderName, default(string)); }
		return () => {
			__callbacks.RemovePropertyCallback(nameof(senderName));
			__senderNameChange -= __handler;
		};
	}

	protected event PropertyChangeHandler<string> __senderTeamChange;
	public Action OnSenderTeamChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
		if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
		__callbacks.AddPropertyCallback(nameof(this.senderTeam));
		__senderTeamChange += __handler;
		if (__immediate && this.senderTeam != default(string)) { __handler(this.senderTeam, default(string)); }
		return () => {
			__callbacks.RemovePropertyCallback(nameof(senderTeam));
			__senderTeamChange -= __handler;
		};
	}

	protected event PropertyChangeHandler<string> __messageChange;
	public Action OnMessageChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
		if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
		__callbacks.AddPropertyCallback(nameof(this.message));
		__messageChange += __handler;
		if (__immediate && this.message != default(string)) { __handler(this.message, default(string)); }
		return () => {
			__callbacks.RemovePropertyCallback(nameof(message));
			__messageChange -= __handler;
		};
	}

	protected event PropertyChangeHandler<string> __timestampChange;
	public Action OnTimestampChange(PropertyChangeHandler<string> __handler, bool __immediate = true) {
		if (__callbacks == null) { __callbacks = new SchemaCallbacks(); }
		__callbacks.AddPropertyCallback(nameof(this.timestamp));
		__timestampChange += __handler;
		if (__immediate && this.timestamp != default(string)) { __handler(this.timestamp, default(string)); }
		return () => {
			__callbacks.RemovePropertyCallback(nameof(timestamp));
			__timestampChange -= __handler;
		};
	}

	protected override void TriggerFieldChange(DataChange change) {
		switch (change.Field) {
			case nameof(senderId): __senderIdChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
			case nameof(senderName): __senderNameChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
			case nameof(senderTeam): __senderTeamChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
			case nameof(message): __messageChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
			case nameof(timestamp): __timestampChange?.Invoke((string) change.Value, (string) change.PreviousValue); break;
			default: break;
		}
	}
}

