public enum TurnState
{
	Ready,
	Moving,
	SelectingAction,
	AnimatingAction,
}

public class GameState
{
	public TurnState TState;
	public Character SelectedCharacter;
}
