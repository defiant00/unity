public enum TurnState
{
	Idle,
	SelectMoveLocation,
	Moving,
}

public class GameState
{
	public TurnState TurnState;
	public Character SelectedCharacter;
	public int[,] MovementMap;
}
