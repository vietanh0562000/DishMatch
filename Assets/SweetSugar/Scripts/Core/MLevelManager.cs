using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SweetSugar.Scriptable.Rewards;
using SweetSugar.Scripts.AdsEvents;
using SweetSugar.Scripts.Blocks;
using SweetSugar.Scripts.Effects;
using SweetSugar.Scripts.GUI;
using SweetSugar.Scripts.GUI.Boost;
using SweetSugar.Scripts.Integrations;
using SweetSugar.Scripts.Integrations.Network;
using SweetSugar.Scripts.Items;
using SweetSugar.Scripts.Level;
using SweetSugar.Scripts.MapScripts;
using SweetSugar.Scripts.System;
using SweetSugar.Scripts.System.Combiner;
using SweetSugar.Scripts.System.Orientation;
using SweetSugar.Scripts.TargetScripts.TargetSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SweetSugar.Scripts.Core
{
//game state enum
    public enum GameState
    {
        Map,
        PrepareGame,
        RegenLevel,
        Tutorial,
        Pause,
        Playing,
        PreFailed,
        GameOver,
        ChangeSubLevel,
        PreWinAnimations,
        Win,
        WaitForPopup,
        WaitAfterClose,
        BlockedGame,
        BombFailed
    }

    /// <summary>
    /// core-game class, using for handle game states, blocking, sync animations and search mathing and map
    /// </summary>
    public class MLevelManager : MonoBehaviour
    {
        public static MLevelManager THIS;
        //life shop reference
        public LifeShop lifeShop;
        //true if Unity in-apps is enable and imported
        public bool enableInApps;
        //square width for border placement
        public float squareWidth = 1.2f;
        //item which was dragged recently 
        [FormerlySerializedAs("lastDraggedItem")] public MItem lastDraggedMItem;
        //item which was switched succesfully recently 
        [FormerlySerializedAs("lastSwitchedItem")] public MItem lastSwitchedMItem;
        //makes scores visible in the game
        public GameObject popupScore;
        //current game level
        public int currentLevel = 1;
        //current sub-level
        private int currentSubLevel = 1;
        public int CurrentSubLevel
        {
            get { return currentSubLevel; }
            set
            {
                currentSubLevel = value;
                mLevelData.currentSublevelIndex = currentSubLevel - 1;
            }
        }
        //current field reference
        public FieldBoard field => fieldBoards[CurrentSubLevel - 1];
        //EDITOR: cost of continue after failing 
        public int FailedCost;
        //EDITOR: moves gived to continue
        public int ExtraFailedMoves = 5;
        //EDITOR: time gived to continue
        public int ExtraFailedSecs = 30;
        //in-app products for purchasing
        public List<GemProduct> gemsProducts = new List<GemProduct>();
        //in-apps product IDs
        public string[] InAppIDs;
        //true if thriving block destroyed on current move
        public bool thrivingBlockDestroyed;
        //variables for boost which place bonus candies on the field
        public int BoostColorfullBomb;
        public int BoostPackage;
        public int BoostStriped;
        public int BoostMarmalade;
        //TODO: check sharing path
        public string androidSharingPath;
        public string iosSharingPath;
        //put some marmalade bears to the field on start
        public bool enableMarmalade => mLevelData.enableMarmalade;
        //empty boost reference for system
        public BoostIcon emptyBoostIcon;
        //debug settings reference
        public DebugSettings DebugSettings;
        //activate boost in game
        public BoostIcon activatedBoost;
        public BoostIcon ActivatedBoost
        {
            get { return activatedBoost == null ? emptyBoostIcon : activatedBoost; }
            set
            {
                if (value == null)
                {
                    if (activatedBoost != null && gameStatus == GameState.Playing)
                    {
                        InitScript.Instance.SpendBoost(activatedBoost.type);
                        UnLockBoosts();
                    }
                }

                //        if (activatedBoost != null) return;
                activatedBoost = value;

                if (value != null)
                {
                    LockBoosts();
                }

                if (activatedBoost == null) return;
                if (activatedBoost.type != BoostType.ExtraMoves && activatedBoost.type != BoostType.ExtraTime) return;
                if (THIS.mLevelData.limitType == LIMIT.MOVES)
                    THIS.mLevelData.limit += 5;
                else
                    THIS.mLevelData.limit += 30;

                ActivatedBoost = null;
            }
        }
        //score gain on this game
        public static int Score;
        //stars gain on this game
        public int stars;
        //striped effect reference
        public GameObject stripesEffect;
        //show popup score on field
        public bool showPopupScores;
        //popup score color
        public Color[] scoresColors;
        //popup score outline
        public Color[] scoresColorsOutline;
        //congratulation words popup reference
        public GameObject[] gratzWords;
        //Level gameobject reference
        public GameObject Level;
        //Gameobject reference
        public GameObject LevelsMap;
        //Gameobject reference
        public GameObject FieldsParent;
        //Gameobject reference
        public GameObject NoMoreMatches;
        //Gameobject reference
        public GameObject CompleteWord;
        //Gameobject reference
        public GameObject FailedWord;
        //in game boost reference
        public BoostIcon[] InGameBoosts;
        //levels passed for the current session
        //reference to orientation handler
        public OrientationGameCameraHandle orientationGameCameraHandle;
        [HideInInspector]public List<AnimateItems> animateItems = new List<AnimateItems>();
        //blocking to drag items for a time
        private bool dragBlocked;
        public bool DragBlocked
        {
            get { return dragBlocked; }
            set
            {
                dragBlocked = value;
                var moveID = THIS.moveID;
                StartCoroutine( ResetBlock(moveID));
            }
        }

        private IEnumerator ResetBlock(int moveID)
        {
            yield return new WaitForSeconds(3);
            if (moveID == THIS.moveID && dragBlocked)
                dragBlocked = false;
        }

        //current move ID
        public int moveID;
        //value for regeneration, items with falling or not
        public bool onlyFalling;
        //level loaded, wait until true for some courotines
        public bool levelLoaded;
        //true if Facebook plugin installed
        public bool FacebookEnable;
        //combine manager listener
        public CombineManager combineManager;
        //true if search of matches has started
        public bool findMatchesStarted;
        //true if need to check matches again
        private bool checkMatchesAgain;
        //if true - start the level avoind the map for debug
        public bool testByPlay;

        //game events
        #region EVENTS

        public delegate void GameStateEvents();
        public static event GameStateEvents OnMapState;
        public static event GameStateEvents OnEnterGame;
        public static event GameStateEvents OnLevelLoaded;
        public static event GameStateEvents OnWaitForTutorial;
        public static event GameStateEvents OnMenuPlay;
        public static event GameStateEvents OnSublevelChanged;
        public static event GameStateEvents OnMenuComplete;
        public static event GameStateEvents OnStartPlay;
        public static event GameStateEvents OnWin;
        public static event GameStateEvents OnLose;
        public static event GameStateEvents OnTurnEnd;
        public static event GameStateEvents OnCombo;

        //current game state
        private GameState GameStatus;
        public GameState gameStatus
        {
            get { return GameStatus; }
            set
            {
                GameStatus = value;
                AdsManager.THIS.CheckAdsEvents(value);
                switch (value)
                {
                    case GameState.PrepareGame://preparing and initializing  the game
                        StartCoroutine(AI.THIS.CheckPossibleCombines());
                        CrosssceneData.passLevelCounter++;
                        PrepareGame();
                        // var firstItemPrefab = THIS.levelData.target.prefabs.FirstOrDefault();
                        // if (firstItemPrefab && firstItemPrefab.GetComponent<Item>() && !firstItemPrefab.GetComponent<ItemSimple>())
                        //     collectIngredients = true;
                        GenerateLevel();
                        levelLoaded = true;
                        OnLevelLoaded?.Invoke();
                        break;
                    case GameState.WaitForPopup://waiting for pre game banners
                        StopCoroutine(IdleItemsDirection());
                        StartCoroutine(IdleItemsDirection());
                        OrientationGameCameraHandle.CameraParameters cameraParameters = orientationGameCameraHandle.GetCameraParameters();
                        Vector2 cameraCenter = orientationGameCameraHandle.GetCenterOffset();
                        StartCoroutine(AnimateField(field.GetPosition() + cameraCenter, cameraParameters.size));

                        break;
                    case GameState.Tutorial://tutorial state
                        OnWaitForTutorial?.Invoke();
                        break;
                    case GameState.PreFailed://chance to continue the game, shows menu PreFailed
                        AdsManager.THIS.CacheRewarded();
                        LeanTween.delayedCall(1, ()=>FailedWord.SetActive(true));
                        LeanTween.delayedCall(3, () =>
                        {
                            var preFailedGameObject = MenuReference.THIS.PreFailed.gameObject;
                            preFailedGameObject.GetComponent<PreFailed>().SetFailed();
                            preFailedGameObject.SetActive(true);
                        });
                        break;
                    case GameState.BombFailed:
                        LeanTween.delayedCall(0.3f, () =>
                        {
                            var preFailedGameObject = MenuReference.THIS.PreFailed.gameObject;
                            preFailedGameObject.GetComponent<PreFailed>().SetBombFailed();
                            preFailedGameObject.SetActive(true);
                        });
                        break;
                    case GameState.Map://map stat
                        break;
                    case GameState.Playing://playing state
                        StartCoroutine(AI.THIS.CheckPossibleCombines());
                        break;
                    case GameState.GameOver://game over
                        MenuReference.THIS.MenuFailed.gameObject.SetActive(true);
                        OnLose?.Invoke();
                        break;
                    case GameState.PreWinAnimations://animations after win
                        StartCoroutine(PreWinAnimationsCor());
                        break;
                    case GameState.ChangeSubLevel://changing sub level state
                        if (CurrentSubLevel != GetLastSubLevel())
                            ChangeSubLevel();
                        break;
                    case GameState.Win://shows MenuComplete
                        OnMenuComplete?.Invoke();
                        MenuReference.THIS.MenuComplete.gameObject.SetActive(true);
                        SoundBase.Instance.PlayOneShot(SoundBase.Instance.complete[1]);
                        if(winRewardAmount > 0)
                            InitScript.Instance.ShowGemsReward(winRewardAmount);// InitScript.Instance.ShowGemsReward(10);
                        OnWin();
                        break;
                }
            }
        }

        //Combine manager reference
        public CombineManager CombineManager
        {
            get
            {
                if(combineManager == null)         combineManager = new CombineManager();
                return combineManager;
            }
        }

        //if true - pausing all falling animations
        public bool StopFall => _stopFall.Any();
        //returns last sub-level of this level
        private int GetLastSubLevel()
        {
            return fieldBoards.Count;
        }
        //returns current game state
        public static GameState GetGameStatus()
        {
            return THIS.gameStatus;
        }
        //menu play enabled invokes event
        public void MenuPlayEvent()
        {
            OnMenuPlay?.Invoke();
        }
        //Switch sub level to next 
        private void ChangeSubLevel()
        {
            CurrentSubLevel++;
            OrientationGameCameraHandle.CameraParameters cameraParameters = orientationGameCameraHandle.GetCameraParameters();
            Vector2 cameraCenter = orientationGameCameraHandle.GetCenterOffset();
            StartCoroutine(AnimateField(field.GetPosition() + cameraCenter, cameraParameters.size));
        }

        #endregion
        //Lock boosts
        private void LockBoosts()
        {
            foreach (var item in InGameBoosts)
            {
                if (item.type != ActivatedBoost.type)
                    item.LockBoost();
            }
        }
        //unlock boosts
        public void UnLockBoosts()
        {
            foreach (var item in InGameBoosts)
            {
                item.UnLockBoost();
            }
        }
        //Load the level from "OpenLevel" player pref
        public void LoadLevel()
        {
            currentLevel = PlayerPrefs.GetInt("OpenLevel");
            if (currentLevel == 0)
                currentLevel = 1;
            LoadLevel(currentLevel);
        }
        //enable map
        public void EnableMap(bool enable)
        {
            GetComponent<Camera>().orthographicSize = enable ? 5.3f : 4;
    
            if (enable)
            {
                // Map enabling logic - skip if no map in scene
                if (LevelsMap != null)
                {
                    GetComponent<Camera>().GetComponent<MapCamera>()
                        .SetPosition(new Vector2(0, GetComponent<Camera>().transform.position.y));
                    if (FindObjectOfType<RestartLevel>() == null && DebugSettings.AI && DebugSettings.testLevel > 0)
                    {
                        PlayerPrefs.SetInt("OpenLevel", DebugSettings.testLevel);
                        RestartLevel();
                    }
                }
            }
            else
            {
                // Game enabling logic
                if(ServerTime.THIS != null && ServerTime.THIS.dateReceived)
                    InitScript.DateOfExit = ServerTime.THIS.serverTime.ToString();
                MenuReference.THIS.GetComponent<GraphicRaycaster>().enabled = false;
                MenuReference.THIS.GetComponent<GraphicRaycaster>().enabled = true;
                Level.transform.Find("Canvas").GetComponent<GraphicRaycaster>().enabled = false;
                Level.transform.Find("Canvas").GetComponent<GraphicRaycaster>().enabled = true;
            }

            // Only manipulate map camera and objects if they exist
            var mapCamera = Camera.main.GetComponent<MapCamera>();
            if (mapCamera != null)
                mapCamera.enabled = enable;
    
            if (LevelsMap != null)
            {
                LevelsMap.SetActive(!enable);
                LevelsMap.SetActive(enable);
            }
    
            Level.SetActive(!enable);

            if (!enable)
                Camera.main.transform.position = new Vector3(0, 0, -10) - (Vector3)orientationGameCameraHandle.offsetFieldPosition;
    
            foreach (var item in fieldBoards)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }
        }

        private void Awake()
        {
            THIS = this;
            testByPlay = false;
    
            // Disable map-related objects
            if (LevelsMap != null)
                LevelsMap.SetActive(false);
        }

        // Use this for initialization
        private void Start()
        {
            DebugSettings = Resources.Load<DebugSettings>("Scriptable/DebugSettings");
            LeanTween.init(800);
            LeanTween.reset();

            // Start directly in PrepareGame state instead of Map
            this.gameStatus = GameState.PrepareGame;
    
            winRewardAmount = Resources.Load<WinReward>("Scriptable/WinReward").winRewardAmount;
        }

        private void PrepareGame()
        {
            ActivatedBoost = null;
            Score = 0;
            stars = 0;
            moveID = 0;
            fieldBoards = new List<FieldBoard>();
            transform.position += Vector3.down * 1000;
            // targetUIObject.SetActive(false);

            EnableMap(false);

            LoadLevel();

            CurrentSubLevel = 1;
            if (ProgressBarScript.Instance != null)
                ProgressBarScript.Instance.InitBar();

            if (mLevelData.limitType == LIMIT.MOVES)
            {
                InGameBoosts.Where(i => i.type == BoostType.ExtraMoves).ToList().ForEach(i => i.gameObject.SetActive(true));
                InGameBoosts.Where(i => i.type == BoostType.ExtraTime).ToList().ForEach(i => i.gameObject.SetActive(false));
            }
            else
            {
                InGameBoosts.Where(i => i.type == BoostType.ExtraMoves).ToList().ForEach(i => i.gameObject.SetActive(false));
                InGameBoosts.Where(i => i.type == BoostType.ExtraTime).ToList().ForEach(i => i.gameObject.SetActive(true));
            }

            OnEnterGame?.Invoke();
        }

        public List<FieldBoard> fieldBoards = new List<FieldBoard>();
        public GameObject FieldBoardPrefab;
        [FormerlySerializedAs("levelData")] public MLevelData mLevelData;
        internal bool tutorialTime;
    
        //Generate loaded level
        private void GenerateLevel()
        {
            var fieldPos = new Vector3(-0.9f, 0, -10);
            var latestFieldPos = Vector3.right * ((GetLastSubLevel() - 1) * 10) + Vector3.back * 10;

            var i = 0;
            foreach (var item in fieldBoards)
            {
                var _field = item.gameObject;
                _field.transform.SetParent(FieldsParent.transform);
                _field.transform.position = fieldPos + Vector3.right * (i * 15);
                var fboard = _field.GetComponent<FieldBoard>();

                fboard.CreateField();
                latestFieldPos = fboard.GetPosition();

                i++;
            }
            mLevelData.TargetCounters.RemoveAll(x => x.targetLevel.setCount == SetCount.FromLevel && x.GetCount()==0);

            transform.position = latestFieldPos + Vector3.right * 10 + Vector3.back * 10 - (Vector3)orientationGameCameraHandle.offsetFieldPosition;
            SetPreBoosts();
        }

        private bool animStarted;
        /// <summary>
        /// move camera to the field
        /// </summary>
        /// <param name="destPos">position of the field</param>
        /// <param name="cameraParametersSize">camera size</param>
        /// <returns></returns>
        private IEnumerator AnimateField(Vector3 destPos, float cameraParametersSize)
        {
            var _camera = GetComponent<Camera>();
            if(animStarted) yield break;
            animStarted = true;
            var duration = 2f;
            var speed = 10f;
            var startPos = transform.position;
            var distance = Vector2.Distance(startPos, destPos);
            var time = distance / speed;
            var curveX = new AnimationCurve(new Keyframe(0, startPos.x), new Keyframe(time, destPos.x));
            var startTime = Time.time;
            float distCovered = 0;
            while (distCovered < distance)
            {
                distCovered = (Time.time - startTime) * speed;
                transform.localPosition = new Vector3(curveX.Evaluate(Time.time - startTime), transform.position.y, 0);
                _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, cameraParametersSize, Time.deltaTime*5);
                yield return new WaitForFixedUpdate();
            }

            _camera.orthographicSize = cameraParametersSize;
            transform.position = destPos;
            yield return new WaitForSeconds(0.5f);
            animStarted = false;
            GameStart();
        }

        //game start
        private void GameStart()
        {
            if (CurrentSubLevel - 1 == 0)
                MenuReference.THIS.PrePlay.gameObject.SetActive(true);
            else
            {
                OnSublevelChanged?.Invoke();
                gameStatus = GameState.Playing;
            }
        }
        /// Cloud effect animation for different direction levels
        private IEnumerator IdleItemsDirection()
        {
            if (field.squaresArray.Select(i => i.direction).Distinct().Count() > 1)
            {
                while (true)
                {
                    yield return new WaitForSeconds(3);
                    if (gameStatus == GameState.Playing && !findMatchesStarted)
                    {

                        // var orderedEnumerableCol = DirectionCloudEffect.GetItems();
                        var orderedEnumerableCol = THIS.field.GetItems()
                            .GroupBy(i => i.square.squaresGroup).ToList()
                            .Select(x => new
                            {
                                items = x,
                                Num = x.Max(i => i.square.orderInSequence),
                                Count = x.Count(),
                                x.Key
                            }).OrderByDescending(i => i.Num).ToList();

                        // Debug.WatchInstance(orderedEnumerableCol);
                        foreach (var items in orderedEnumerableCol)
                        {
                            var animationFinished = false;
                            foreach (var item in items.items)
                            {
                                if(item.destroying) continue;
                                StartCoroutine(item.DirectionAnimation(() => { animationFinished = true; }));
                            }

                            yield return new WaitUntil(() => animationFinished);
                        }
                    }

                    yield return new WaitForSeconds(1);
                }
            }
        }

        //Check win or lose conditions
        public void CheckWinLose()
        {
            var lose = false;
            var win = false;

            if (mLevelData.limit <= 0)
            {
                mLevelData.limit = 0;

                if (!mLevelData.IsTotalTargetReached())
                    lose = true;
                else win = true;
            }

            else
            {
                if (mLevelData.IsTotalTargetReached() && !mLevelData.WaitForMoveOut())
                {
                    win = true;
                }
                else if (mLevelData.IsTargetReachedSublevel() && fieldBoards.Count>1)
                    gameStatus = GameState.ChangeSubLevel;
            }

            if (lose)
            {
                gameStatus = GameState.PreFailed;
            }

            else if (win && !lose)
                gameStatus = GameState.PreWinAnimations;
            else if (!win && !lose && FindObjectsOfType<MItemTimeBomb>().Any(i => i.timer <= 0))
            {
                MLevelManager.THIS.gameStatus = GameState.BombFailed;
            }

            if (DebugSettings.AI && (win || lose))
            {
                Debug.Log((win ? "win " : "lose ") + " score " + Score+ " stars " + stars + " moves/time rest " + THIS.mLevelData.limit);
                RestartLevel();
            }
        }

        public GameObject winTrail;
        public Transform movesTransform;
        public int destLoopIterations;
        //Animations after win
          private IEnumerator PreWinAnimationsCor()
          { 
              tapToSkip = Instantiate((GameObject) Resources.Load("Prefabs/TapToSkip"),OrientationActivator.ActiveOrientationCanvas.transform);
            if (!InitScript.Instance.losingLifeEveryGame && InitScript.lifes < InitScript.Instance.CapOfLife)
                InitScript.Instance.AddLife(1);
            CompleteWord.SetActive(true);
            var limit = Mathf.Clamp(mLevelData.limit, 0, 5);

            if(!skipWin)
            {
                var c1 = StartCoroutine(PreWinLoop(limit));
                yield return c1;
            }
            if(skipWin)
            {
                Score += limit * Random.Range(500, 3000) / mLevelData.colorLimit;
                CheckStars();
                skipWin = false;
                Destroy(tapToSkip);
            }
            if (PlayerPrefs.GetInt($"Level.{currentLevel:000}.StarsCount", 0) < stars)
                PlayerPrefs.SetInt($"Level.{currentLevel:000}.StarsCount", stars);
            if (Score > PlayerPrefs.GetInt("Score" + currentLevel))
            {
                PlayerPrefs.SetInt("Score" + currentLevel, Score);
            }
            if(PlayerPrefs.GetInt("ReachedLevel") <= currentLevel)
                PlayerPrefs.SetInt("ReachedLevel",currentLevel+1);
            PlayerPrefs.Save();
            if(Application.isEditor)
                Debug.Log("Level " + currentLevel + " score " + Score + " stars " + stars);
            CrosssceneData.win = true;
#if PLAYFAB || GAMESPARKS
            NetworkManager.dataManager.SetPlayerScore(currentLevel, Score);
            NetworkManager.dataManager.SetPlayerLevel(currentLevel + 1);
            NetworkManager.dataManager.SetStars();
#endif


            gameStatus = GameState.Win;
        }

        private IEnumerator PreWinLoop(int limit)
        {
            var items = field.GetRandomItems(mLevelData.limitType == LIMIT.MOVES ? limit : 5);
            var list = new List<object>();
            for (var i = 0; i < items.Count; i++)
            {
                if (items?[i] == null) continue;
                var go = Instantiate(winTrail);
                go.transform.position = movesTransform.position;
                go.GetComponent<TrailEffect>().target = items[i];
                list.Add(go);
                go.GetComponent<TrailEffect>().StartAnim(target =>
                {
                    if (mLevelData.limitType == LIMIT.MOVES)
                    {
                        mLevelData.limit--;
                    }

                    if (target != null && target.gameObject.activeSelf)
                    {
                        target.NextType = (ItemsTypes) Random.Range(4, 6);
                        target.ChangeType();
                    }
                });
                if(skipWin) yield break;
                yield return new WaitForSeconds(0.3f);
            }


            if (list.Count > 0)
            {
                if(skipWin) yield break;
                yield return new WaitForListNull(list);
            }
            mLevelData.limit = 0;

            do
            {
                while (field.GetAllExtaItems().Count > 0 && gameStatus != GameState.Win)
                {
                    var item = field.GetAllExtaItems()[0];
                    if (item != null && item.currentType != ItemsTypes.MULTICOLOR) item.DestroyItem();
                    else if (item.currentType == ItemsTypes.MULTICOLOR) item.Check(item, field.GetRandomItems(1).First());
                    dragBlocked = true;
                    if(skipWin) yield break;
                    yield return new WaitForSeconds(0.1f);
                    FindMatches();
                }
                if(skipWin) yield break;

                yield return new WaitWhile(() => findMatchesStarted);
            } while (field.GetAllExtaItems().Count > 0);

            FindMatches();
            while (dragBlocked)
                yield return new WaitForFixedUpdate();

            if(skipWin) yield break;
            yield return new WaitForSeconds(1f);

            MenuReference.THIS.PreCompleteBanner.gameObject.SetActive(true);
            yield return new WaitForSeconds(3);
            MenuReference.THIS.PreCompleteBanner.gameObject.SetActive(false);
        }

        private void Update()
        {
            //  AvctivatedBoostView = ActivatedBoost;
            if (Input.GetKeyDown(DebugSettings.Regen) && DebugSettings.enableHotkeys)
            {
                NoMatches();
            }

            if (Input.GetKeyDown(DebugSettings.Win) && DebugSettings.enableHotkeys)
            {
                stars = Mathf.Clamp(stars,1, stars);
                gameStatus = GameState.PreWinAnimations;
            }

            if (Input.GetKeyDown(DebugSettings.Lose) && DebugSettings.enableHotkeys)
            {
                mLevelData.limit = 1;
            }

            if (Input.GetKeyDown(DebugSettings.Restart) && DebugSettings.enableHotkeys)
            {
                InitScript.Instance.AddLife(1);
                RestartLevel();
            }

// #if UNITY_EDITOR
            if (gameStatus == GameState.Playing)
                Time.timeScale = DebugSettings.TimeScaleItems;
            else
                Time.timeScale = DebugSettings.TimeScaleUI;
// #endif

            //if (Input.GetKeyDown(KeyCode.D))
            //{
            //	DestroyAnimatedItems();
            //}
            if (Input.GetKeyDown(DebugSettings.SubSwitch) && DebugSettings.enableHotkeys)
            {
                gameStatus = GameState.ChangeSubLevel;
            }

            if (Input.GetKeyUp(DebugSettings.Back) && DebugSettings.enableHotkeys)
            {
                if (THIS.gameStatus == GameState.Playing)
                    GameObject.Find("CanvasGlobal").transform.Find("MenuPause").gameObject.SetActive(true);
                else if (THIS.gameStatus == GameState.Map)
                    Application.Quit();
            }
            
            if (gameStatus == GameState.PreWinAnimations)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    skipWin = true;
                }
            }

            if (gameStatus == GameState.Playing || gameStatus == GameState.Tutorial)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    var hit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition),
                        1 << LayerMask.NameToLayer("Item"));
                    if (hit != null)
                    {
                        _lastTouchedMItem = hit.gameObject.GetComponent<MItem>();
                        if (tutorialTime && !_lastTouchedMItem.tutorialUsableItem) return;
                        if (!THIS.DragBlocked &&
                            (gameStatus == GameState.Playing || gameStatus == GameState.Tutorial))
                        {
                            OnStartPlay?.Invoke();
                            tutorialTime = false;
                            if (THIS.ActivatedBoost.type == BoostType.ExplodeArea &&
                                _lastTouchedMItem.currentType != ItemsTypes.MULTICOLOR && _lastTouchedMItem.currentType != ItemsTypes.INGREDIENT)
                            {
                                SoundBase.Instance.PlayOneShot(SoundBase.Instance.boostBomb);
                                THIS.DragBlocked = true;
                                var obj = Instantiate(Resources.Load("Boosts/area_explosion"), _lastTouchedMItem.transform.position,
                                    _lastTouchedMItem.transform.rotation) as GameObject;
                                obj.GetComponent<SpriteRenderer>().sortingOrder = 4;
                                obj.GetComponent<BoostAnimation>().square = _lastTouchedMItem.square;
                                THIS.ActivatedBoost = null;
                            }
                            else if (THIS.ActivatedBoost.type == BoostType.Bomb &&
                                     _lastTouchedMItem.currentType != ItemsTypes.MULTICOLOR && _lastTouchedMItem.currentType != ItemsTypes.INGREDIENT)
                            {
                                SoundBase.Instance
                                    .PlayOneShot(SoundBase.Instance.boostColorReplace);
                                THIS.DragBlocked = true;
                                var obj = Instantiate(Resources.Load("Boosts/simple_explosion"), _lastTouchedMItem.transform.position,
                                    _lastTouchedMItem.transform.rotation) as GameObject;
                                obj.GetComponent<BoostAnimation>().square = _lastTouchedMItem.square;
                                obj.GetComponent<SpriteRenderer>().sortingOrder = 4;
                                THIS.ActivatedBoost = null;
                            }
                            else if (_lastTouchedMItem.square.GetSubSquare().CanGoOut()/* && lastTouchedItem.Combinable*/)
                            {
                                _lastTouchedMItem.dragThis = true;
                                _lastTouchedMItem.mousePos = _lastTouchedMItem.GetMousePosition();
                                _lastTouchedMItem.deltaPos = Vector3.zero;
                            }
                        }
                    }
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    if (_lastTouchedMItem != null)
                    {
                        _lastTouchedMItem.dragThis = false;
                        _lastTouchedMItem.switchDirection = Vector3.zero;
                    }
                }
                else if (Input.GetMouseButton(1))
                {
                    var hit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition),
                        1 << LayerMask.NameToLayer("Item"));
                    if (hit != null)
                    {
                        var item = hit.gameObject.GetComponent<MItem>();
                        Camera.main.GetComponent<ItemChangerTest>().ShowMenuItems(item);

                    }
                }
            }
        }

        private void RestartLevel()
        {
            GameObject gm = new GameObject();
            gm.AddComponent<RestartLevel>();
            string scene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(scene);
        }

        #region RegenerateLevel
        //No matches detected, regenerate level
        public void NoMatches()
        {
            if (field.fieldData.noRegenLevel)
            {
                return;
            }
            StartCoroutine(NoMatchesCor());
        }

        private IEnumerator NoMatchesCor()
        {
            if (gameStatus == GameState.Playing)
            {
                DragBlocked = true;

                SoundBase.Instance.PlayOneShot(SoundBase.Instance.noMatch);

                NoMoreMatches.gameObject.SetActive(true);
                gameStatus = GameState.RegenLevel;
                yield return new WaitForSeconds(1);
                ReGenLevel();
            }
        }

        public void ReGenLevel()
        {
            DragBlocked = true;
            //if (gameStatus != GameState.Playing && gameStatus != GameState.RegenLevel)
            //DestroyAnimatedItems();
//        if (gameStatus == GameState.RegenLevel)
//            DestroyItems(true);
            StartCoroutine(RegenMatches());
            OnLevelLoaded();
        }

        private IEnumerator RegenMatches(bool onlyFalling = false)
        {
            if (gameStatus == GameState.RegenLevel)
            {
                yield return new WaitForSeconds(0.5f);
            }
            if (!onlyFalling)
                field.RegenItems(false);
            else
                THIS.onlyFalling = true;
            field.GetItems().ForEach(i=>i.Hide(true));
            yield return new WaitForFixedUpdate();

            var combs = new List<List<MItem>>();
            do
            {
                yield return new WaitWhileFall();
                combs = CombineManager.GetCombinedItems(field);
                combs.Add( AI.THIS.GetMarmaladeCombines()?.items);
                combs = combs.WhereNotNull().Where(i => i.Count() > 0).ToList();
                yield return new WaitForEndOfFrame();
                foreach (var comb in combs)
                {
                    foreach (var item in comb)
                    {
                        item.GenColor();
                    }
                }
            } while (combs.Count > 0);

            SetPreBoosts();
            if (!onlyFalling)
                DragBlocked = false;
            THIS.onlyFalling = false;
            if (gameStatus == GameState.RegenLevel)
                gameStatus = GameState.Playing;
            yield return new WaitForEndOfFrame();
            field.GetItems().ForEach(i=>
            {
                var transformLocalScale = i.transform.localScale;
                i.transform.localScale = Vector3.zero;
                LeanTween.scale(i.gameObject, transformLocalScale, .3f); i.Hide(false);  });
        }
    
        public void DestroyItems(bool withoutEffects = false)
        {
            var items = field.GetItems();
            foreach (var item in items)
            {
                if (item != null)
                {
                    if (item.GetComponent<MItem>().currentType != ItemsTypes.INGREDIENT &&
                        item.GetComponent<MItem>().currentType == ItemsTypes.NONE)
                    {
                        if (!withoutEffects)
                            item.GetComponent<MItem>().DestroyItem();
                        else
                            item.GetComponent<MItem>().anim.SetTrigger("disappear");
                    }
                }
            }
        }
    
        #endregion


        /// <summary>
        /// Place boosts in the field which bought before in Play menu
        /// </summary>
        private void SetPreBoosts()
        {
            if (BoostPackage > 0)
            {
                InitScript.Instance.SpendBoost(BoostType.Packages);
                foreach (var item in field.GetRandomItems(BoostPackage))
                {
                    item.NextType = ItemsTypes.PACKAGE;
                    item.ChangeType(null, true, false);
                }

                BoostPackage = 0;
            }

            if (BoostColorfullBomb > 0)
            {
                InitScript.Instance.SpendBoost(BoostType.MulticolorCandy);
                foreach (var item in field.GetRandomItems(BoostColorfullBomb))
                {
                    item.NextType = ItemsTypes.MULTICOLOR;
                    item.ChangeType(null, true, false);
                }

                BoostColorfullBomb = 0;
            }

            if (BoostStriped > 0)
            {
                InitScript.Instance.SpendBoost(BoostType.Stripes);
                foreach (var item in field.GetRandomItems(BoostStriped))
                {
                    item.NextType = (ItemsTypes)Random.Range(4, 6);
                    item.ChangeType(null, true, false);
                }

                BoostStriped = 0;
            }

            if (BoostMarmalade > 0)
            {
                InitScript.Instance.SpendBoost(BoostType.Marmalade);
                foreach (var item in field.GetRandomItems(BoostMarmalade))
                {
                    item.NextType = ItemsTypes.MARMALADE;
                    item.ChangeType(null, true, false);
                }

                BoostStriped = 0;
            }
        }


        //Find matches with delay
        public IEnumerator FindMatchDelay()
        {
            yield return new WaitForSeconds(0.2f);
            THIS.FindMatches();
        }

        //start sync matches search then wait while items destroy and fall
        public void FindMatches()
        {
            if (!findMatchesStarted)
                StartCoroutine(FallingDown());
            else
                checkMatchesAgain = true;
        }

        internal List<MGameBlocker> _stopFall = new List<MGameBlocker>();
        public int combo;
        public AnimationCurve fallingCurve = AnimationCurve.Linear(0, 0, 1, 0);
        public float waitAfterFall = 0.02f;
        [HideInInspector] private bool collectIngredients;

        private MItem _lastTouchedMItem;
        private int winRewardAmount;
        public bool skipWin;
        private GameObject tapToSkip;

        private IEnumerator FallingDown()
        {
            findMatchesStarted = true;
//        Debug.Log("@@@ Next Move search matches @@@");
            THIS.thrivingBlockDestroyed = false;
            combo = 0;
            AI.THIS.allowShowTip = false;
            var it = field.GetItems();
            for (var i = 0; i < it.Count; i++)
            {
                var item = it[i];
                if (item != null)
                {
                    item.anim.StopPlayback();
                }
            }

            destLoopIterations = 0;
            while (true)
            {
                destLoopIterations++;
                checkMatchesAgain = false;

                var destroyItemsListed = field.GetItems().Where(i => i.destroyNext).ToList();
                if (destroyItemsListed.Count > 0)
                    yield return new WaitWhileDestroyPipeline(destroyItemsListed, new Delays());
                yield return new WaitWhileDestroying();
                yield return new WaitWhile(()=>StopFall);
                yield return new WaitWhileFall();
                yield return new WaitWhileCollect();
//            yield return new WaitWhileFallSide();
                var combineCount = CombineManager.GetCombines(field);
                if ((combineCount.Count <= 0 || !combineCount.SelectMany(i => i.items).Any()) && !field.DestroyingItemsExist() && !field.GetEmptySquares().Any() &&
                    !checkMatchesAgain)
                {
                    break;
                }

                if(destLoopIterations > 1)
                {
                    var combines = combineCount.Where(i=>i.items.All(x=>!x.dontDestroyForThisCombine)).ToList();
                    foreach (var combine in combines)
                    {
                        if(combine.items.Any())
                            combine.items.NextRandom().NextType = combine.nextType;
                    }

                    combines.SelectMany(i => i.items).ToList().ForEach(i => i.destroyNext = true);
                }
                if (!MLevelManager.THIS.animateItems.Any())
                    mLevelData.TargetCounters.ForEachY(i => i.GetCount());
            }

            if (combo > 2 && gameStatus == GameState.Playing)
            {
                gratzWords[Random.Range(0, gratzWords.Length)].SetActive(true);
                combo = 0;
                OnCombo?.Invoke();
            }

            //CheckItemsPositions();
            DragBlocked = false;
            findMatchesStarted = false;
            checkMatchesAgain = false;
            if (gameStatus == GameState.Playing)
                StartCoroutine(AI.THIS.CheckPossibleCombines());

//        Debug.Log("<-next turn->");
            if (gameStatus == GameState.Playing)
            {
                if (!MLevelManager.THIS.animateItems.Any())
                {
                    OnTurnEnd?.Invoke();
                    THIS.CheckWinLose();
                }
                else
                {
                    yield return new WaitWhile(()=>MLevelManager.THIS.animateItems.Any());
                    OnTurnEnd?.Invoke();
                    THIS.CheckWinLose();
                }
            }
        }
        /// <summary>
        /// Get square by position
        /// </summary>
        public Square GetSquare(int col, int row, bool safe = false)
        {
            return field.GetSquare(col, row, safe);
        }
        /// <summary>
        /// Get bunch of squares by row number
        /// </summary>
        public List<Square> GetRowSquare(int row)
        {
            var itemsList = new List<Square>();
            for (var col = 0; col < mLevelData.maxCols; col++)
            {
                Square square = GetSquare(col, row, true);
                if (!square.IsNone())
                    itemsList.Add(square);
            }

            return itemsList;
        }
        /// Get bunch of squares by column number
        public List<Square> GetColumnSquare(int col)
        {
            var itemsList = new List<Square>();
            for (var row = 0; row < mLevelData.maxRows; row++)
            {
                Square square = GetSquare(col, row, true);
                if (!square.IsNone())
                    itemsList.Add(square);
            }

            return itemsList;
        }
        /// Get bunch of items by row number
        public List<MItem> GetRow(int row)
        {
            var itemsList = new List<MItem>();
            for (var col = 0; col < mLevelData.maxCols; col++)
            {
                itemsList.Add(GetSquare(col, row, true).MItem);
            }

            return itemsList.WhereNotNull().ToList();
        }
        /// Get bunch of items by column number
        public List<MItem> GetColumn(int col)
        {
            var itemsList = new List<MItem>();
            for (var row = 0; row < mLevelData.maxRows; row++)
            {
                itemsList.Add(GetSquare(col, row, true).MItem);
            }

            return itemsList.WhereNotNull().ToList();
        }
        /// <summary>
        /// Get squares around the square
        /// </summary>
        /// <param name="square"></param>
        /// <returns></returns>
        public List<Square> GetSquaresAroundSquare(Square square)
        {
            var col = square.col;
            var row = square.row;
            var itemsList = new List<Square>();
            for (var r = row - 1; r <= row + 1; r++)
            {
                for (var c = col - 1; c <= col + 1; c++)
                {
                    itemsList.Add(GetSquare(c, r, true));
                }
            }

            return itemsList;
        }

        /// <summary>
        /// get 9 items around the square
        /// </summary>
        /// <param name="square"></param>
        /// <returns></returns>
        public List<MItem> GetItemsAround9(Square square)
        {
            var itemsList = GetItemsAround8(square);
            itemsList.Add(square.MItem);
            return itemsList;
        }

        /// get 8 items around the square
        public List<MItem> GetItemsAround8(Square square)
        {
            var col = square.col;
            var row = square.row;
            var itemsList = new List<MItem>();
            for (var r = row - 1; r <= row + 1; r++)
            {
                for (var c = col - 1; c <= col + 1; c++)
                {
                    var pos = GetSquare(c, r, true).transform.position;
                    RaycastHit2D[] result = new RaycastHit2D[3];
                    Physics2D.LinecastNonAlloc(square.transform.position, pos, result,1<< LayerMask.NameToLayer("Item"));
                    var hit = result.Where(i => i.transform).OrderBy(i => (i.transform.position - pos).magnitude).FirstOrDefault();
                    if(hit && hit.transform)
                        itemsList.Add(hit.transform.GetComponent<MItem>());
                }
            }

            return itemsList;
        }

        //striped effect
        public void StripedShow(GameObject obj, bool horrizontal)
        {
            var effect = Instantiate(stripesEffect, obj.transform.position, Quaternion.identity);
            if (!horrizontal)
                effect.transform.Rotate(Vector3.back, 90);
            Destroy(effect, 1);
        }

        //popup score, to use - enable "Popup score" in editor
        public void ShowPopupScore(int value, Vector3 pos, int color)
        {
            // UpdateBar();
            if (showPopupScores)
            {
                var parent = GameObject.Find("CanvasScore").transform;
                var poptxt = Instantiate(popupScore, pos, Quaternion.identity);
                poptxt.transform.GetComponentInChildren<Text>().text = "" + value;
                if (color <= scoresColors.Length - 1)
                {
                    poptxt.transform.GetComponentInChildren<Text>().color = scoresColors[color];
                    poptxt.transform.GetComponentInChildren<Outline>().effectColor = scoresColorsOutline[color];
                }

                poptxt.transform.SetParent(parent);
                //   poptxt.transform.position += Vector3.right * 1;
                poptxt.transform.localScale = Vector3.one / 1.5f;
                Destroy(poptxt, 0.3f);
            }
        }

        /// <summary>
        /// check gained stars
        /// </summary>
        public void CheckStars()
        {
            if (Score >= mLevelData.star1 && stars <= 0)
            {
                stars = 1;
            }

            if (Score >= mLevelData.star2 && stars <= 1)
            {
                stars = 2;
            }

            if (Score >= mLevelData.star3 && stars <= 2)
            {
                stars = 3;
            }
        }

        /// <summary>
        /// load level from 
        /// </summary>
        /// <param name="currentLevel"></param>
        public void LoadLevel(int currentLevel)
        {
            levelLoaded = false;
            mLevelData = MLoadingManager.LoadForPlay(currentLevel, mLevelData);

            if (gameStatus != GameState.Map)
            {
                foreach (var fieldData in mLevelData.fields)
                {
                    var _field = Instantiate(FieldBoardPrefab);
                    var fboard = _field.GetComponent<FieldBoard>();
                    fboard.fieldData = fieldData;
                    fboard.squaresArray = new Square[fieldData.maxCols * fieldData.maxRows];
                    fieldBoards.Add(fboard);
                }
            }
        }
    }
}