using System.Collections;
using Common;

public class HudManager : Manager<HudManager> {
	#region Manager implementation

	protected override IEnumerator InitCoroutine() {
		yield break;
	}

	#endregion

	#region Callbacks to GameManager events

	protected override void OnGameStatisticsChanged(GameStatisticsChangedEvent e) {
		// TODO
	}

	#endregion

	// [Header("HudManager")]

	#region Labels & Values

	// TODO

	#endregion
}
