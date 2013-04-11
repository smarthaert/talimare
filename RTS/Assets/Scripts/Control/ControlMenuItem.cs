
public class ControlMenuItem {
	public string ControlCode { get; protected set; }
	public string DestinationMenu { get; protected set; }
	
	public ControlMenuItem(string controlCode, string destinationMenu) {
		ControlCode = controlCode;
		DestinationMenu = destinationMenu;
	}
}
