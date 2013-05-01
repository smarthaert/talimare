
public class ControlMenuItem {
	public string ControlCode { get; protected set; }
	public Creatable Creatable { get; protected set; }
	public Control Control { get; protected set; }
	public string DestinationMenu { get; protected set; }
	public bool RequiresPower { get; protected set; }
	
	// Holds whether this ControlMenuItem is enabaled or not. If not, also holds a string reason why not.
	public BoolAndString Enabled { get; set; }
	
	
	public ControlMenuItem(Creatable creatable)
	: this(creatable, null, false) {}
	
	public ControlMenuItem(Creatable creatable, string destinationMenu)
	: this(creatable, destinationMenu, false) {}
	
	public ControlMenuItem(Creatable creatable, bool requiresPower)
	: this(creatable, null, requiresPower) {}
	
	public ControlMenuItem(Creatable creatable, string destinationMenu, bool requiresPower)
	: this(creatable.ControlCode, destinationMenu, requiresPower) {
		Creatable = creatable;
	}
	
	public ControlMenuItem(string controlCode)
	: this(controlCode, null, false) {}
	
	public ControlMenuItem(string controlCode, string destinationMenu)
	: this(controlCode, destinationMenu, false) {}
	
	public ControlMenuItem(string controlCode, bool requiresPower)
	: this(controlCode, null, requiresPower) {}
	
	public ControlMenuItem(string controlCode, string destinationMenu, bool requiresPower) {
		ControlCode = controlCode;
		Control = ControlStore.ControlMap[ControlCode];
		DestinationMenu = destinationMenu;
		RequiresPower = requiresPower;
		Enabled = new BoolAndString(true);
	}
}
