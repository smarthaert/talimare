
public class ControlMenuItem {
	public string ControlCode { get; protected set; }
	public Creatable Creatable { get; protected set; }
	public Control Control { get; protected set; }
	public string DestinationMenu { get; protected set; }
	
	// Holds whether this ControlMenuItem is enabaled or not. If not, also holds a string reason why not.
	public BoolAndString Enabled { get; set; }
	
	public ControlMenuItem(string controlCode, string destinationMenu) {
		ControlCode = controlCode;
		Control = ControlStore.ControlMap[ControlCode];
		DestinationMenu = destinationMenu;
		Enabled = new BoolAndString(true);
	}
	
	public ControlMenuItem(Creatable creatable, string destinationMenu) {
		Creatable = creatable;
		ControlCode = Creatable.ControlCode;
		Control = ControlStore.ControlMap[ControlCode];
		DestinationMenu = destinationMenu;
		Enabled = new BoolAndString(true);
	}
}
