using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
 
/// <summary>
/// state pushed on top of the GameManager when the player dies.
/// </summary>
public class GameOverState : AState
{
    public TrackManager trackManager;
    public Canvas canvas;

	public Leaderboard miniLeaderboard;
	public Leaderboard fullLeaderboard;
	
	public Scrollbar returnInfoBar;
    
    public float timeToAutoRestart = 7.5f;
    private float _timeToAutoRestart;

    private readonly List<string> _skiers = new() { "Didier", "Pirmin", "Beat", "Peter", "Carlo", "Vreni", "Erika", "Maria", "Lara", "Michaela" };

    public override void Enter(AState from)
    {
        canvas.gameObject.SetActive(true);

        var skierName = _skiers[Random.Range(0, _skiers.Count)];
        miniLeaderboard.playerEntry.inputName.text = $"{skierName} (YOU)";
		
		miniLeaderboard.playerEntry.score.text = trackManager.score.ToString();
		miniLeaderboard.Populate();

		CreditCoins();

		returnInfoBar.size = 0.0f;
		_timeToAutoRestart = timeToAutoRestart;
    }

	public override void Exit(AState to)
    {
        canvas.gameObject.SetActive(false);
        FinishRun();
    }

    public override string GetName()
    {
        return "GameOver";
    }

    public override void Tick()
    {
	    _timeToAutoRestart -= Time.deltaTime;
	    
	    returnInfoBar.size = 1.0f - (_timeToAutoRestart / timeToAutoRestart);
	    
        if (_timeToAutoRestart <= 0.0f)
		{
			GoToLoadout();
		}
    }

	public void OpenLeaderboard()
	{
		fullLeaderboard.forcePlayerDisplay = false;
		fullLeaderboard.displayPlayer = true;
		fullLeaderboard.playerEntry.playerName.text = miniLeaderboard.playerEntry.inputName.text;
		fullLeaderboard.playerEntry.score.text = trackManager.score.ToString();

		fullLeaderboard.Open();
    }


    public void GoToLoadout()
    {
        trackManager.isRerun = false;
		manager.SwitchState("Loadout");
    }

    protected void CreditCoins()
	{
		PlayerData.instance.Save();
	}

	protected void FinishRun()
    {

        PlayerData.instance.InsertScore(trackManager.score, miniLeaderboard.playerEntry.inputName.text );

        CharacterCollider.DeathEvent de = trackManager.characterController.characterCollider.deathData;

        PlayerData.instance.Save();

        trackManager.End();
    }

    //----------------
}
