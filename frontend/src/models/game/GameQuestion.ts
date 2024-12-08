import QuizQuestionType from "../quiz/QuizQuestionType";

export default interface GameQuestion {
  type: QuizQuestionType;
  text: string;
  timeLimit: number;
  options?: string[];
  questionAppearanceTime: number;
}