/// <summary>
/// 실수형 피해 흐름을 유지하면서 히트 문맥을 먼저 전달받기 위한 선택형 계약
/// </summary>
public interface IReceivesHitContext
{
    /// <summary>
    /// 다음 피해 적용에서 사용할 히트 문맥을 저장한다
    /// </summary>
    /// <param name="context">히트 정보</param>
    void SetHitContext(HitContext context);
}
