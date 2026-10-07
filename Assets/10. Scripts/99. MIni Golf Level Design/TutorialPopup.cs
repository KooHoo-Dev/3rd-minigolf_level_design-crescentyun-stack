using UnityEngine;
using UnityEngine.UI;

public class TutorialPopup : MonoBehaviour
{
    [Header("UI 연결 (필수)")]
    [Tooltip("팝업 전체를 감싸고 있는 Panel 오브젝트")]
    [SerializeField] private GameObject tutorialPanel;
    
    [Tooltip("확인 버튼")]
    [SerializeField] private Button confirmButton;

    [Header("스테이지별 설정")]
    [Tooltip("씬별 고유 이름 (예: StageList, Stage1, Stage2, Stage3, Stage4)")]
    [SerializeField] private string stageKey = "Stage1";

    [Header("옵션")]
    [Tooltip("팝업이 켜져있을 때 게임 시간을 멈출지 여부")]
    [SerializeField] private bool pauseGameOnStart = true;

    // 저장 및 조회용 Key
    private string PrefsKey => $"Tutorial_Seen_{stageKey}";

    private void Awake()
    {
        
        
        // 확인 버튼 이벤트 연결
        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(CloseTutorial);
        }
    }

    private void Start()
    {
        
        
        // 이미 본 적이 있는지 체크 (0: 안봄, 1: 이미 봄)
        if (PlayerPrefs.GetInt(PrefsKey, 0) == 0)
        {
            ShowTutorial();
        }
        else
        {
            // 이미 봤으면 시작하자마자 팝업 끄기
            if (tutorialPanel != null)
            {
                tutorialPanel.SetActive(false);
            }
        }
    }

    public void ShowTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);

            if (pauseGameOnStart)
            {
                Time.timeScale = 0f;
            }
        }
    }

    public void CloseTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);

            // 해당 스테이지 튜토리얼을 보았음을 저장
            PlayerPrefs.SetInt(PrefsKey, 1);
            PlayerPrefs.Save();

            if (pauseGameOnStart)
            {
                Time.timeScale = 1f;
            }
        }
    }

    // ★ 테스트용: 인스펙터 우클릭 시 해당 스테이지 튜토리얼 기록만 초기화
    [ContextMenu("Reset This Stage Record")]
    public void ResetThisStageRecord()
    {
        PlayerPrefs.DeleteKey(PrefsKey);
        Debug.Log($"[{stageKey}] 튜토리얼 기록이 삭제되었습니다.");
    }

    // ★ 테스트용: 모든 스테이지 튜토리얼 기록 전체 초기화
    [ContextMenu("Reset ALL Tutorial Records")]
    public void ResetAllRecords()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("모든 PlayerPrefs 기록이 초기화되었습니다.");
    }
}