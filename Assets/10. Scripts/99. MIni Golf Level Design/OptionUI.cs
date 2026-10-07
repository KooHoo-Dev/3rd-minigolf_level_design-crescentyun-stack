using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Study.Utilities;
using UnityEngine.InputSystem;


public class OptionUI : MonoBehaviour
{
    [Header("1. Main Panel Reference")]
    [Tooltip("ESC 누르면 열리는 옵션 전체 패널")]
    [SerializeField] private GameObject optionPanel;

    [Header("2. Option Buttons")]
    [SerializeField] private Button mainSceneButton;  // 1번: 00.Main씬 이동
    [SerializeField] private Button resumeButton;     // 2번: 게임 계속 플레이
    [SerializeField] private Button tutorialButton;   // 3번: 튜토리얼 모아보기

    [Header("3. Scene Settings")]
    [Tooltip("메인씬 이름")]
    [SerializeField] private string mainSceneName = "00.Main";

    [Header("4. Tutorial Viewer Settings")]
    [Tooltip("튜토리얼 모아보기 패널")]
    [SerializeField] private GameObject tutorialViewerPanel;
    [Tooltip("튜토리얼 이미지를 띄울 UI Image 컴포넌트")]
    [SerializeField] private Image tutorialImageDisplay;
    [Tooltip("총 6장의 튜토리얼 스프라이트 배열 (인스펙터에서 6개 등록)")]
    [SerializeField] private Sprite[] tutorialSprites = new Sprite[6];
    
    [Header("4-1. Tutorial Navigation Buttons")]
    [SerializeField] private Button prevPageButton;    // 이전 페이지 버튼
    [SerializeField] private Button nextPageButton;    // 다음 페이지 버튼
    [SerializeField] private Button closeTutorialViewerButton; // 튜토리얼 모아보기 닫기 버튼

    private int currentTutorialIndex = 0;
    private bool isOptionOpen = false;

    private void Awake()
    {
        // 옵션 버튼 리스너 바인딩
        if (mainSceneButton != null)
            mainSceneButton.onClick.AddListener(GoToMainScene);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(CloseOption);

        if (tutorialButton != null)
            tutorialButton.onClick.AddListener(OpenTutorialViewer);

        // 튜토리얼 뷰어 버튼 리스너 바인딩
        if (prevPageButton != null)
            prevPageButton.onClick.AddListener(ShowPrevPage);

        if (nextPageButton != null)
            nextPageButton.onClick.AddListener(ShowNextPage);

        if (closeTutorialViewerButton != null)
            closeTutorialViewerButton.onClick.AddListener(CloseTutorialViewer);
    }

    private void Start()
    {
        // 시작 시 패널들 비활성화
        if (optionPanel != null) optionPanel.SetActive(false);
        if (tutorialViewerPanel != null) tutorialViewerPanel.SetActive(false);
    }

    private void Update()
    {
        // ESC 키 입력 처리
        if (SimpleInput.GetKeyDown(Key.Escape))
        {
            // 튜토리얼 모아보기가 켜져 있다면 그것부터 닫음
            if (tutorialViewerPanel != null && tutorialViewerPanel.activeSelf)
            {
                CloseTutorialViewer();
            }
            else
            {
                // 옵션창 토글
                if (isOptionOpen)
                {
                    CloseOption();
                }
                else
                {
                    OpenOption();
                }
            }
        }
    }

    #region Option Panel Control
    public void OpenOption()
    {
        isOptionOpen = true;
        if (optionPanel != null) optionPanel.SetActive(true);
        Time.timeScale = 0f; // 게임 일시정지
    }

    public void CloseOption()
    {
        isOptionOpen = false;
        if (optionPanel != null) optionPanel.SetActive(false);
        if (tutorialViewerPanel != null) tutorialViewerPanel.SetActive(false);
        Time.timeScale = 1f; // 게임 일시정지 해제
    }

    public void GoToMainScene()
    {
        Time.timeScale = 1f; // 씬 넘어가기 전 시간 복원
        SceneManager.LoadScene(mainSceneName);
    }
    #endregion

    #region Tutorial Viewer Control
    public void OpenTutorialViewer()
    {
        if (tutorialViewerPanel != null)
        {
            tutorialViewerPanel.SetActive(true);
            currentTutorialIndex = 0;
            UpdateTutorialPage();
        }
    }

    public void CloseTutorialViewer()
    {
        if (tutorialViewerPanel != null)
        {
            tutorialViewerPanel.SetActive(false);
        }
    }

    public void ShowNextPage()
    {
        if (tutorialSprites == null || tutorialSprites.Length == 0) return;

        currentTutorialIndex++;
        if (currentTutorialIndex >= tutorialSprites.Length)
        {
            currentTutorialIndex = 0; // 6장이 넘어가면 다시 첫 장으로 (또는 끝에서 멈추게 설정 가능)
        }
        UpdateTutorialPage();
    }

    public void ShowPrevPage()
    {
        if (tutorialSprites == null || tutorialSprites.Length == 0) return;

        currentTutorialIndex--;
        if (currentTutorialIndex < 0)
        {
            currentTutorialIndex = tutorialSprites.Length - 1; // 첫 장에서 이전 누르면 마지막 장으로
        }
        UpdateTutorialPage();
    }

    private void UpdateTutorialPage()
    {
        if (tutorialImageDisplay != null && tutorialSprites.Length > 0 && currentTutorialIndex < tutorialSprites.Length)
        {
            tutorialImageDisplay.sprite = tutorialSprites[currentTutorialIndex];
        }
    }
    #endregion

    private void OnDestroy()
    {   
        // 이벤트 정리
        if (mainSceneButton != null) mainSceneButton.onClick.RemoveAllListeners();
        if (resumeButton != null) resumeButton.onClick.RemoveAllListeners();
        if (tutorialButton != null) tutorialButton.onClick.RemoveAllListeners();
        if (prevPageButton != null) prevPageButton.onClick.RemoveAllListeners();
        if (nextPageButton != null) nextPageButton.onClick.RemoveAllListeners();
        if (closeTutorialViewerButton != null) closeTutorialViewerButton.onClick.RemoveAllListeners();
    }
}