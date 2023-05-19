using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// State pushed on the GameManager during the Loadout, when player select player, theme and accessories
/// Take care of init the UI, load all the data used for it etc.
/// </summary>
public class LoadoutState : AState
{

	public MeshFilter skyMeshFilter;
    public MeshFilter UIGroundFilter;
    public Text startInfo;
    public Text startInfoPercent;


	protected Modifier m_CurrentModifier = new Modifier();

    protected int k_UILayer;

    public override void Enter(AState from)
    {
	    k_UILayer = LayerMask.NameToLayer("UI");

        SetObjectsActive(true);

        // Reseting the global blinking value. Can happen if the game unexpectedly exited while still blinking
        Shader.SetGlobalFloat("_BlinkingValue", 0.0f);
    }

    private void SetObjectsActive(bool state)
    {
	    skyMeshFilter.gameObject.SetActive(state);
	    UIGroundFilter.gameObject.SetActive(state);
	    startInfo.gameObject.SetActive(state);
	    startInfoPercent.gameObject.SetActive(state);
    }

    public override void Exit(AState to)
    {
	    GameState gs = to as GameState;

	    SetObjectsActive(false);

        if (gs != null)
        {
			gs.currentModifier = m_CurrentModifier;
			
            // We reset the modifier to a default one, for next run (if a new modifier is applied, it will replace this default one before the run starts)
			m_CurrentModifier = new Modifier();
        }
    }

    public override string GetName()
    {
        return "Loadout";
    }

    public override void Tick()
    {
	    startInfoPercent.text = (BodyDetector.GetDetectionPercent() / 0.8).ToString("0.00");
		if (BodyDetector.GetDetectionOverThreshold())
		{
			StartGame();
		}
    }

    public void StartGame()
    {
        if (PlayerData.instance.tutorialDone)
        {
            if (PlayerData.instance.ftueLevel == 1)
            {
                PlayerData.instance.ftueLevel = 2;
                PlayerData.instance.Save();
            }
        }

        manager.SwitchState("Game");
    }
}
