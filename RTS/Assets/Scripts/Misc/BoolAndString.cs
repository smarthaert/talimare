
public class BoolAndString {
	public bool Bool { get; set; }
	public string String { get; set; }
	
	public BoolAndString() {
		String = "";
	}
	
	public BoolAndString(bool boolean) {
		Bool = boolean;
		String = "";
	}
}
