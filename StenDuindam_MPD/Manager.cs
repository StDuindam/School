using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour {

    //Singleton
    public static Manager Instance;

    //The manager
    public AudioManager audioManager;
    private GameOver gameOverCondition;
    private AbstractFactory platform;

    //gamestates
    public bool inStartGame = true;
    public bool inMainGame = false;
    public bool inTutorial = false;
    private bool isPaused = true;
    private Tutorial tutorial;
    public bool gameIsRunning = false;

    //Gamefield variables
    public int xWidth = 4;
    private int xRangeMax;
    public int yLength = 5;
    private int boardFloor = 0;
    private int depth = 0;
    private bool gridIsDrawn = false;
    private List<GameObject> grid = new List<GameObject>();
    public List<GameObject> activeBlocks = new List<GameObject>();
    [SerializeField]
    private Collider boardCap;

    //Block variables
    [SerializeField]
    private GameObject unbreakableBlock;
    private Transform gridParent;
    public BlockSpawner blockSpawner;
    public int hitsToBreak = 2;
    public float ghostLinger = 5;
    public float catLinger = 3;

    //Spawn conditions
    public float timeToSpawn;
    private int spawnHeightDifference = 2;
    private int colliderHeightDifference = 2;

    //Other / UI
    [SerializeField]
    private Text pauseText;
    [SerializeField]
    public GameObject cat1;
    [SerializeField]
    private GameObject neighbour;
    public int neighbourHits = 10;
    [SerializeField]
    private Text gameOver;
    [SerializeField]
    private Text yourScore;
    public Text currentScoreText;
    [SerializeField]
    private GameObject panel;
    //Event tresholds
    public int overTime = 20;
    public int topsyTurvey = 50;
    public float topsyTurveyTime = 10;

    public int timeyWimey = 75;
    public float timeyWimeyTime = 10;

    public int wrathOfTheNeighbour = 100;
    public GameObject theNeighbour;
    public bool isTheNeighbourActive = false;

    public int rainStorm = 20;
    private bool isRaining = false;
    public float rainStormTime = 15;

    public int hauntedMansion = 20;

    public int catpocalypse = 15;
    public int biohazard = 2;

    public int powerOfTheDarksite = 10;
    public float powerOftheDarsiteTimer = 0f;
    public float powerOfTheDarkSiteMaxTime = 10f;
    public int powerOfTheDarkSiteCap = 10;

    public bool isGod = false;
    public int amountToDestroy = 4;

    //Keep track of all blocks deleted
    public int blocksDestroyed = 0;//total blocks

    public int overTimeProgression = 0;
    public int topsyTurveyProgression = 0;
    public int timeyWimeyProgression = 0;
    public int wotnProgression = 0;

    public int waterDestroyed = 0;
    public int catDestroyed = 0;
    public int ghostDestroyed = 0;

    public int comboProgression;
    public float comboTime = 10;

    //Everything needed for the Arduino input
    public ArduinoRead arduino;
    public bool requestInput = false;

    //INIT AND UNITY FUNCTIONALITIES
    //Setup the game instance to be a singleton
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Debug.Log("You already have a Manager");
            Destroy(this);
        }
        //Get factory
        platform = AbstractFactory.GetInstance();
        platform.LoadBoardPreset();
        //Set a container for all the blocks to be spawned
        gridParent = GameObject.FindGameObjectWithTag("Blocks").GetComponent<Transform>();
        //Create a shortcut to the audio manager
        audioManager = this.gameObject.GetComponent<AudioManager>();
        //subscribe events
        EventManager.BasicEvent += Overtime;
        EventManager.BasicEvent += TopsyTurvey;
        EventManager.BasicEvent += WrathOfTheNeighbour;
        EventManager.BasicEvent += Biohazard;
        EventManager.BasicEvent += BlocksDestroyedTracker;
        EventManager.WaterEvent += Rainstorm;
        EventManager.GhostEvent += HauntedMansion;
        EventManager.CatEvent += Catpocalypse;
        EventManager.BasicEvent += TimeyWimey;
        //Game over limit
        boardCap.transform.position = new Vector3(xWidth/2,yLength+ colliderHeightDifference, 0);
    }

    //Begin the gameflow
    private void Start() {
        //draw a grid at the start
        audioManager.PlayMainTheme(songType.main);
        blockSpawner = this.gameObject.GetComponent<BlockSpawner>();
        tutorial = this.gameObject.GetComponent<Tutorial>();
        gameOverCondition = this.gameObject.GetComponentInChildren<GameOver>();
        }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.S)) {
            //StartGame();
            blocksDestroyed = 100;
            wotnProgression = 95;
            }

        //Check for input
        platform.CheckInput();

        //During tutorial call the tutorial script
        if (inTutorial) {
            tutorial.TutorialGame();
            }
        if (gameIsRunning) {
            currentScoreText.text = ("Score: " + blocksDestroyed.ToString());
        }
        //POWER OF THE DARKSIDE EVENT TIMER
        /*if (powerOftheDarsiteTimer < powerOfTheDarkSiteMaxTime) {
            powerOftheDarsiteTimer += Time.deltaTime;
            if (blocksDestroyed >= powerOfTheDarkSiteCap && !isGod && !inTutorial &&!isPaused) {
                    Debug.Log("You are a wizard");
                    PowerOfTheDarkSide();
                audioManager.EventSfx(eventSongType.powerofthedarkside);
                isGod = true;                
                }            
            }
        else {
            isGod = false;
            powerOftheDarsiteTimer = 0;
            powerOfTheDarkSiteCap = blocksDestroyed + powerOfTheDarksite;
            }*/
        }
           
    public void StartGame() {
        Debug.Log("START GAME");
        tutorial.TutorialScreen(false);
        inStartGame = false;
        isPaused = false;
        //REQUEST BOARD SPECS FROM ABSTRACT FACTORY
        DrawGrid(xWidth, yLength);
        blockSpawner.SetCoords(xRange(), yLength+ spawnHeightDifference);
        if (!gameIsRunning) {
            StartCoroutine(SpawnBlock(timeToSpawn));
            StartCoroutine(BlockCheck());
            }
        audioManager.PlayMainTheme(songType.game);
        }

        public void GameOver(string score) {
        StartCoroutine(EndGame(score));
        }

    public IEnumerator EndGame(string score) {
        isPaused = true;
        neighbour.SetActive(false);
        gameOver.text = ("Game over! Well done! The neighbour has been defeated!");
        yourScore.text = ("Your score is: " + score);
        gameOver.enabled = true;
        yourScore.enabled = true;
        panel.SetActive(true);
        requestInput = true;
        yield return new WaitWhile(()=> requestInput);//        yield return new WaitWhile(() => eventSfx.isPlaying);
        WhipeBoard();
        if (arduino != null) {
            arduino.setInput(false);
            }
        }

        public void ResetGame() {
        panel.SetActive(false);
        gameOver.enabled = false;
        yourScore.enabled = false;
        gameOverCondition.DisableText();
        inTutorial = false;
        inMainGame = false;
        inStartGame = true;
        blocksDestroyed = 0;
        tutorial.ResetTutorial();
        blockSpawner.ResetSpawner();
        tutorial.TutorialScreen(true);
        neighbour.SetActive(false);
        comboProgression = 0;
        overTimeProgression = 0;
        timeyWimeyProgression = 0;
        topsyTurveyProgression = 0;
        wotnProgression = 0;
        gameIsRunning = false;
        }
        //Cool gameover Thingy
        //Continue?
        //Quit
    public void WhipeBoard() {
        for (int i = 0; i < activeBlocks.Count; i++) {
            if (activeBlocks[i]) {
                activeBlocks[i].SetActive(false);
                }
            }
        if (activeBlocks.Count != 0) {
            WhipeBoard();
            return;
            }

        /*else {
            ResetGame();
            }*/
        }

//END OF INIT

//BOARD
    public void DrawGrid(int xSize, int ySize) {
        //Delete the older grid if there is one
        if (gridIsDrawn) {
            for (int i = 0; i < grid.Count; i++) {
                Destroy(grid[i]);
                }
            grid.Clear();
            }

        //Draw a new grid
        for (int i = 0; i < xSize; i++) {
            GameObject obj = Instantiate(unbreakableBlock, new Vector3(i, boardFloor, depth), Quaternion.identity);
            grid.Add(obj);
            obj.transform.parent = gridParent;
            }

        xWidth = xSize;
        yLength = ySize;
        //Set the coords on the BlockSpawner script
        blockSpawner.SetCoords(xRange(), yLength);
        gridIsDrawn = true;
        }

    //Get the max range to spawn the field
    private int xRange() {
        xRangeMax = xWidth - 1;
        return xRangeMax;
        }
    //Add a block to the activeblocks list
    public void addActiveBlock(GameObject block) {
        activeBlocks.Add(block);
        }
    //And delete one
    public void removeActiveBlock(GameObject block) {
        activeBlocks.Remove(block);
        }
    //Once every interval check if every active block is not stuck
    public IEnumerator BlockCheck() {

        yield return new WaitForSeconds(1);
        if (activeBlocks.Count != 0) {
            for (int i = 0; i < activeBlocks.Count; i++) {
                activeBlocks[i].GetComponent<BaseClass>().rayCastDown();
            }
            StartCoroutine(BlockCheck());
        }
    }

    //END OF BOARD

    //ARDUINO

    public void AddArduinoComponent() {
        this.gameObject.AddComponent<ArduinoRead>();
        }
    //END OF ARDUINO

    //WINDOWS

    /*
    public void MouseHit(Vector3 pos) {
		//determine location of camera and clicked position (assumes the blocks are at z = 0)
		Vector3 MouseLocation = Input.mousePosition;
		Vector3 sourcePos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
		MouseLocation.z = -sourcePos.z;
		Vector3 targetPos = Camera.main.ScreenToWorldPoint(MouseLocation);

		//determine direction of raycast
		Vector3 direction = targetPos - sourcePos;

		//make the actual raycast
		Debug.DrawRay(sourcePos, direction, Color.red);
		RaycastHit hit;
		if (Physics.Raycast(sourcePos, direction, out hit, 200f)) {
			if (hit.transform.GetComponent<BaseClass>()) {
				hit.transform.GetComponent<BaseClass>().Hit();
			}

            if (hit.transform.GetComponent<NeighbourScript>()) {
                Debug.Log("HIT!");
                hit.transform.GetComponent<NeighbourScript>().HitTheNeighbour();
                }
           if(hit.transform.tag == ("Cat")) {
                audioManager.Sfx(blockType.cat);
                }
		}	*/
	
    //END OF WINDOWS

    //BLOCK EXTRA FUNCTIONS
    //Function to change two blocks of position
    public Vector3 switchBlocks(GameObject block) {
        Vector3 tempVector = block.transform.position;
        Vector3 newPos = new Vector3(0, 0, 0);
        if (activeBlocks.Count > 2) {
            int switchInt = Random.Range(0, activeBlocks.Count);
            if (activeBlocks[switchInt] != block && activeBlocks[switchInt].transform.position.y < yLength-2) {
                newPos = activeBlocks[switchInt].transform.position;
                blockSpawner.SpawnParticle(blockType.swap, newPos);
                block.transform.position = newPos;
                activeBlocks[switchInt].transform.position = tempVector;
                blockSpawner.SpawnParticle(blockType.swap, tempVector);
                activeBlocks[switchInt].GetComponent<BaseClass>().SwitchState(false);
                block.GetComponent<BaseClass>().SwitchState(false);
                }
            else { switchBlocks(block); }
            return newPos;
            }
        else {
            return block.transform.position;
            }
        }
    //Spawn the blocks using the blockspawner, recursive
    IEnumerator SpawnBlock(float timeToSpawn) {
        blockSpawner.SpawnBlocks();
        yield return new WaitForSeconds(getSpawnTime());
        if (!isPaused) {
            StartCoroutine(SpawnBlock(getSpawnTime()));
            }
        }
    //start a cat blocking the screen
    public void callCat() {
        StartCoroutine(activateCat());
        }

    IEnumerator activateCat() {
        cat1.SetActive(true);            
        yield return new WaitForSeconds(2);
        cat1.SetActive(false);
        }
    //Add or subtract from the current time to spawn for a given duration
    public void callTimeEdit(float spawnEdit, float timeExtended) {
        StartCoroutine(spawnTimeBuff(spawnEdit, timeExtended));
        }
    public IEnumerator spawnTimeBuff(float spawnEdit, float timeExtended) {
        timeToSpawn += spawnEdit;
        yield return new WaitForSeconds(timeExtended);
        timeToSpawn -= spawnEdit;
        }
    //easy way to spawn a normal block
    public void SpawnNormalBlock(Vector3 position) {
        blockSpawner.SpawnNormalBlock(position);
        }
//END OF BLOCK EXTRAS

//EVENTS
    public void Overtime() {
        //normal blocks
        if(!inTutorial && !isPaused)
            overTimeProgression++;
        
        if(overTimeProgression == overTime) {
            Debug.Log("OverTime");
            audioManager.EventSfx(eventSongType.overtime);
            isPaused = true;
            for(int i = 0; i < xWidth; i++) {
                SpawnNormalBlock(new Vector3(i, yLength, depth));
            }
            overTimeProgression = 0;
            isPaused = false;
        }
        }
    public void TopsyTurvey() {
        //normal blocks
        if (!inTutorial)
            topsyTurveyProgression++;
        if(topsyTurveyProgression == topsyTurvey) {
            Debug.Log("Topsy Turvey");
            audioManager.EventSfx(eventSongType.topsyturvey);
            StartCoroutine(callTopsyTurvey());
        }
        }
    IEnumerator callTopsyTurvey() {
        blockSpawner.isTopsyTurvy = true;
        yield return new WaitForSeconds(topsyTurveyTime);
        blockSpawner.isTopsyTurvy = false;
        topsyTurveyProgression = 0;
    }

    public void TimeyWimey() {
        //normal blocks
        if (!inTutorial) {
            timeyWimeyProgression++;
        }
        if(timeyWimeyProgression == timeyWimey && !inTutorial) {
            {
                Debug.Log("Timey Wimey");
                audioManager.EventSfx(eventSongType.timeywimey);
                StartCoroutine(callTimeyWimey());
            }
        }
    }
    IEnumerator callTimeyWimey() {
        float tempTimeToSpawn;
        tempTimeToSpawn = timeToSpawn;
        timeToSpawn = timeToSpawn * 0.75f;
        yield return new WaitForSeconds(timeyWimeyTime);
        timeToSpawn = tempTimeToSpawn;
        timeyWimeyProgression = 0;
    }

    public void WrathOfTheNeighbour() {
        //normal blocks
        if (!inTutorial)
            wotnProgression++;
        if(wotnProgression == wrathOfTheNeighbour) {
            Debug.Log("WRATH OF THE FUCKING NEIGHBOUR");
            audioManager.EventSfx(eventSongType.wotn);
            isPaused = true;
            isTheNeighbourActive = true;
            neighbour.GetComponent<NeighbourScript>().neighbourLife = neighbourHits;
            neighbour.SetActive(true);
            wotnProgression = 0;
            }
        }

    public void Rainstorm() {
        //water blocks
        if (!inTutorial)
            waterDestroyed++;
        if(waterDestroyed == rainStorm) {
            Debug.Log("Rainstorm");
            audioManager.EventSfx(eventSongType.rainstorm);
            if (!isRaining) {
                StartCoroutine(timeRainstorm());
                StartCoroutine(callRainStorm());
                isRaining = true;
                }
            }
        }

    IEnumerator callRainStorm() {
        blockSpawner.SpawnSpecificBlock(blockType.water);
        yield return new WaitForSeconds(1);
        if (isRaining) {
            StartCoroutine(callRainStorm());
            }
        else {
            isRaining = false;
            }
        }

    IEnumerator timeRainstorm() {
        yield return new WaitForSeconds(rainStormTime);
        isRaining = false;
        waterDestroyed = 0;
        }

    public void HauntedMansion() {
        //ghost blocks
        ghostDestroyed++;
        }
    public void Catpocalypse() {
        //cat blocks
        catDestroyed++;
        //audioManager.EventSfx(eventSongType.catpocalypse);
        }
    public void Biohazard() {
        //lack of active blocks
        if (!inTutorial && !isPaused) {
            if (activeBlocks.Count <= 2) {
                Debug.Log("Biohazard");
                audioManager.EventSfx(eventSongType.biohazard);
                blockSpawner.SpawnSpecificBlock(blockType.slime);
                blockSpawner.SpawnSpecificBlock(blockType.slime);
                }
            }
        }
    public void PowerOfTheDarkSide() {
        for (int i = 0; i < amountToDestroy; i++) {
            if (i < activeBlocks.Count) {
                activeBlocks[i].SetActive(false);
                }
            }
        }

    public void BlocksDestroyedTracker() {
        blocksDestroyed++;
        }
//END OF EVENTS

    //Easy way to return and set some board specifics
    public int getBoardWidth() {
        return xWidth;
        }
    public int getBoardHeight() {
        return yLength;
        }
    public float getSpawnTime() {
        return timeToSpawn;
        }
    public float setSpawnTime(float newTimeToSpawn) {
        timeToSpawn = newTimeToSpawn;
        return timeToSpawn;
        }
     
    //LINKING STUFF TO BUTTONS TO DEBUG, TEST AND WHAT NOT :)

    public void UpdateGrid() {
        DrawGrid(xWidth, yLength);
        }

    public void PauseTheGame() {
        if (isPaused && !gameIsRunning) {
            StartCoroutine(SpawnBlock(timeToSpawn));
            //pauseText.enabled = false;
            gameIsRunning = true;
            isPaused = false;
        }
        else {
            //pauseText.enabled = true;

            gameIsRunning = false;
            isPaused = true;
            }
        }

    public Transform GetParent() {
        return gridParent;
        }
    }
//End of script
