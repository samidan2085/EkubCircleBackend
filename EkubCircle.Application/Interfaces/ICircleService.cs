using EkubCircle.Application.DTOs.Circle;
using EkubCircle.Application.DTOs.Member;

namespace EkubCircle.Application.Interfaces;

public interface ICircleService
{
    Task<CircleResponseDto> CreateCircleAsync(
        CircleDetailDto request);

    Task<IEnumerable<CircleResponseDto>> GetAllCirclesAsync();

    Task<CircleDetailDto?> GetMyCircleDetailsAsync(
        string organizerId);

    Task<CircleDetailDto?> GetCircleAsync(
        Guid circleId);

    Task AddMemberAsync(
        Guid circleId,
        AddMemberDto request,
        string organizerId);

    Task<IEnumerable<MemberResponseDto>> GetMembersAsync(
        Guid circleId);

    // Start the Ekub circle
    Task StartCircleAsync(
        Guid circleId,
        string organizerId);
        Task TogglePaymentAsync(
    Guid circleId,
    int roundNumber,
    string memberId,
    string organizerId);
}