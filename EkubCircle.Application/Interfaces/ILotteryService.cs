using EkubCircle.Application.DTOs.Round;

namespace EkubCircle.Application.Interfaces;

public interface ILotteryService
{
    Task<RoundResponseDto> ConductLotteryDrawAsync(Guid roundId);

    Task<RoundResponseDto> GetCurrentRoundAsync(Guid circleId);

    Task<RoundResponseDto> CompleteRoundAsync(Guid circleId);

    Task<IEnumerable<RoundResponseDto>> GetCircleRoundsAsync(Guid circleId);
}