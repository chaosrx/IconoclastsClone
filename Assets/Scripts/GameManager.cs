using UnityEngine;
using System.Collections;

public class GameManager : Single<GameManager> {

	public AudioManager audioManager;
	public InputManager inputManager;
	public Player player;

	void Awake()
	{
		Application.targetFrameRate = 60;
	}
}
