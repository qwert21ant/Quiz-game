import QuizQuestionType from "./QuizQuestionType";

export default interface QuizQuestion {
  type: QuizQuestionType;
  text: string;
  answer?: string;
  options?: string[];
  answerOptionInd?: number;
}