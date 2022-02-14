using System.Collections;
using Common;

public class LevelManager : Manager<LevelManager> {
	#region Manager implementation

	protected override IEnumerator InitCoroutine() {
		yield break;
	}

	#endregion

	protected override void GamePlay(GamePlayEvent e) {}

	protected override void GameMenu(GameMenuEvent e) {}
}
