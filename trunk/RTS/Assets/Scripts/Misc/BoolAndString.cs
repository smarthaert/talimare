
public class BoolAndString {
	public bool Bool { get; set; }
	public string String { get; set; }
	
	public BoolAndString(bool boolean)
		: this(boolean, "") {}
	
	public BoolAndString(bool boolean, string str) {
		Bool = boolean;
		String = str;
	}
}
