
public class ControlMenuItem {
	public string ControlCode { get; protected set; }
	public Control Control { get; protected set; }
	public string DestinationMenu { get; protected set; }
	
	//TODO high: add ability to disable menu items and a reason for the disabling
	
	public ControlMenuItem(string controlCode, string destinationMenu) {
		ControlCode = controlCode;
		Control = ControlStore.ControlMap[ControlCode];
		DestinationMenu = destinationMenu;
	}
}
