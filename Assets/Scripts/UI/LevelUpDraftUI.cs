using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 레벨업 드래프트 UI.
/// 레벨업 이벤트를 구독하여 3개 옵션을 보여주고, 선택/리롤을 처리한다.
/// </summary>
public class LevelUpDraftUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private PlayerExperience playerExperience;      // 레벨업 이벤트 소스

    [SerializeField]
    private UpgradeRuntimeState runtimeState;       // 레벨 상태

    [SerializeField]
    private UpgradePoolPlus upgradePool;                // 롤 매니저

    [SerializeField]
    private GameObject playerRoot;                  // 적용 대상(플레이어 루트)

    [SerializeField]
    private Text[] optionTexts;                     // 3개 옵션 이름 표기용

    [SerializeField]
    private Button[] optionButtons;                 // 3개 버튼(클릭 시 선택)

    [SerializeField]
    private Button rerollButton;                    // 리롤 버튼(선택사항)

    [SerializeField]
    private Text rerollCountText;                   // 남은 리롤 수 표기

    [Header("Draft Rules")]
    [SerializeField]
    private int optionCount = 3;                    // 옵션 개수

    [SerializeField]
    private int rerollCount = 1;                    // 기본 리롤 횟수

    [SerializeField]
    private GameObject panelUpgrade;

    private List<UpgradeDefinition> currentOptions = new List<UpgradeDefinition>();
    private float cachedTimeScale = 1.0f;
    private float sessionElapsed;                   // 외부에서 세션 시간 전달 대신 내부 누적(간단 버전)

    /// <summary>
    /// 초기화: 이벤트 구독 및 버튼 콜백 연결.
    /// </summary>
    private void Awake()
    {
        if (playerExperience != null)
        {
            playerExperience.OnLevelUp += HandleLevelUp;
        }

        Hide();
        UpdateRerollLabel();
    }

    /// <summary>
    /// 언스케일드 시간 누적(희귀도 가중치 보정에 사용).
    /// </summary>
    private void Update()
    {
        sessionElapsed += sessionElapsed;
    }

    /// <summary>
    /// 레벨업 이벤트 핸들러: 드래프트를 오픈한다.
    /// </summary>
    /// <param name="newLevel">상승한 레벨 값</param>
    private void HandleLevelUp(int newLevel)
    {
        OpenDraft();
    }

    /// <summary>
    /// 드래프트 UI를 연다: 옵션 롤, 일시정지, 버튼 활성화.
    /// </summary>
    private void OpenDraft()
    {
        RollAndShowOptions();

        //cachedTimeScale = Time.timeScale;
        Time.timeScale = 0.0f;

        panelUpgrade.SetActive(true);
    }

    /// <summary>
    /// 드래프트 UI를 닫고 게임을 재개한다.
    /// </summary>
    public void CloseDraft()
    {
        Time.timeScale = cachedTimeScale;
        Hide();
    }

    /// <summary>
    /// 내부적으로 옵션을 롤하고 화면에 표시한다.
    /// </summary>
    private void RollAndShowOptions()
    {
        currentOptions.Clear();

        if (upgradePool != null)
        {
            List<UpgradeDefinition> rolled = upgradePool.RollOptions(optionCount, runtimeState, sessionElapsed);
            currentOptions.AddRange(rolled);
        }

        for (int i = 0; i < optionTexts.Length; ++i)
        {
            if (i < currentOptions.Count)
            {
                UpgradeDefinition d = currentOptions[i];
                int curLv = runtimeState != null ? runtimeState.GetLevel(d.GetId()) : 0;
                int nextLv = curLv + 1;

                string label = d.GetDisplayName() + "  (Lv " + nextLv + "/" + d.GetMaxLevel() + ")";
                optionTexts[i].text = label;
                optionButtons[i].interactable = true;
            }
            else
            {
                optionTexts[i].text = "-";
                optionButtons[i].interactable = false;
            }
        }

        UpdateRerollLabel();
    }

    /// <summary>
    /// 옵션 버튼 클릭 처리: 선택 적용 후 닫기.
    /// </summary>
    public void OnOptionClicked(int index)
    {
        if (index < 0 || index >= currentOptions.Count)
        {
            return;
        }

        UpgradeDefinition def = currentOptions[index];
        if (def == null)
        {
            return;
        }

        if (runtimeState != null)
        {
            if (runtimeState.IsMaxed(def) == true)
            {
                // 방어: 이론상 롤 단계에서 배제되므로 도달하지 않아야 한다.
                return;
            }
        }

        // 1) 적용
        UpgradeApplier.ApplyOneLevel(playerRoot, def);

        // 2) 상태 증가
        if (runtimeState != null)
        {
            runtimeState.IncreaseLevel(def.GetId());
        }

        // 3) 닫기
        CloseDraft();
    }

    /// <summary>
    /// 리롤 버튼 클릭 처리: 남은 횟수 소모 후 다시 롤.
    /// </summary>
    public void OnRerollClicked()
    {
        if (rerollCount <= 0)
        {
            return;
        }

        --rerollCount;
        RollAndShowOptions();
    }

    /// <summary>
    /// UI를 숨긴다.
    /// </summary>
    private void Hide()
    {
        panelUpgrade.SetActive(false);
    }

    /// <summary>
    /// 리롤 남은 횟수 라벨 업데이트.
    /// </summary>
    private void UpdateRerollLabel()
    {
        if (rerollCountText != null)
        {
            rerollCountText.text = "Reroll: " + rerollCount;
        }
    }
}
