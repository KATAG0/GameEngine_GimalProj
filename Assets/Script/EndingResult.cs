/// <summary>
/// Ending 씬으로 넘어가기 전에 결과 문구를 저장합니다.
/// 씬이 바뀌어도 static 값은 유지되므로 Ending 씬 UI에서 읽을 수 있습니다.
/// </summary>
public static class EndingResult
{
    public static string Message { get; private set; } = "GameOver";

    public static void SetGameOver()
    {
        Message = "GameOver";
    }

    public static void SetClear()
    {
        Message = "Clear!";
    }
}
