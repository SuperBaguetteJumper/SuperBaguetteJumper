using System.Collections;
using Common;

public class LevelManager : Manager<LevelManager> {
	#region Manager implementation

	protected override IEnumerator InitCoroutine() {
		yield break;
	}

	#endregion

	protected override void OnGamePlay(GamePlayEvent e) {}

	protected override void OnGameMenu(GameMenuEvent e) {}
}
