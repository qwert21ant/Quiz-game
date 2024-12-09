import QuizQuestion from "@/models/quiz/QuizQuestion";
import CommonServiceBase from "./CommonServiceBase";
import Quiz from "@/models/quiz/Quiz";

export default class QuizService extends CommonServiceBase {
  public constructor() {
    super("/quiz");
  }

  public async createQuiz(quizName: string): Promise<string> {
    return (await this.post("/create", { value: quizName }) as any).value;
  }

  public async removeQuiz(quizId: string): Promise<void> {
    return this.post("/remove", { value: quizId });
  }

  public async addQuizQuestion(quizId: string, question: QuizQuestion): Promise<void> {
    return this.post("/addQuestion", { quizId, question });
  }

  public async changeQuizQuestion(quizId: string, questionInd: number, question: QuizQuestion): Promise<void> {
    return this.post("/changeQuestion", { quizId, questionInd, question });
  }

  public async removeQuizQuestion(quizId: string, questionInd: number): Promise<void> {
    return this.post("/removeQuestion", { quizId, questionInd });
  }

  public async getQuiz(quizId: string): Promise<Quiz> {
    return this.get("", { value: quizId });
  }
}