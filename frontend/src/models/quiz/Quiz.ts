import QuizInfo from "./QuizInfo";
import QuizQuestion from "./QuizQuestion";

export default interface Quiz {
  info: QuizInfo;
  questions: QuizQuestion[];
}