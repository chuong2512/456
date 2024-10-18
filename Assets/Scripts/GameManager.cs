using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public UIManager uIManager;

	public ScoreManager scoreManager;

	[Header("Game settings")]
	[Space(5f)]
	public GameObject player;

	[Space(5f)]
	public int jumpForce;

	[Space(5f)]
	public GameObject leftSide;

	[Space(5f)]
	public GameObject rightSide;

	[Space(5f)]
	public float sidesOpeningSpeed;

	[Space(5f)]
	public float sidesClosingSpeed;

	[Space(5f)]
	public Color[] colorTable;

	[Space(5f)]
	public GameObject obstaclesPrefab;

	[Space(5f)]
	[Range(0.15f, 0.6f)]
	public float delayBetweenObstacles = 0.4f;

	public float minObstacleSpeed = 3f;

	public float maxObstacleSpeed = 8f;

	public float minAplitude = 0.5f;

	public float maxAmplitude = 2f;

	public float minLeftRightSpeed = 1f;

	public float maxLeftRightSpeed = 7f;

	[Space(5f)]
	public bool spawning;

	public bool inAir;

	private float sidesSpeed;

	private Vector3 screenSize;

	private Vector2 leftSideTargetPosition;

	private Vector2 rightSideTargetPosition;

	private GameObject obstacle;

	private float sideXStartPos;

	private float sideXClosePos;

	public Vector2 gravityTemp;

	public static GameManager Instance
	{
		get;
		set;
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		screenSize = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
		Application.targetFrameRate = 90;
		SetSides();
		ShowPlayer();
	}

	private void Update()
	{
		gravityTemp = Physics2D.gravity;
		if (uIManager.gameState != GameState.PLAYING || uIManager.IsButton())
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			if (!spawning)
			{
				spawning = true;
				inAir = false;
				CloseSides();
				ScoreManager.Instance.StartCounting();
				StartCoroutine(SpawnObstacle(delayBetweenObstacles));
			}
			if (!inAir)
			{
				inAir = true;
				if (player.transform.position.x < 0f)
				{
					Physics2D.gravity = new Vector2(9.8f, 0f);
					player.GetComponent<Rigidbody2D>().AddForce(new Vector2(jumpForce, 0f));
				}
				else
				{
					Physics2D.gravity = new Vector2(-9.8f, 0f);
					player.GetComponent<Rigidbody2D>().AddForce(new Vector2(-jumpForce, 0f));
				}
				return;
			}
		}
		if (leftSide.transform.position.x + sideXStartPos < 0.001f)
		{
			CloseSides();
		}
		leftSide.transform.position = Vector2.Lerp(leftSide.transform.position, leftSideTargetPosition, sidesSpeed);
		rightSide.transform.position = Vector2.Lerp(rightSide.transform.position, rightSideTargetPosition, sidesSpeed);
	}

	private IEnumerator SpawnObstacle(float delay)
	{
		while (spawning)
		{
			obstacle = UnityEngine.Object.Instantiate(obstaclesPrefab);
			obstacle.transform.position = new Vector3(UnityEngine.Random.Range(leftSide.transform.position.x + leftSide.transform.localScale.x / 2f + 0.75f, rightSide.transform.position.x - rightSide.transform.localScale.x / 2f - 0.75f), screenSize.y + 1f);
			obstacle.GetComponent<SpriteRenderer>().color = colorTable[Random.Range(0, colorTable.Length)];
			obstacle.GetComponent<Obstacle>().InitOBstacle(UnityEngine.Random.Range(minObstacleSpeed, maxObstacleSpeed), UnityEngine.Random.Range(minAplitude, maxAmplitude), UnityEngine.Random.Range(minLeftRightSpeed, maxLeftRightSpeed));
			yield return new WaitForSeconds(delayBetweenObstacles);
		}
	}

	public void ShowPlayer()
	{
		player.GetComponent<SpriteRenderer>().enabled = true;
		player.GetComponent<SpriteRenderer>().color = colorTable[Random.Range(0, colorTable.Length)];
		bool flag = (UnityEngine.Random.Range(0, 2) == 1) ? true : false;
		sideXStartPos = screenSize.x + leftSide.transform.localScale.x / 2f;
		sideXClosePos = leftSide.transform.localScale.x / 2f;
		if (flag)
		{
			player.transform.position = new Vector2(leftSide.transform.position.x + leftSide.transform.localScale.x / 2f + player.GetComponent<SpriteRenderer>().bounds.size.x / 2f, -2.5f);
			Physics2D.gravity = new Vector2(-9.8f, 0f);
			player.GetComponent<Player>().previosSide = 0;
		}
		else
		{
			player.transform.position = new Vector2(rightSide.transform.position.x - rightSide.transform.localScale.x / 2f - player.GetComponent<SpriteRenderer>().bounds.size.x / 2f, -2.5f);
			Physics2D.gravity = new Vector2(9.8f, 0f);
			player.GetComponent<Player>().previosSide = 1;
		}
	}

	public void RestartGame()
	{
		StopAllCoroutines();
		if (uIManager.gameState == GameState.PAUSED)
		{
			Time.timeScale = 1f;
		}
		ClearScene();
		ShowPlayer();
		scoreManager.ResetCurrentScore();
		inAir = false;
		spawning = true;
		uIManager.ShowGameplay();
		OpenSides();
		ScoreManager.Instance.StartCounting();
		StartCoroutine(SpawnObstacle(delayBetweenObstacles));
	}

	public void SetSides()
	{
		leftSide.transform.localScale = new Vector2(screenSize.x, 2f * screenSize.y);
		rightSide.transform.localScale = new Vector2(screenSize.x, 2f * screenSize.y);
		leftSide.transform.position = new Vector3(0f - screenSize.x - leftSide.GetComponent<SpriteRenderer>().bounds.size.x / 2f, 0f, 0f);
		rightSide.transform.position = new Vector3(screenSize.x + rightSide.GetComponent<SpriteRenderer>().bounds.size.x / 2f, 0f, 0f);
	}

	public void OpenSides()
	{
		sidesSpeed = sidesOpeningSpeed;
		leftSideTargetPosition = new Vector2(0f - sideXStartPos, leftSide.transform.position.y);
		rightSideTargetPosition = new Vector2(sideXStartPos, rightSide.transform.position.y);
	}

	public void CloseSides()
	{
		sidesSpeed = sidesClosingSpeed;
		leftSideTargetPosition = new Vector2(0f - sideXClosePos, leftSide.transform.position.y);
		rightSideTargetPosition = new Vector2(sideXClosePos, rightSide.transform.position.y);
	}

	public void ClearScene()
	{
		spawning = false;
		StopAllCoroutines();
		GameObject[] array = GameObject.FindGameObjectsWithTag("Obstacle");
		for (int i = 0; i < array.Length; i++)
		{
			UnityEngine.Object.Destroy(array[i]);
		}
	}

	public void GameOver()
	{
		if (uIManager.gameState == GameState.PLAYING)
		{
			ScoreManager.Instance.StopCounting();
			AudioManager.Instance.PlayEffects(AudioManager.Instance.gameOver);
			uIManager.ShowGameOver();
			player.gameObject.GetComponent<SpriteRenderer>().enabled = false;
			scoreManager.UpdateScoreGameover();
		}
	}
}
