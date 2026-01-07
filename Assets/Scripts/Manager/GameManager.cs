using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameState currentState;

    public float targetPlayTime;    // 클리어 조건이 될 목표 시간.

    public bool ignorePlayTime = false;

    public KeyCode pauseKey = KeyCode.Escape;   // 일시 정지를 위한 키 지정.

    public KeyCode restartKey = KeyCode.R;  // 재시작을 위한 키 지정.

    public float defaultTimeScale = 1.0f;   // 기본 타임 스케일.

    public UIManager uiManager;

    private float elapsedPlayTime;  // 실제로 플레이가 진행된 누적 시간.
    private float savedTimeScale;

    private bool isPlaying; // 현재 게임이 진행중인지 여부.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = GameState.Ready;
        elapsedPlayTime = 0.0f;
        isPlaying = false;

        savedTimeScale = defaultTimeScale;
        Time.timeScale = defaultTimeScale;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentState == GameState.Playing && isPlaying == true)
        {
            if(ignorePlayTime == false)
            {
                elapsedPlayTime += Time.deltaTime;

                if (elapsedPlayTime >= targetPlayTime)
                {
                    HandleGameClear();
                }
            }
        }

        if(currentState == GameState.Ready)
        {
            if(Input.GetKeyDown(KeyCode.Space) == true)
            {
                StartGame();
            }
        }

        if(Input.GetKeyDown(pauseKey) == true)
        {
            TogglePause();
        }

        if(Input.GetKeyDown(restartKey) == true)
        {
            RestartGame();
        }
    }

    public void Pause()
    {
        if(currentState == GameState.Playing)
        {
            SetPanelPauseVisible(true);

            currentState = GameState.Paused;
            savedTimeScale = Time.timeScale;
            Time.timeScale = 0.0f;
        }
    }

    public void Resume()
    {
        if (currentState == GameState.Paused)
        {
            SetPanelPauseVisible(false);

            currentState = GameState.Playing;
            Time.timeScale = savedTimeScale;
        }
    }

    void SetPanelPauseVisible(bool visible)
    {
        if (uiManager != null)
        {
            uiManager.SetPanelPauseVisible(visible);
        }
    }

    public void TogglePause()
    {
        if(currentState == GameState.Playing)
        {
            Pause();
        }
        else if(currentState == GameState.Paused)
        {
            Resume();
        }
    }

    public void StartGame()
    {
        elapsedPlayTime = 0.0f;

        currentState = GameState.Playing;

        isPlaying = true;

        // 추구 추가할 기능.
        // 플레이어 초기화.
        // 적 스폰 시작.
        // UI 업데이트.

        Debug.Log("Game Started");
    }

    public void HandleGameOver()
    {
        currentState = GameState.GameOver;

        isPlaying = false;

        StartCoroutine(CoHandleGameOver());
    }

    IEnumerator CoHandleGameOver()
    {
        yield return new WaitForSeconds(2.0f);

        // GameOver UI 표시.
        // 재시작 버튼 노출.
        // 데이터 정리.
        if (uiManager != null)
        {
            uiManager.OpenGameOver();
        }

        Time.timeScale = 0.0f;
        currentState = GameState.PostResult;

        Debug.Log("Game Over");
    }


    public void HandleGameClear()
    {
        currentState = GameState.Clear;

        isPlaying = false;

        // 보상 처리.
        // 다음 난이도 잠금 해제.
        // 결과 화면 표시.

        if(uiManager != null)
        {
            uiManager.OpenClear();
        }

        Time.timeScale = 0.0f;
        currentState = GameState.PostResult;

        Debug.Log("Game Clear");
    }

    public void RestartGame()
    {
        Time.timeScale = defaultTimeScale;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool IsPlaying()
    {
        if(currentState == GameState.Playing && isPlaying == true)
        {
            return true;
        }

        return false;
    }

    public float GetElapsedPlayTime()
    {
        return elapsedPlayTime;
    }
}
