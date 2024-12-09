using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize]
[Route("quiz")]
public class QuizController : Controller {
  private readonly IQuizService quizService;

  public QuizController(IQuizService quizService) {
    this.quizService = quizService;
  }

  [HttpPost("create")]
  public async Task<IActionResult> CreateQuiz([FromBody] StringDTO data) {
    var user = HttpContext.User.Identity?.Name!;
    
    var id = quizService.CreateQuiz(user, data.Value);
    return Json(new StringDTO {
      Value = id
    });
  }

  [HttpPost("remove")]
  public async Task<IActionResult> RemoveQuiz([FromBody] StringDTO data) {
    var user = HttpContext.User.Identity?.Name!;
    
    quizService.RemoveQuiz(user, data.Value);
    return Ok();
  }

  [HttpPost("addQuestion")]
  public async Task<IActionResult> AddQuizQuestion([FromBody] QuizQuestionDTO data) {
    var user = HttpContext.User.Identity?.Name!;

    if (data.Question == null)
      throw new ServiceException("quiestion is null");
    
    quizService.AddQuizQuestion(user, data.QuizId, data.Question);
    return Ok();
  }

  [HttpPost("changeQuestion")]
  public async Task<IActionResult> ChangeQuizQuestion([FromBody] QuizQuestionDTO data) {
    var user = HttpContext.User.Identity?.Name!;

    if (data.QuestionInd == null)
      throw new ServiceException("quiestionInd is null");
    
    quizService.ChangeQuizQuestion(user, data.QuizId, (int) data.QuestionInd, data.Question!);
    return Ok();
  }

  [HttpPost("removeQuestion")]
  public async Task<IActionResult> RemoveQuiestion([FromBody] QuizQuestionDTO data) {
    var user = HttpContext.User.Identity?.Name!;

    if (data.QuestionInd == null)
      throw new ServiceException("quiestionInd is null");
    
    quizService.RemoveQuizQuestion(user, data.QuizId, (int) data.QuestionInd);
    return Ok();
  }

  [HttpGet()]
  public async Task<IActionResult> GetQuiz([FromQuery] StringDTO data) {
    var user = HttpContext.User.Identity?.Name!;
    
    var quiz = quizService.GetQuiz(user, data.Value);
    return Json(quiz);
  }
}