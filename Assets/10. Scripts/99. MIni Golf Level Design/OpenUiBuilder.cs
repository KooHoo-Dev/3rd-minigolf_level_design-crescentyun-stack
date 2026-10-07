#if UNITY_EDITOR
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Study.GolfTown
{
    /// <summary>
    /// 일시정지(옵션) UI를 코드만으로 조립하는 에디터 툴.
    /// 빈 GameObject에 붙인 뒤 컴포넌트 우클릭 메뉴(1. Build UI / 2. Clear UI)로 실행한다.
    /// 화면만 만들고 게임 로직은 연결하지 않는다.
    /// </summary>
    public class OptionUIBuilder : MonoBehaviour
    {
        private const string RootName = "option UI";
        private static readonly Vector2 ReferenceResolution = new Vector2(1920f, 1080f);

        // 버튼 규격 (기준 해상도 픽셀)
        private const float ButtonWidth = 340f;
        private const float ButtonHeight = 240f;
        private const float ButtonLip = 24f;    // 버튼 아래쪽 두께(진한 띠) 높이
        private const float ButtonGap = 90f;

        // 색상 (시안에서 추정)
        private static readonly Color DimColor = new Color(0f, 0f, 0f, 0.6f);
        private static readonly Color TitleFront = Hex("#FFA62B");
        private static readonly Color TitleDepth = Hex("#E8782A");

        private static readonly Color RedFace = Hex("#BB5A63");
        private static readonly Color RedLip = Hex("#7E3E48");
        private static readonly Color RedIcon = Hex("#5A2C35");

        private static readonly Color GreenFace = Hex("#1AAE80");
        private static readonly Color GreenLip = Hex("#12805F");
        private static readonly Color GreenIcon = Hex("#7FDCA0");

        private static readonly Color OrangeFace = Hex("#FBA52E");
        private static readonly Color OrangeLip = Hex("#C98A25");
        private static readonly Color OrangeIcon = Hex("#E07A25");

        [Header("텍스트")]
        [SerializeField] private TMP_FontAsset font;      // 한글 폰트 교체용
        [SerializeField] private bool showButtonLabels = true;

        #region 진입점

        [ContextMenu("1. Build UI")]
        private void BuildUI()
        {
            if (FindRoot() != null)
            {
                Debug.LogWarning($"[OptionUIBuilder] '{RootName}' 오브젝트가 이미 있어 생성을 중단합니다. 먼저 '2. Clear UI'를 실행하세요.");
                return;
            }

            GameObject root = CreateCanvas();
            Transform canvas = root.transform;

            CreateDim(canvas);
            Transform panel = CreateContentPanel(canvas);
            CreateTitle(panel);
            CreateButtonRow(panel);

            if (FindFirstObjectByType<EventSystem>() == null)
            {
                Debug.LogWarning("[OptionUIBuilder] 씬에 EventSystem이 없습니다. 버튼 클릭을 받으려면 추가하세요.");
            }

            Selection.activeGameObject = root;
            EditorSceneManager.MarkSceneDirty(gameObject.scene);
            Debug.Log($"[OptionUIBuilder] '{RootName}' 생성 완료");
        }

        [ContextMenu("2. Clear UI")]
        private void ClearUI()
        {
            List<GameObject> targets = new List<GameObject>();
            foreach (GameObject go in gameObject.scene.GetRootGameObjects())
            {
                if (go != gameObject && go.name == RootName)
                {
                    targets.Add(go);
                }
            }

            if (targets.Count == 0)
            {
                Debug.Log($"[OptionUIBuilder] 지울 '{RootName}' 오브젝트가 없습니다.");
                return;
            }

            foreach (GameObject go in targets)
            {
                Debug.Log($"[OptionUIBuilder] '{go.name}' 삭제");
                Undo.DestroyObjectImmediate(go);
            }

            EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }

        private GameObject FindRoot()
        {
            foreach (GameObject go in gameObject.scene.GetRootGameObjects())
            {
                if (go != gameObject && go.name == RootName)
                {
                    return go;
                }
            }
            return null;
        }

        #endregion

        #region Canvas

        private GameObject CreateCanvas()
        {
            GameObject root = new GameObject(RootName, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            SceneManager.MoveGameObjectToScene(root, gameObject.scene);
            Undo.RegisterCreatedObjectUndo(root, "Build Option UI");
            root.layer = LayerMask.NameToLayer("UI");

            Canvas canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = ReferenceResolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            return root;
        }

        #endregion

        #region 배경 / 중앙 패널

        // 화면 전체를 덮는 어두운 배경. 뒤쪽 클릭도 막는다.
        private void CreateDim(Transform canvas)
        {
            GameObject dim = NewUI("Dim", canvas);
            Stretch(dim.GetComponent<RectTransform>(), 0f);

            Image image = dim.AddComponent<Image>();
            image.color = DimColor;
            image.raycastTarget = true;
        }

        // 타이틀과 버튼줄을 담는 중앙 패널
        private Transform CreateContentPanel(Transform canvas)
        {
            GameObject panel = NewPanel("ContentPanel", canvas, new Vector2(1400f, 640f));
            return panel.transform;
        }

        #endregion

        #region 타이틀

        // 밝은 주황 글자 뒤에 진한 주황 글자를 아래로 어긋나게 깔아 입체감을 만든다.
        private void CreateTitle(Transform panel)
        {
            GameObject title = NewUI("Title", panel);
            Place(title.GetComponent<RectTransform>(),
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, 0f), new Vector2(1200f, 230f));

            TMP_Text depth = NewText("TitleDepth", title.transform, "일시 정지", 170f, TextAlignmentOptions.Center);
            depth.color = TitleDepth;
            depth.fontStyle = FontStyles.Bold;
            depth.rectTransform.anchoredPosition = new Vector2(0f, -14f);

            TMP_Text front = NewText("TitleText", title.transform, "일시 정지", 170f, TextAlignmentOptions.Center);
            front.color = TitleFront;
            front.fontStyle = FontStyles.Bold;
        }

        #endregion

        #region 버튼줄

        private void CreateButtonRow(Transform panel)
        {
            float rowWidth = ButtonWidth * 3f + ButtonGap * 2f;

            GameObject row = NewUI("ButtonRow", panel);
            Place(row.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0f, 40f), new Vector2(rowWidth, ButtonHeight));

            HorizontalLayoutGroup layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = ButtonGap;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            // 첫 번째 칸: 메인화면으로 가기
            CreateMenuButton(row.transform, "Btn_MainMenu", "메인화면",
                RedFace, RedLip, "Icon_MainMenu_Placeholder", RedIcon, new Vector2(110f, 140f));

            // 두 번째 칸: 게임 이어서 하기
            CreateMenuButton(row.transform, "Btn_Resume", "이어하기",
                GreenFace, GreenLip, "Icon_Resume_Placeholder", GreenIcon, new Vector2(120f, 130f));

            // 세 번째 칸: TIP 화면 열기
            CreateMenuButton(row.transform, "Btn_Tip", "TIP",
                OrangeFace, OrangeLip, "Icon_Tip_Placeholder", OrangeIcon, new Vector2(130f, 130f));
        }

        private void CreateMenuButton(Transform parent, string name, string label,
            Color faceColor, Color lipColor, string iconName, Color iconColor, Vector2 iconSize)
        {
            Button button = NewButton(name, parent, label, out TMP_Text labelText);

            // 아래쪽 두께 띠 = 버튼 루트 이미지, 앞면 = Face 이미지
            button.GetComponent<Image>().color = lipColor;
            Transform face = button.transform.Find("Face");
            face.GetComponent<Image>().color = faceColor;

            labelText.gameObject.SetActive(showButtonLabels);

            // 스프라이트가 없으므로 아이콘 자리는 색 블록으로 대신한다.
            GameObject icon = NewUI(iconName, face);
            Place(icon.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0f, 18f), iconSize);

            Image iconImage = icon.AddComponent<Image>();
            iconImage.color = iconColor;
            iconImage.raycastTarget = false;
        }

        #endregion

        #region 도우미 메서드

        private GameObject NewUI(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            go.layer = LayerMask.NameToLayer("UI");
            return go;
        }

        private GameObject NewPanel(string name, Transform parent, Vector2 size)
        {
            GameObject go = NewUI(name, parent);
            Place(go.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Vector2.zero, size);

            // 눈에 보이지 않는 영역 컨테이너
            Image image = go.AddComponent<Image>();
            image.color = Color.clear;
            image.raycastTarget = false;
            return go;
        }

        private TMP_Text NewText(string name, Transform parent, string text, float size, TextAlignmentOptions align)
        {
            GameObject go = NewUI(name, parent);
            Stretch(go.GetComponent<RectTransform>(), 0f);

            TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
            if (font != null)
            {
                tmp.font = font;
            }
            tmp.text = text;
            tmp.fontSize = size;
            tmp.alignment = align;
            tmp.color = Color.white;
            tmp.overflowMode = TextOverflowModes.Overflow;
            tmp.raycastTarget = false;
            return tmp;
        }

        private Button NewButton(string name, Transform parent, string label, out TMP_Text labelText)
        {
            // 루트: 버튼 아래쪽 두께 띠 역할
            GameObject root = NewUI(name, parent);
            Place(root.GetComponent<RectTransform>(),
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(ButtonWidth, ButtonHeight));
            Image lip = root.AddComponent<Image>();
            lip.color = Color.gray;

            // Face: 위쪽 앞면. 아래쪽 ButtonLip 만큼 띠가 보이도록 띄운다.
            GameObject face = NewUI("Face", root.transform);
            RectTransform faceRect = face.GetComponent<RectTransform>();
            faceRect.anchorMin = Vector2.zero;
            faceRect.anchorMax = Vector2.one;
            faceRect.pivot = new Vector2(0.5f, 0.5f);
            faceRect.offsetMin = new Vector2(0f, ButtonLip);
            faceRect.offsetMax = Vector2.zero;
            Image faceImage = face.AddComponent<Image>();
            faceImage.color = Color.white;

            Button button = root.AddComponent<Button>();
            button.targetGraphic = faceImage;

            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.92f, 0.92f, 0.92f, 1f);
            colors.pressedColor = new Color(0.75f, 0.75f, 0.75f, 1f);
            colors.selectedColor = Color.white;
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.08f;
            button.colors = colors;

            // 라벨: Face 아래쪽 가운데
            labelText = NewText("Label", face.transform, label, 40f, TextAlignmentOptions.Center);
            Place(labelText.rectTransform,
                new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0f, 14f), new Vector2(0f, 56f));

            return button;
        }

        private void Stretch(RectTransform rect, float padding)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = new Vector2(padding, padding);
            rect.offsetMax = new Vector2(-padding, -padding);
        }

        // 앵커/피벗/위치/크기를 한 번에 명시한다.
        private static void Place(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
            Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;
        }

        private static Color Hex(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out Color color);
            return color;
        }

        #endregion
    }
}
#endif