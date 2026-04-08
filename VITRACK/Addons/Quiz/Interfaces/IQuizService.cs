using VITRACK.Addons.Quiz.DTOs;
using VITRACK.Addons.Quiz.Entities;

namespace VITRACK.Addons.Quiz.Interfaces;

public interface IQuizService
{
    // Admin
    Task<List<QuestionDto>> GetAllQuestionsAsync();
    Task<QuestionDto> CreateQuestionAsync(QuestionDto dto);
    Task<QuestionDto?> UpdateQuestionAsync(int id, QuestionDto dto);
    Task<bool> DeleteQuestionAsync(int id);
    Task ResetLeaderboardAsync();

    // Participant
    Task<List<ParticipantQuestionDto>> GetParticipantQuestionsAsync();
    Task<QuizResult> SubmitQuizAsync(SubmitQuizDto dto);

    // Global
    Task<List<QuizResult>> GetLeaderboardAsync();
}
