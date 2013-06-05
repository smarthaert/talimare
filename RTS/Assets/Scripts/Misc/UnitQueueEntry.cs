
public class UnitQueueEntry {
	public Controllable UnitToConvert { get; protected set; }
	public CreatableUnit DestinationUnit { get; protected set; }
	
	public UnitQueueEntry(Controllable unitToConvert, CreatableUnit destinationUnit) {
		UnitToConvert = unitToConvert;
		DestinationUnit = destinationUnit;
	}
}