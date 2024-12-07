import QuizInfo from "./quiz/QuizInfo";

export default interface UserData {
  username: string;
  quizzes: QuizInfo[];
}