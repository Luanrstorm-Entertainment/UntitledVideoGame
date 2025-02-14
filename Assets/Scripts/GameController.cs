using Assets.Scripts.SaveSystem;
using Assets.Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniStorm;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts
{
    [RequireComponent(typeof(TimeLoopEffectManager))]
    public class GameController : MonoBehaviour
    {
        public static GameController instance;
        [ReadOnly] public string LevelName;
        [ReadOnly] public string SaveDirectory;
        public BuildModeController BuildModeController;
        public WeatherType WeatherType;
        public GameObject TimerUi;
        public GameObject ClockUi;
        private Transform clockArm;
        private Image nightBG;
        private Image dayBG;
        private Image duskBG;
        public GameObject EndBoss;
        public GameObject WaveController;
        public float PreAttackTimer = 300;
        private bool AttackHasStarted = false;
        public bool SkipPreAttackTimer = false;
        public string time;


        public string skipInputName = "startAttack";

        private TimeLoopEffectManager TimeLoopEffectManager;
        private bool timeLoopStarted = false;

        private float armPos;
        private float armSpeed;

        private void Awake()
        {
            SaveDirectory = Application.persistentDataPath;
            TimeLoopEffectManager = GetComponent<TimeLoopEffectManager>();
            clockArm = ClockUi.transform.Find("ClockArm");
            nightBG = ClockUi.transform.Find("Night").GetComponent<Image>();
            dayBG = ClockUi.transform.Find("Day").GetComponent<Image>();
            duskBG = ClockUi.transform.Find("Dusk").GetComponent<Image>();
        }

        // Start is called before the first frame update
        void Start()
        {

            Time.timeScale = 1;
            if (!instance) instance = this;
            else Destroy(gameObject);

            LevelName = SceneManager.GetActiveScene().name;

            nightBG.fillAmount = 0.75f;
            dayBG.fillAmount = 0.125f;
            duskBG.fillAmount = dayBG.fillAmount + 0.125f;

            InitializeLevel();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateTimer();
            if (Input.GetButtonUp(skipInputName) && !AttackHasStarted)
            {
                UniStormManager.Instance.SetTime(19, 00);
                TimeLoopEffectManager.StartCapture();
            }

            if (UniStormSystem.Instance.Hour == 7 && UniStormSystem.Instance.Minute == 00)
            {
                WaveController.SetActive(false);
                EndBoss.gameObject.SetActive(true);
            }

        }

        private void UpdateTimer()
        {
            int hour = UniStormSystem.Instance?.Hour ?? 0;
            int minutes = UniStormSystem.Instance?.Minute ?? 0;

            TimerUi.GetComponent<TextMeshProUGUI>().text = $"{hour}:{minutes.ToString().PadLeft(2, '0')}";
            TimerUi.GetComponent<TextMeshProUGUI>().text = $"{hour}:{minutes.ToString().PadLeft(2, '0')}";

            // NIGHT LASTS FROM 19 PM TO 7 AM; THE BOSS FIGHT BEGINS AT 7 AM
            if (hour is >= 17 and < 19)
            {
                // 2 minute day
                // armPos += Time.deltaTime / 83f;
                // 5 minute day
                // armPos += Time.deltaTime / 207.5f;
                // 15 minute day
                armPos += Time.deltaTime / 616f;
                // 30 minute day
                // armPos += Time.deltaTime / 1170f;
            }
            else
            {
                // 1 minute night
                // armPos += Time.deltaTime / 78f;
                // 3 minute night
                // armPos += Time.deltaTime / 234f;
                // 15 minute night
                armPos += Time.deltaTime / 1245.0006f;
            }
            
            if (UniStormSystem.Instance.Hour == 19 && UniStormSystem.Instance.Minute == 00) armPos = 0.25f;

            float posNormalised = armPos % 1f;

            float rotationDegreesPerDay = 360f;

            clockArm.eulerAngles = new Vector3(0, 0, -posNormalised * rotationDegreesPerDay);

            if (hour == 19 && !AttackHasStarted)
            {
                BuildModeController.Disable();
                WaveController.GetComponent<WaveSpawner>().StartWaves();
                AttackHasStarted = true;
            }
        }


        private void InitializeLevel()
        {
            if (SaveManager.Instance.SaveExists(LevelName))
            {
                SaveManager.Instance.LoadSave(LevelName);
            }
            if (SkipPreAttackTimer) PreAttackTimer = 1;
            UniStorm.UniStormManager.Instance.ChangeWeatherInstantly(WeatherType);

        }

        [ContextMenu("Clearsave")]
        public void ClearSave()
        {
            SaveManager.Instance.ClearSave(SceneManager.GetActiveScene().name);
        }

        [ContextMenu("StartTimeloop")]
        public void StartTimeLoop()
        {
            if (!timeLoopStarted)
            {
                TimeLoopEffectManager.StopCapture();
                timeLoopStarted = true;

                SaveManager.Instance.SaveStateToFile(LevelName);
                var loadingTask = SceneManager.LoadSceneAsync(LevelName);
                loadingTask.allowSceneActivation = false;
                TimeLoopEffectManager.StartTimeloopEffect(() =>
                {
                    loadingTask.allowSceneActivation = true;

                });
            }
        }
        
        public void StartTimeLoop2()
        {
            if (!timeLoopStarted)
            {
                timeLoopStarted = true;
                SaveManager.Instance.SaveStateToFile(LevelName);
                SceneManager.LoadScene(LevelName);
            }
        }
        
        [ContextMenu("GenerateIdsForPersistable")]
        public void GenerateIdsForPersistable()
        {
            ClearSave();
            GameObject.FindObjectsOfType<PersistableMonoBehaviour>().ToList().ForEach(x => x.GenerateId());
        }

    }
}